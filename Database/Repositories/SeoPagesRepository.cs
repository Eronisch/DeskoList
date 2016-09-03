using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class SeoPagesRepository : IRepository<SeoPages>
    {
        private readonly DbSet<SeoPages> _query;

        public SeoPagesRepository(DbSet<SeoPages> settings)
        {
            _query = settings;
        }

        public SeoPages GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public SeoPages GetByControllerAndAction(string controller, string action)
        {
            return _query.FirstOrDefault(x => x.PageController.Equals(controller, StringComparison.CurrentCultureIgnoreCase)
                && x.PageIndex.Equals(action, StringComparison.CurrentCultureIgnoreCase));
        }

        public IQueryable<SeoPages> GetAll()
        {
            return _query;
        }

        public void AddSeo(SeoPages seoPage)
        {
            _query.Add(seoPage);
        }
    }
}
