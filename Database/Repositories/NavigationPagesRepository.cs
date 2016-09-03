using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class NavigationPagesRepository : IRepository<NavigationPages>
    {
        private readonly DbSet<NavigationPages> _query;

        public NavigationPagesRepository(DbSet<NavigationPages> settings)
        {
            _query = settings;
        }

        public NavigationPages GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<NavigationPages> GetAll()
        {
            return _query;
        }

        public void AddNavigation(NavigationPages navigationPage)
        {
            _query.Add(navigationPage);
        }
    }
}
