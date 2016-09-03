using System.Collections.Generic;
using Core.Models.Websites;

namespace Core.Models.Statistics
{
    public class BundleWebsiteStatisticsModel : BundleStatisticsModel
    {
        public WebsiteModel Website;

        public BundleWebsiteStatisticsModel(IList<MonthStatisticsModel> @in, IList<MonthStatisticsModel> @out,
            IList<MonthStatisticsModel> uniqueIn, IList<MonthStatisticsModel> uniqueOut, WebsiteModel website) : base(@in, @out, @uniqueIn, @uniqueOut)
        {
            Website = website;
        }
    }
}