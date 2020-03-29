using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CmslCore.Analysis
{
    public static class CmslLogger
    {
        private static StreamWriter fs;

        public static bool IsValid => fs != null;

        public static void Initialize(string path)
        {
            Debug.Assert(fs == null, "Initialize 함수를 여러번 호출하지 마십시오.");

            string temp = Path.GetDirectoryName(path);
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);
            if (File.Exists(path))
                File.Delete(path);

            fs = new StreamWriter(
                new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read),
                Encoding.UTF8);

            Log("CmslLogger가 준비되었습니다.");
            WarnIf(!CmslRegistry.IsInitialized, "CmslRegistry 가 아직 준비되지 않았습니다.");
            WarnIf(CmslRegistry.IsInitialized && CmslRegistry.IsEmptyTree, "레지스트리 트리가 누락되어 있습니다.");
            Log("메인 프로그램을 로딩합니다...");
        }

        public static void Log(string message) => fs.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] {message}");

        public static void LogIf(bool condition, string message)
        {
            if (condition)
                Log(message);
        }

        public static void Warn(string message) => fs.WriteLine($"[{DateTime.Now:HH:mm:ss}] [WARN] {message}");

        public static void WarnIf(bool condition, string message)
        {
            if (condition)
                Warn(message);
        }

        public static void Error(string message) => fs.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR] {message}");

        public static void ErrorIf(bool condition, string message)
        {
            if (condition)
                Error(message);
        }

        public static void Close(Exception exception)
        {
            if (exception != null)
            {
                Error("오류가 발생했습니다.");
                Error(exception.ToString());
                Exception foo = exception.InnerException;
                while (foo != null)
                {
                    Error("Rethrow Exception");
                    Error(foo.ToString());
                    foo = foo.InnerException;
                }
            }
            else
                Log("내부 스레드를 종료합니다.");

            fs.Flush();
            fs.Close();
        }
    }
}
