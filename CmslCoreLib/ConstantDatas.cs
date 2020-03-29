#pragma warning disable CA1034
namespace CmslCore
{
    public static class ConstantDatas
    {
        public const string useragent = "User-Agent";
        public const string header = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063";

        public const string Java64path = @"C:\Program Files\Java\";
        public const string Java86path = @"C:\Program Files (x86)\Java\";
        public const string Git64path = @"C:\Program Files\Git\git-cmd.exe";
        public const string Git86path = @"C:Program Files (x86)\Git\git-cmd.exe";

        public static class Links
        {
            public const string BuildTool =
                "https://hub.spigotmc.org/jenkins/job/BuildTools/lastSuccessfulBuild/artifact/target/BuildTools.jar";
        }
    }
}
