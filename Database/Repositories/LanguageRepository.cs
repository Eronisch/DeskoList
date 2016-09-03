using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class LanguageRepository : IRepository<Languages>
    {
        private readonly DbSet<Languages> _query;

        public LanguageRepository(DbSet<Languages> settings)
        {
            _query = settings;
        }

        public Languages GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<Languages> GetAll()
        {
            return _query.OrderBy(l => l.Name);
        }

        public void Add(Languages language)
        {
            _query.Add(language);
        }

        public void Remove(Languages language)
        {
            _query.Remove(language);
        }

        public bool Exists(string abbreviation)
        {
            return _query.Any(l => l.Abbreviation == abbreviation);
        }
    }
}
