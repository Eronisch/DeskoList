using System;
using System.Linq;
using Core.Business.Websites;
using Database;
using Database.Entities;

namespace Core.Business.Rating
{
    public class RatingService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();
        private readonly  WebsiteService _websiteService = new WebsiteService();
        private const int DefaultRating = 5;

        public void AddRating(int websiteId, int rate, string ip)
        {
            if (!HasRated(ip, websiteId))
            {
                _unitOfWorkRepository.RatingRepository.AddRating(new WebsiteRating
                {
                    Ip = ip,
                    WebsiteID = websiteId,
                    Rating = rate
                });

                _unitOfWorkRepository.SaveChanges();
            }
        }

        public double GetAverageRatingFromWebsite(int websiteId)
        {
            var website = _websiteService.GetWebsite(websiteId);

            if (website.WebsiteRating.Any())
            {
                return Math.Round(website.WebsiteRating.Average(x => x.Rating), 1);
            }

            return DefaultRating;
        }

        public int GetAverageRatingFromWebsite(Database.Entities.Websites website)
        {
            if (website.WebsiteRating.Any())
            {
                return (int)Math.Round(website.WebsiteRating.Average(x => x.Rating), 0);
            }

            return DefaultRating;
        }

        public bool HasRated(string ip, int websiteId)
        {
            return _unitOfWorkRepository.RatingRepository.HasRated(websiteId, ip);
        }
    }
}
