using System.Diagnostics;
using System.Threading;

namespace CmslCore
{
    /// <summary>
    /// 기본이 되는 클래스이며 상속될 수 있습니다.
    /// </summary>
    partial class CmslServer
    {
        /// <summary>
        /// 최저 메모리 값
        /// </summary>
        protected int m_miniRam;
        /// <summary>
        /// 최대 메모리 값
        /// </summary>
        protected int m_maxiRam;
        /// <summary>
        /// 자바 경로, 설정하지 않은 경우 null
        /// </summary>
        protected string javapath;
        /// <summary>
        /// 서버 구현 프로그램 경로, 설정하지 않은 경우 null
        /// </summary>
        protected string corepath;
        /// <summary>
        /// jvm 인수, null이 아닌 경우 jvm 인수를 새로 만들지 않음
        /// </summary>
        protected string jvm_line;
        /// <summary>
        /// 기본 프로세서 입니다.
        /// </summary>
        protected Process process;

        #region 생성자
        private CmslServer(int min, int max)
        {
            m_miniRam = min;
            m_maxiRam = max;
        }

        private CmslServer(int min, int max, string java) : this(min, max) => javapath = string.Concat("\"", java, "\"");

        private CmslServer(string core, int min, int max) : this(min, max) => corepath = string.Concat("\"", core, "\"");

        private CmslServer(int min, int max, string java, string core) : this(min, max)
        {
            javapath = string.Concat("\"", java, "\"");
            corepath = string.Concat("\"", core, "\"");
        }

        /// <summary>
        /// JVM 인수로 초기화합니다.
        /// </summary>
        /// <param name="jvm_args">JVM 인수를 담은 문자열 입니다.</param>
        public CmslServer(string jvm_args) =>
            jvm_line = jvm_args;

        /// <summary>
        /// 빈 데이터를 만듭니다.
        /// </summary>
        public CmslServer() { }

        /// <summary>
        /// GB 단위로 메모리를 설정합니다.
        /// </summary>
        /// <param name="min">최저 메모리값 입니다.</param>
        /// <param name="max">최대 메모리값 입니다.</param>
        /// <returns>초기화된 CmslServer</returns>
        public static CmslServer Create(short min, short max) =>
            min > max 
            ? null 
            : new CmslServer(min * 1024, max * 1024);

        /// <summary>
        /// GB 단위로 메모리를 설정하고, java.exe 경로를 수동으로 초기화합니다.
        /// </summary>
        /// <param name="min">최저 메모리값 입니다.</param>
        /// <param name="max">최대 메모리값 입니다.</param>
        /// <param name="java">자바 경로</param>
        /// <returns>초기화된 CmslServer</returns>
        public static CmslServer Create(short min, short max, string java) =>
            min > max || string.IsNullOrWhiteSpace(java) 
            ? null 
            : new CmslServer(min * 1024, max * 1024, java);

        /// <summary>
        /// GB 단위로 메모리를 설정하고, 서버 구현 프로그램(버킷.jar) 경로를 수동으로 초기화합니다.
        /// </summary>
        /// <param name="min">최저 메모리값 입니다.</param>
        /// <param name="max">최대 메모리값 입니다.</param>
        /// <param name="core">코어 파일 (경로 또는 이름)</param>
        /// <returns>초기화된 CmslServer</returns>
        public static CmslServer Create(string core, short min, short max) =>
            min > max || string.IsNullOrWhiteSpace(core) 
            ? null
            : new CmslServer(core, min * 1024, max * 1024);

        /// <summary>
        /// GB 단위로 메모리를 설정하고
        /// java.exe 경로 및 서버 구현 프로그램(버킷.jar) 경로를 수동으로 초기화합니다.
        /// </summary>
        /// <param name="min">최저 메모리값 입니다.</param>
        /// <param name="max">최대 메모리값 입니다.</param>
        /// <param name="java">자바 경로</param>
        /// <param name="core">코어 파일 (경로 또는 이름)</param>
        /// <returns>초기화된 CmslServer</returns>
        public static CmslServer Create(short min, short max, string java, string core) =>
            min > max || string.IsNullOrWhiteSpace(java) || string.IsNullOrWhiteSpace(core) 
            ? null 
            : new CmslServer(min * 1024, max * 1024, java, core);

