using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class ActiveDllRepository : IRepository<ActiveDlls>
    {
        private readonly DbSet<ActiveDlls> _query;

        public ActiveDllRepository(DbSet<ActiveDlls> settings)
        {
            _query = settings;
        }

        public ActiveDlls GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<ActiveDlls> GetAll()
        {
            return _query;
        }

        public IQueryable<ActiveDlls> GetAll(bool includeDlls)
        {
            return includeDlls ? _query.Include(w => w.Widgets) : GetAll();
        }

        public IQueryable<ActiveDlls> GetActiveDllsByName(string dllName)
        {
            return _query.Where(a => a.Dlls.Name == dllName);
        }

        public void Add(ActiveDlls activeDlls)
        {
            _query.Add(activeDlls);
        }
    }
}
