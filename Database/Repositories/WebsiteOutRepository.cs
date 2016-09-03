using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class WebsiteOutRepository : IRepository<WebsiteOut>
    {
        private readonly DbSet<WebsiteOut> _query;

        public WebsiteOutRepository(DbSet<WebsiteOut> settings)
        {
            _query = settings;
        }

        public WebsiteOut GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<WebsiteOut> GetAll()
        {
            return _query;
        }

        public void CleanWebsiteOut()
        {
            _query.ToList().ForEach(x =>
            {
                _query.Remove(x);
            });
        }

        public void AddRedirect(WebsiteOut redirect)
        {
            _query.Add(redirect);
        }

        public bool IsUniqueRedirectToday(int websiteId, string ip, DateTime dateRedirect)
        {
            return !_query.Any(x => x.WebsiteID == websiteId
                && x.Date.Year == dateRedirect.Year
                && x.Date.Month == dateRedirect.Month
                && x.Date.Day == dateRedirect.Day
                && x.IP == ip);
        }

        public bool IsUniqueRedirect(int websiteId, string ip)
        {
            return !_query.Any(x => x.WebsiteID == websiteId && x.IP == ip);
        }
    }
}
