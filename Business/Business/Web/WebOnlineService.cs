using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Elmah;

namespace Core.Business.Web
{
    public class WebOnlineService
    {
        /// <summary>
        /// Downloads the file to the given path
        /// </summary>
        /// <param name="urlDownload"></param>
        /// <param name="path"></param>
        /// <param name="handlerProgressChanged"></param>
        /// <param name="additionalChangedParameter"></param>
        /// <param name="handlerCompleted"></param>
        /// <param name="additionalCompletedParameter"></param>
        public async Task DownloadAsyncFile(string urlDownload, string path, Action<object, DownloadProgressChangedEventArgs, object> handlerProgressChanged, object additionalChangedParameter, Action<object, AsyncCompletedEventArgs, object> handlerCompleted, object additionalCompletedParameter)
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadFileCompleted += (sender, e) => handlerCompleted(sender, e, additionalChangedParameter);

                    webClient.DownloadProgressChanged += (sender, e) => handlerProgressChanged(sender, e, additionalCompletedParameter);

                    await webClient.DownloadFileTaskAsync(new Uri(urlDownload), path);
                }
                catch (WebException ex)
                {
                    ErrorLog.GetDefault(null).Log(new Error(ex));
                }
            }
        }

        public void DownloadFile(string urlDownload, string path)
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadFile(new Uri(urlDownload), path);
                }
                catch (WebException ex)
                {
                    ErrorLog.GetDefault(null).Log(new Error(ex));
                }
            }
        }

        public string DownloadPage(string urlDownload)
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    return webClient.DownloadString(new Uri(urlDownload));
                }
                catch (WebException ex)
                {
                    ErrorLog.GetDefault(null).Log(new Error(ex));
                }
            }
            return string.Empty;
        }
    }
}
