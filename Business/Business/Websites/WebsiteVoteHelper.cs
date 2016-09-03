using Core.Business.Url;

namespace Core.Business.Websites
{
    public static class WebsiteVoteHelper
    {
        public static string GetVotingImage(string relativeImagePath)
        {
            return string.Format("{0}/{1}", UrlHelpers.GetCurrentBaseUrl(), relativeImagePath);
        }

        public static string GetVotingUrl(string username, string websiteId, string redirectUrl = null)
        {
            string baseVotingUrl = string.Format("{0}/Website/Vote/{1}/{2}", UrlHelpers.GetCurrentBaseUrl(), username, websiteId);

            return string.IsNullOrEmpty(redirectUrl)
                ? baseVotingUrl
                : string.Format("{0}?redirect={1}", baseVotingUrl, redirectUrl);
        }

        public static string GetApiUrl(string websiteId)
        {
            return string.Format("{0}/Website/CheckVote?id={1}", UrlHelpers.GetCurrentBaseUrl(), websiteId);
        }
    }
}
