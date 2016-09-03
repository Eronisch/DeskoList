using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class ThemeOpenUpdatesRepository : IRepository<ThemeOpenUpdates>
    {
        private readonly DbSet<ThemeOpenUpdates> _query;

        public ThemeOpenUpdatesRepository(DbSet<ThemeOpenUpdates> dbSet)
        {
            _query = dbSet;
        }

        public ThemeOpenUpdates GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<ThemeOpenUpdates> GetDownloadedUpdates()
        {
            return _query.Where(u => u.IsDownloaded);
        }

        public ThemeOpenUpdates GetByThemeIdAndVersion(int themeId, string version)
        {
            return _query.FirstOrDefault(x => x.ThemeId == themeId && x.Version.Equals(version, StringComparison.CurrentCultureIgnoreCase));
        }

        public IQueryable<ThemeOpenUpdates> GetAll()
        {
            return _query;
        }

        public void Add(ThemeOpenUpdates openUpdate)
        {
            _query.Add(openUpdate);
        }

        public void Remove(ThemeOpenUpdates openUpdate)
        {
            _query.Remove(openUpdate);
        }

        public bool IsVersionAdded(string version)
        {
            return _query.Any(x => x.Version == version);
        }

        public IQueryable<ThemeOpenUpdates> GetNotDownloadedUpdates()
        {
            return _query.Where(x => !x.IsDownloaded);
        }
    }
}
