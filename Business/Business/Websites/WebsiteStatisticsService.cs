using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models.Statistics;

namespace Core.Business.Websites
{
    public class WebsiteStatisticsService
    {
        private readonly WebsiteService _websiteService = new WebsiteService();

        public IEnumerable<BundleStatisticsModel> GetStatistics(int userId, bool isMonths)
        {
            var userWebsites = _websiteService.GetWebsitesDbFromUser(userId);

            return userWebsites.Select(website => GetStatisticsFromWebsite(website.Id, isMonths));
        }

        public BundleStatisticsModel GetStatisticsFromWebsite(int websiteId, bool isMonths)
        {
            var website = _websiteService.GetWebsite(websiteId);
            return GetStatisticsFromWebsite(website, isMonths);
        }

        public BundleStatisticsModel GetStatisticsFromWebsite(Database.Entities.Websites website, bool isMonths)
        {
            var @in = (isMonths ? GetInMonths(website, false) : GetInDays(website, false)).ToList();
            var uniqueIn = (isMonths ? GetInMonths(website, true) : GetInDays(website, true)).ToList();
            var @out = (isMonths ? GetOutMonths(website, false) : GetOutDays(website, false)).ToList();
            var uniqueOut = (isMonths ? GetOutMonths(website, true) : GetOutDays(website, true)).ToList();

            return new BundleStatisticsModel(@in, @out, uniqueIn, uniqueOut);
        }

        /// <summary>
        /// Get website statistics including website information
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ip"></param>
        /// <param name="isMonths"></param>
        /// <returns></returns>
        public IEnumerable<BundleStatisticsModel> GetBundleStatisticsFromUser(int userId, string ip, bool isMonths)
        {
            var websites = _websiteService.GetWebsitesDbFromUser(userId);

            return from website in websites
                let statistics = GetStatisticsFromWebsite(website.Id, isMonths)
                select
                    new BundleWebsiteStatisticsModel(statistics.In, statistics.Out, statistics.UniqueIn,
                        statistics.UniqueOut, website);
        }

        private IEnumerable<MonthStatisticsModel> GetInMonths(Database.Entities.Websites website, bool includeOnlyUnique)
        {
            for (int counterMonths = 0; 6 > counterMonths; counterMonths++)
            {
                var currentDate = DateTime.Now.AddMonths(-counterMonths);

                var returnDate = new DateTime(currentDate.Year, currentDate.Month, 1);

                var websiteVotes = from websiteVote in website.WebsiteInDaily
                                   where websiteVote.Date.Year == currentDate.Year
                                   && websiteVote.Date.Month == currentDate.Month
                                   select websiteVote;

                yield return new MonthStatisticsModel
                {
                    Amount = includeOnlyUnique ? websiteVotes.Sum(x => x.UniqueIn) : websiteVotes.Sum(x => x.TotalIn),
                    Date = returnDate
                };
            }
        }

        private IEnumerable<MonthStatisticsModel> GetOutMonths(Database.Entities.Websites website, bool includeOnlyUnique)
        {
            for (int counterMonths = 0; 6 > counterMonths; counterMonths++)
            {
                var currentDate = DateTime.Now.AddMonths(-counterMonths);

                var returnDate = new DateTime(currentDate.Year, currentDate.Month, 1);

                var websiteRedirects = from websiteRedirect in website.WebsiteOutDaily
                                       where websiteRedirect.Date.Year == currentDate.Year && websiteRedirect.Date.Month == currentDate.Month
                                       select websiteRedirect;

                yield return new MonthStatisticsModel
                {
                    Amount = includeOnlyUnique ? websiteRedirects.Sum(x => x.UniqueOut) : websiteRedirects.Sum(x => x.TotalOut),
                    Date = returnDate
                };
            }
        }

        private IEnumerable<MonthStatisticsModel> GetInDays(Database.Entities.Websites website, bool includeOnlyUnique)
        {
            for (int counterDays = 0; 6 > counterDays; counterDays++)
            {
                var currentDate = DateTime.Now.AddDays(-counterDays);

                var websiteVotes = (from websiteVote in website.WebsiteInDaily
                                    where websiteVote.Date.Year == currentDate.Year
                                    && websiteVote.Date.Month == currentDate.Month
                                    && websiteVote.Date.Day == currentDate.Day
                                    select websiteVote);

                yield return new MonthStatisticsModel
                {
                    Amount = includeOnlyUnique ? websiteVotes.Sum(x => x.UniqueIn) : websiteVotes.Sum(x => x.TotalIn),
                    Date = currentDate
                };
            }
        }

        private IEnumerable<MonthStatisticsModel> GetOutDays(Database.Entities.Websites website, bool includeOnlyUnique)
        {
            for (int counterDays = 0; 6 > counterDays; counterDays++)
            {
                var currentDate = DateTime.Now.AddDays(-counterDays);

                var websiteRedirects = (from websiteRedirect in website.WebsiteOutDaily
                                        where websiteRedirect.Date.Year == currentDate.Year
                                        && websiteRedirect.Date.Month == currentDate.Month
                                        && websiteRedirect.Date.Day == currentDate.Day
                                        select websiteRedirect);

                yield return new MonthStatisticsModel
                {
                    Amount = includeOnlyUnique ? websiteRedirects.Sum(x => x.UniqueOut) : websiteRedirects.Sum(x => x.TotalOut),
                    Date = currentDate
                };
            }
        }
    }
}
