using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class CategoryRepository : IRepository<Categories>
    {
        private readonly DbSet<Categories> _query;

        public CategoryRepository(DbSet<Categories> settings)
        {
            _query = settings;
        }

        public Categories GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<Categories> GetAll()
        {
            return _query.OrderBy(x=> x.Name);
        }

        public void AddCategory(Categories category)
        {
            _query.Add(category);
        }
    }
}
