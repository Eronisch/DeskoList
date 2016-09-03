using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Models.Websites;

namespace Topsite.Models
{
    public class BundleWebsitesRanking
    {
        public IEnumerable<WebsiteModel> Websites { get; set; }

        public int StartIndex { get; set; }
        public bool IsMonitoringEnabled { get; set; }
        public bool IsThumbnailEnabled { get; set; }

        public BundleWebsitesRanking()
        {
            Websites = new Collection<WebsiteModel>();
            StartIndex = 1;
        }
    }
}