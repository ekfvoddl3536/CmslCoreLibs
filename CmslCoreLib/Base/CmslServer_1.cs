using System;
using System.Diagnostics;
using System.Threading;

namespace CmslCore
{
    public partial class CmslServer : ICmslServer, IDisposable
    {
        #region fields
        public event Action<string> OutputTextChangeEvent;
        private ManualResetEvent doWait;
        private bool EndReadCmd;
        #endregion

        #region 프로세서
        /// <summary>
        /// 프로세서 Data와 Error의 Text를 받습니다.
        /// </summary>
        protected virtual void TextReceived(object sender, DataReceivedEventArgs e) => OutputTextChangeEvent?.Invoke(e.Data);

        private string ArgumentParse() =>
            !IsNullArgs
                ? jvm_line
                :
            string.Concat(javapath ?? "java",
                " -Xmx", m_maxiRam,
                "M -Xms", m_miniRam,
                "M -Djline.terminal=jline.UnsupportedTerminal -jar ",
                corepath ?? "file.jar");

        private void ProcessInit(string f) =>
            process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = f,
                    Arguments = string.Concat(@"/c ", ArgumentParse(), " & exit")
                }
            };

        /// <summary>
        /// 명령어를 전달합니다.
        /// </summary>
        /// <param name="text">명령어</param>
        public virtual void StdInput(string text)
        {
            if (!IsRunning || string.IsNullOrWhiteSpace(text)) 
                return;
            process.StandardInput.WriteLine(text);
        }

        /// <summary>
        /// 서버를 시작합니다.
        /// </summary>
        /// <param name="folder">서버 폴더를 지정합니다.</param>
        /// <returns>정상적으로 구동된 경우 True를 반환합니다.</returns>
        public virtual bool Start(string folder)
        {
            if (!IsCreated && IsNullArgs || IsRunning) 
                return false;

            ProcessInit(folder);

            process.OutputDataReceived += TextReceived;
            process.ErrorDataReceived += TextReceived;

            process.EnableRaisingEvents = true;
            process.Exited += Process_Exited;

            process.Start();

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            return true;
        }

        /// <summary>
        /// 서버를 종료합니다.
        /// </summary>
        /// <returns>정상적으로 종료된 경우 True를 반환합니다.</returns>
        public virtual bool Abort()
        {
            if (!IsRunning)
                return false;

            CancelReadCommand();
            process.StandardInput.WriteLine("stop");
            process.StandardInput.Close();

            ProcessDispose();
            return true;
        }

        /// <summary>
        /// 프로세서가 종료될 때 호출됩니다.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        protected virtual void Process_Exited(object sender, EventArgs e) =>
            ProcessDispose();

        /// <summary>
        /// 프로세서를 해제합니다.
        /// </summary>
        protected virtual void ProcessDispose()
        {
            process.CancelOutputRead();
            process.CancelErrorRead();

            process.OutputDataReceived -= TextReceived;
            process.ErrorDataReceived -= TextReceived;
            process.Exited -= Process_Exited;

            if (process.MainWindowHandle != IntPtr.Zero)
                process.CloseMainWindow();

            process.Close();
            process.Dispose();

            process = null;

            EventCall();
        }
        #endregion

        #region 스레드 진행 관리
        /// <summary>
        /// 다음 신호가 발생하기 전까지 무기한 대기합니다.
        /// 보통 서버가 종료될때까지 대기합니다.
        /// </summary>
        public virtual void Wait()
        {
            if (doWait == null)
                CreateThreadWait(false);

            WaitHandle.WaitAll(new ManualResetEvent[] { doWait }, Timeout.Infinite);
        }

        /// <summary>
        /// 명령 읽어 들이기를 시작합니다. 콘솔에서 유효합니다.
        /// </summary>
        public virtual void BeginReadCommand()
        {
            while (process != null)
            {
                string temp = Console.ReadLine();
                if (process == null || EndReadCmd) break;
                else if (string.IsNullOrWhiteSpace(temp)) continue;
                process.StandardInput.WriteLine(temp);
            }
        }

        /// <summary>
        /// 지정한 횟수만큼 명령을 읽어 들입니다. 콘솔에서 유효합니다.
        /// </summary>
        /// <param name="max">값</param>
        public virtual void BeginReadCommand(uint max)
        {
            for (uint i = 0; i < max; i++)
            {
                string temp = Console.ReadLine();
                if (process == null || EndReadCmd) break;
                else if (string.IsNullOrWhiteSpace(temp)) continue;
                process.StandardInput.WriteLine(temp);
            }
        }

        /// <summary>
        /// 명령 읽어 들이기를 중단합니다.
        /// </summary>
        public virtual void CancelReadCommand()
        {
            Console.SetCursorPosition(0, Console.CursorTop--);
            EndReadCmd = true;
            if (doWait != null) doWait.Set();
        }

        private void EventCall()
        {
            if (doWait == null) 
                return;
            else
                doWait.Set();
        }
        #endregion

        #region 값들
        /// <summary>
        /// GB 단위로 메모리 값을 다시 정의합니다.
        /// </summary>
        /// <param name="min">최저 메모리 값</param>
        /// <param name="max">최대 메모리 값</param>
        public void ResizeMemoryGB(short min, short max)
        {
            if (IsRunning || min < max) return;
            m_miniRam = min * 1024;
            m_maxiRam = max * 1024;
        }

        /// <summary>
        /// MB 단위로 메모리 값을 다시 정의합니다.
        /// </summary>
        /// <param name="min">최저 메모리 값</param>
        /// <param name="max">최대 메모리 값</param>
        public void ResizeMemoryMB(int min, int max)
        {
            if (IsRunning || min < max) return;
            m_miniRam = min;
            m_maxiRam = max;
        }
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsRunning)
                Abort();
            if (process != null)
                ProcessDispose();
            doWait.Dispose();
        }
    }
}
