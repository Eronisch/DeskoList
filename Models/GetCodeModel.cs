using Core.Models.Websites;

namespace Topsite.Models
{
    public class GetCodeModel
    {
        public GetCodeModel(string siteTitle, string hostUrl, WebsiteModel website)
        {
            TopsiteTitle = siteTitle;
            HostUrl = hostUrl;
            Website = website;
        }

        public string TopsiteTitle { get; private set; }
        public WebsiteModel Website { get; private set; }
        public string HostUrl { get; set; }
    }
}