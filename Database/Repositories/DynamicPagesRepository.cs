using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class DynamicPagesRepository : IRepository<DynamicPages>
    {
        private readonly DbSet<DynamicPages> _query;

        public DynamicPagesRepository(DbSet<DynamicPages> settings)
        {
            _query = settings;
        }

        public DynamicPages GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }
        public IQueryable<DynamicPages> GetAll()
        {
            return _query.OrderBy(x=> x.Title);
        }

        public void Add(DynamicPages dynamicPage)
        {
            _query.Add(dynamicPage);
        }

        public void Remove(DynamicPages dynamicPage)
        {
            _query.Remove(dynamicPage);
        }
    }
}
