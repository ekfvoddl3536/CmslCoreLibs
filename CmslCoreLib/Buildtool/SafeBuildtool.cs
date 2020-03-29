using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Timers;

namespace CmslCore.Buildtool
{
    using static CmslRegistry;
    public sealed class SafeBuildtool : IDisposable
    {
        public const string Name = "BuildTools";

        // cb null 오류를 예방하기 위함
        private static void NOP_CB(string str) { }

        #region fields
        private Timer timer;
        private float percent;
        private string storagepath, javapath, gitpath;
        private Action<string> cb;
        #endregion

        #region 본문
        public SafeBuildtool(string storagepath, string javapath, string gitpath, Action<string> cb, int cb_interval = 1000)
        {
            Debug.Assert(storagepath != null, $"null: {nameof(storagepath)}");

            timer = new Timer(cb_interval);
            timer.Elapsed += Timer_Elapsed;
            percent = 0;

            this.storagepath = storagepath;
            this.javapath = javapath;
            this.gitpath = gitpath;

            this.cb = cb ?? NOP_CB;
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e) => cb.Invoke(e.Data);

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) => percent = e.BytesReceived / (float)e.TotalBytesToReceive * 100;

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) => cb.Invoke(string.Format("[INFO] {0:P2} 진행", percent));

        public bool IsCanStart(string javaur, bool allowDownloadGit)
        {
            // GetValue(useportablegit, false)
            if (IsNullKey(javapath) && string.IsNullOrWhiteSpace(javaur))
                cb.Invoke("[ERROR] 심각한 오류: Java (을)를 찾을 수 없습니다.");
            if (IsNullKey(gitpath) && !allowDownloadGit)
                cb.Invoke("[ERROR] 심각한 오류: Git (을)를 찾지 못했으며, Portable-Git 다운로드 및 사용이 금지되어 있습니다.");
            else
                return true;
            return false;
        }

        public async Task EnsureBuildTools()
        {
            string bdt = storagepath + $"\\{Name}\\{Name}.jar";
            if (File.Exists(bdt))
                return;

            percent = 0;
            cb.Invoke($"[WARN] {Name} 을(를) 찾지 못했습니다.");
            cb.Invoke($"[INFO] {Name} 다운로드를 시작합니다");
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(ConstantDatas.useragent, ConstantDatas.header);
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;

                timer.Start();
                await wc.DownloadFileTaskAsync(ConstantDatas.Links.BuildTool, bdt);

                timer.Stop();
                wc.DownloadProgressChanged -= Wc_DownloadProgressChanged;
            }
        }

        public void ConvertAuto(string working_directory, string java, string version, string folder, bool copy_mode)
        {
            // working_directory = GetValue(storagepath, ConstantDatas.DefaultStorageFolder) + $"\\{Name}";
            string spi = $"{working_directory}\\spigot-{version}.jar";
            string nfi = $"{folder}\\file.jar";
            if (File.Exists(spi)) 
                cb.Invoke("[INFO] 이미 컨버팅된 파일을 발견했습니다.");
            else
            {
                cb.Invoke("[INFO] 컨버팅을 시작합니다.");
                using (Process p = new Process())
                {
                    p.StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        WorkingDirectory = working_directory,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = string.Format("/c \"{0}\" -jar BuildTools.jar --rev {1}", java ?? GetValue(javapath, "java"), version)
                    };
                    p.OutputDataReceived += OutputDataReceived;
                    p.ErrorDataReceived += OutputDataReceived;
                    p.Start();

                    p.BeginErrorReadLine();
                    p.BeginOutputReadLine();

                    p.WaitForExit();

                    p.OutputDataReceived -= OutputDataReceived;
                    p.ErrorDataReceived -= OutputDataReceived;
                }

                if (File.Exists(spi))
                {
                    cb.Invoke("[INFO] 컨버팅에 성공했습니다.");
                    if (copy_mode)
                        File.Copy(spi, nfi, true);
                    else
                        File.Move(spi, nfi);
                }
                else
                    cb.Invoke("[ERROR] 컨버팅에 실패했습니다.");
            }
        }

        public void Dispose()
        {
            timer.Dispose();
            timer = null;

            storagepath = null;
            javapath = null;
            gitpath = null;
            cb = null;

            percent = 0;
            GC.Collect(0, GCCollectionMode.Default, false, false);
        }
        #endregion
    }
}
