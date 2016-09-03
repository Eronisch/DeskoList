using System;
using System.IO;
using System.Net;
using Core.Business.Settings;
using Core.Models;
using Newtonsoft.Json;

namespace Core.Business.Captcha
{
    public class CaptchaService
    {
        private readonly SettingsService _settingsService;
        private const string VerificationUrl = "https://www.google.com/recaptcha/api/siteverify";

        public CaptchaService()
        {
            _settingsService = new SettingsService();
        }

        /// <summary>
        /// Performs a web request to googles captcha service to validate the response
        /// Timeouts after 10 seconds
        /// </summary>
        /// <param name="responseCaptcha">g-recaptcha-response from the form</param>
        /// <param name="ip">Ip from the user</param>
        /// <returns>True if valid response or an exception occurs</returns>
        public ResultModel ValidateCaptcha(string responseCaptcha, string ip)
        {
            string url = string.Format("{0}?secret={1}&response={2}&remoteip={3}", VerificationUrl, _settingsService.GetCaptcha().RepcaptchaSecretKey, responseCaptcha, ip);

            var webRequest = WebRequest.Create(url);
            webRequest.Timeout = (int) TimeSpan.FromSeconds(10).TotalMilliseconds;

            try
            {
                using (var webResponse = webRequest.GetResponse())
                {
                    var responseStream = webResponse.GetResponseStream();
                    var response = new StreamReader(responseStream).ReadToEnd();
                    dynamic jsonResult = JsonConvert.DeserializeObject(response);
                    bool isSuccess = jsonResult.success == "true";

                    return
                        new ResultModel(!isSuccess
                            ? Localization.Languages.Business.Captcha.CaptchaService.InvalidCaptcha
                            : string.Empty);
                }
            }
            catch(Exception ex)
            {
                if (ex is WebException || ex is IOException)
                {
                    return new ResultModel();    
                }

                throw;
            }
        }
    }
}
