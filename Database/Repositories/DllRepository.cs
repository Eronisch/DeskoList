using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class DllRepository : IRepository<Dlls>
    {
        private readonly DbSet<Dlls> _query;

        public DllRepository(DbSet<Dlls> settings)
        {
            _query = settings;
        }

        public Dlls GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<Dlls> GetAll()
        {
            return _query;
        }

        public Dlls GetDllByName(string dllName)
        {
            return _query.FirstOrDefault(d => d.Name == dllName);
        }

        public void Remove(Dlls dll)
        {
            _query.Remove(dll);
        }

        public void Add(Dlls dll)
        {
            _query.Add(dll);
        }
    }
}
