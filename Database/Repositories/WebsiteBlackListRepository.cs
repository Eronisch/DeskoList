using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class WebsiteBlackListRepository : IRepository<WebsiteBlackList>
    {
        private readonly DbSet<WebsiteBlackList> _query;

        public WebsiteBlackListRepository(DbSet<WebsiteBlackList> settings)
        {
            _query = settings;
        }

        public WebsiteBlackList GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<WebsiteBlackList> GetAll()
        {
            return _query;
        }

        public void AddWebsite(WebsiteBlackList websiteBlackList)
        {
            _query.Add(websiteBlackList);
        }

        public void RemoveWebsite(WebsiteBlackList websiteBlackList)
        {
            _query.Remove(websiteBlackList);
        }
    }
}
