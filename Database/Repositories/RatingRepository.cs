using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class RatingRepository : IRepository<WebsiteRating>
    {
        private readonly DbSet<WebsiteRating> _query;

        public RatingRepository(DbSet<WebsiteRating> settings)
        {
            _query = settings;
        }

        public WebsiteRating GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<WebsiteRating> GetAll()
        {
            return _query;
        }

        public void AddRating(WebsiteRating rate)
        {
            _query.Add(rate);
        }

        public IEnumerable<WebsiteRating> GetLatestRatings(int amount)
        {
           return _query.OrderByDescending(x => x.Id).Take(5).Include(x => x.Websites);
        }

        public bool HasRated(int websiteId, int userId)
        {
            return _query.Any(x => x.UserId == userId && x.WebsiteID == websiteId);
        }

        public bool HasRated(int websiteId, string ip)
        {
            return _query.Any(x => x.Ip == ip && x.WebsiteID == websiteId);
        }

        public void RemoveRatingsFromUserId(int userId)
        {
            _query.RemoveRange(_query.Where(r => r.UserId ==  userId));
        }
    }
}
