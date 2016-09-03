using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class WebsiteInRepository : IRepository<WebsiteIn>
    {
        private readonly DbSet<WebsiteIn> _query;

        public WebsiteInRepository(DbSet<WebsiteIn> settings)
        {
            _query = settings;
        }

        public WebsiteIn GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<WebsiteIn> GetAll()
        {
            return _query;
        }

        public void AddVote(WebsiteIn vote)
        {
            _query.Add(vote);
        }

        public bool IsUniqueVote(int websiteId, string ip)
        {
            return !_query.Any(x => x.WebsiteID == websiteId && x.IP == ip);
        }

        public bool IsUniqueVoteToday(int websiteId, string ip, DateTime dateVote)
        {
            return !_query.Any(x => x.WebsiteID == websiteId
                && x.Date.Year == dateVote.Year
                && x.Date.Month == dateVote.Month
                && x.Date.Day == dateVote.Day
                && x.IP == ip);
        }

        public void CleanWebsiteIn()
        {
            _query.ToList().ForEach(x =>
            {
                _query.Remove(x);
            });
        }
    }
}
