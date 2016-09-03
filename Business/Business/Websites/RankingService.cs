using System;
using System.Collections.Generic;
using System.Linq;
using Core.Business.Settings;
using Core.Models.Websites;
using Database;

namespace Core.Business.Websites
{
    public class RankingService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();
        private readonly SettingsService _settingsService = new SettingsService();

        public IEnumerable<WebsiteModel> GetWebsites(int page = 1)
        {
            int amountWebsites = GetAmountWebsitesToShow();

            return GetQueryWebsites().Skip((page - 1) * amountWebsites)
                .Take(amountWebsites).ToList().Select(w => new WebsiteModel(w));
        }

        public IEnumerable<WebsiteModel> GetWebsites(int page, int category)
        {
            int amountWebsites = GetAmountWebsitesToShow();

            return GetQueryWebsites().Where(x => x.CategoryID == category).Skip((page - 1) * amountWebsites)
                .Take(amountWebsites).ToList().Select(w => new WebsiteModel(w));
        }

        public IEnumerable<WebsiteModel> GetWebsites(int page, string search)
        {
            int amountWebsites = GetAmountWebsitesToShow();

            return GetMatchedWebsites(GetQueryWebsites(), search).Skip((page - 1) * amountWebsites)
                .Take(amountWebsites).ToList().Select(w => new WebsiteModel(w));;
        }

        private IEnumerable<Database.Entities.Websites> GetMatchedWebsites(IEnumerable<Database.Entities.Websites> websitesToSearchIn, string searchText)
        {
            return (from website in websitesToSearchIn
                let searchWords = searchText.Split()
                where searchWords.Any(sw => website.Title.ToLower().Contains(sw.ToLower()) || (website.Keywords != null && website.Keywords.Contains(sw.ToLower())) || website.Description.ToLower().Contains(sw.ToLower()))
                select website);
        }

        private IQueryable<Database.Entities.Websites> GetQueryWebsites()
        {
            return (from website in _unitOfWorkRepository.WebsitesRepository.GetAll()
                   let amountVotes = website.WebsiteIn.Count(x => x.Unique)
                   let ratings = website.WebsiteRating.Select(x => x.Rating)
                   let avgRate = ratings.Any() ? (int)ratings.Average() : 5
                   let amountRedirects = website.WebsiteOut.Count(x => x.Unique)
                   where
                       website.Enabled && website.Users.IsAdminVerified && website.Users.IsEmailVerified &&
                       (website.Users.BannedEndDate == null || DateTime.Now > website.Users.BannedEndDate)
                   orderby website.Sponsored descending,
                       amountVotes descending,
                       amountRedirects descending,
                       avgRate descending,
                       website.DateAdded descending
                   select website);

        }

        private int GetAmountWebsitesToShow()
        {
            return _settingsService.GetAmountWebsitesToShow();
        }
    }
}
