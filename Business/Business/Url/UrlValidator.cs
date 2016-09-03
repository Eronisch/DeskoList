using System;

namespace Core.Business.Url
{
    public static class UrlValidator
    {
        public static bool ValidateUrl(string url)
        {
            Uri localUrl;
            return Uri.TryCreate(url, UriKind.Absolute, out localUrl);
        }
    }
}
