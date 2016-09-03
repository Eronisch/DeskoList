using Core.Models.Statistics;

namespace Topsite.Models
{
    public class ViewWebsiteModel
    {
        public readonly BundleWebsiteStatisticsModel BundleWebsiteStatistics;
        public WebsiteRatingModel WebsiteRating { get; set; }
        public readonly bool HasRated;

        public ViewWebsiteModel(BundleWebsiteStatisticsModel bundleWebsiteStatistics, bool hasRated)
        {
            BundleWebsiteStatistics = bundleWebsiteStatistics;
            WebsiteRating = new WebsiteRatingModel(bundleWebsiteStatistics.Website.Id);
            HasRated = hasRated;
        }
    }
}