        /// <summary>
        /// MB 단위로 메모리를 설정합니다.
        /// </summary>
        /// <param name="min">최저 메모리값 입니다.</param>
        /// <param name="max">최대 메모리값 입니다.</param>
        /// <returns>초기화된 CmslServer</returns>
        public static CmslServer Create(int min, int max) =>
            min > max 
            ? null 
            : new CmslServer(min, max);

        /// <summary>
        /// MB 단위로 메모리를 설정하고, java.exe 경로를 수동으로 초기화합니다.
        /// </summary>
        /// <param name="min">최저 메모리값 입니다.</param>
        /// <param name="max">최대 메모리값 입니다.</param>
        /// <param name="java">자바 경로</param>
        /// <returns>초기화된 CmslServer</returns>
        public static CmslServer Create(int min, int max, string java) =>
            min > max || string.IsNullOrWhiteSpace(java) 
            ? null
            : new CmslServer(min, max, java);

        /// <summary>
        /// MB 단위로 메모리를 설정하고, 서버 구현 프로그램(버킷.jar) 경로를 수동으로 초기화합니다.
        /// </summary>
        /// <param name="min">최저 메모리값 입니다.</param>
        /// <param name="max">최대 메모리값 입니다.</param>
        /// <param name="core">코어 파일 (경로 또는 이름)</param>
        /// <returns>초기화된 CmslServer</returns>
        public static CmslServer Create(string core, int min, int max) =>
            min > max || string.IsNullOrWhiteSpace(core) 
            ? null
            : new CmslServer(core, min, max);

        /// <summary>
        /// MB 단위로 메모리를 설정하고
        /// java.exe 경로 및 서버 구현 프로그램(버킷.jar) 경로를 수동으로 초기화합니다.
        /// </summary>
        /// <param name="min">최저 메모리값 입니다.</param>
        /// <param name="max">최대 메모리값 입니다.</param>
        /// <param name="java">자바 경로</param>
        /// <param name="core">코어 파일 (경로 또는 이름)</param>
        /// <returns>초기화된 CmslServer</returns>
        public static CmslServer Create(int min, int max, string java, string core) =>
            min > max || string.IsNullOrWhiteSpace(java) || string.IsNullOrWhiteSpace(core)
            ? null 
            : new CmslServer(min, max, java, core);
        #endregion

        #region 반환
        /// <summary>
        /// 설정된 최저 메모리 값을 가져옵니다.
        /// </summary>
        public virtual int GetMinimumSize => m_miniRam;

        /// <summary>
        /// 설정된 최대 메모리 값을 가져옵니다.
        /// </summary>
        public virtual int GetMaxinumSize => m_maxiRam;

        /// <summary>
        /// 설정된 java.exe 경로를 가져옵니다.
        /// </summary>
        public virtual string GetJavaPath => javapath;

        /// <summary>
        /// 설정된 코어 파일 경로를 가져옵니다.
        /// </summary>
        public virtual string GetCorePath => corepath;

        /// <summary>
        /// 설정된 JVM 인수를 가져옵니다.
        /// </summary>
        public virtual string GetJVMArgs => jvm_line;

        /// <summary>
        /// JVM 인수로 초기화한 경우 True 이며 그렇지 않은 경우 False 입니다.
        /// </summary>
        public virtual bool IsNullArgs => jvm_line == null;

        /// <summary>
        /// 서버의 데이터가 만들어진 경우 True이며 그렇지 않은 경우 False를 반환하며
        /// 최저 메모리가 1MB 미만일 경우 False를 반환합니다. (구동 불가)
        /// </summary>
        public virtual bool IsCreated => m_miniRam > 1 && m_maxiRam > 1 || jvm_line != null;

        /// <summary>
        /// 현재 서버가 구동중이면 True 이며 그렇지 않은 경우 False를 반환합니다.
        /// </summary>
        public virtual bool IsRunning => process != null;
        #endregion

        #region 추가 메소드
        /// <summary>
        /// 스레드를 다음 이벤트 발생전까지 무기한 대기시키는 이벤트를 초기화합니다.
        /// </summary>
        /// <param name="f">true 초기 상태를 설정 하려면를 신호 받음 false 에 초기 상태를 신호 없음으로 설정 합니다.</param>
        protected virtual void CreateThreadWait(bool f) => doWait = new ManualResetEvent(f);
        #endregion
    }
}