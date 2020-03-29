using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

namespace CmslCore.Net
{
    using static ConstantDatas;
    public static class FileDownloader
    {
        public static WebClient GetClient()
        {
            WebClient wc = new WebClient();
            wc.Headers.Add(useragent, header);
            return wc;
        }

        public static WebClient Start(string uri, string path)
        {
            WebClient wc = GetClient();
            wc.DownloadFileTaskAsync(uri, path);
            return wc;
        }

        public static WebClient Start(string uri, string path, DownloadProgressChangedEventHandler handler)
        {
            WebClient wc = GetClient();
            wc.DownloadProgressChanged += handler;
            wc.DownloadFileTaskAsync(uri, path);
            return wc;
        }

        public static WebClient Start(string uri, string path, DownloadProgressChangedEventHandler h1, AsyncCompletedEventHandler h2)
        {
            WebClient wc = GetClient();
            wc.DownloadProgressChanged += h1;
            wc.DownloadFileCompleted += h2;
            wc.DownloadFileTaskAsync(uri, path);
            return wc;
        }

        public static async Task StartAsyncAutomatic(string uri, string path, DownloadProgressChangedEventHandler handler)
        {
            WebClient wc = GetClient();
            wc.DownloadProgressChanged += handler;
            await wc.DownloadFileTaskAsync(uri, path);

            wc.DownloadProgressChanged -= handler;
            wc.Dispose();
        }

        public static async Task StartAsyncAutomatic(string uri, string path, DownloadProgressChangedEventHandler h1, AsyncCompletedEventHandler h2)
        {
            WebClient wc = GetClient();
            wc.DownloadProgressChanged += h1;
            wc.DownloadFileCompleted += h2;
            await wc.DownloadFileTaskAsync(uri, path);

            wc.DownloadProgressChanged -= h1;
            wc.DownloadFileCompleted -= h2;

            wc.Dispose();
        }

        public static Task Start(out WebClient result, string uri, string path)
        {
            result = GetClient();
            return result.DownloadFileTaskAsync(uri, path);
        }

        public static Task Start(out WebClient result, string uri, string path, DownloadProgressChangedEventHandler handler)
        {
            result = GetClient();
            result.DownloadProgressChanged += handler;
            return result.DownloadFileTaskAsync(uri, path);
        }

        public static Task Start(out WebClient result, string uri, string path, DownloadProgressChangedEventHandler h1, AsyncCompletedEventHandler h2)
        {
            result = GetClient();
            result.DownloadProgressChanged += h1;
            result.DownloadFileCompleted += h2;
            return result.DownloadFileTaskAsync(uri, path);
        }
    }
}
