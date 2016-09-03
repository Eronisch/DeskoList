using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class OpenUpdatesRepository : IRepository<OpenUpdates>
    {
        private readonly DbSet<OpenUpdates> _query;

        public OpenUpdatesRepository(DbSet<OpenUpdates> dbSet)
        {
            _query = dbSet;
        }

        public OpenUpdates GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<OpenUpdates> GetDownloadedUpdates()
        {
            return _query.Where(u => u.IsDownloaded);
        }

        public IQueryable<OpenUpdates> GetAll()
        {
            return _query;
        }

        public void Add(OpenUpdates openUpdate)
        {
            _query.Add(openUpdate);
        }

        public OpenUpdates GetByVersion(string version)
        {
            return _query.FirstOrDefault(x => x.Version == version);
        }

        public void Remove(OpenUpdates openUpdate)
        {
            _query.Remove(openUpdate);
        }

        public bool IsVersionAdded(string version)
        {
            return _query.Any(x => x.Version == version);
        }

        public IQueryable<OpenUpdates> GetNotDownloadedUpdates()
        {
            return _query.Where(x => !x.IsDownloaded);
        }
    }
}
