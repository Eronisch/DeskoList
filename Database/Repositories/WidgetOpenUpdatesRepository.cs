using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class WidgetOpenUpdatesRepository : IRepository<WidgetOpenUpdates>
    {
        private readonly DbSet<WidgetOpenUpdates> _query;

        public WidgetOpenUpdatesRepository(DbSet<WidgetOpenUpdates> dbSet)
        {
            _query = dbSet;
        }

        public WidgetOpenUpdates GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public WidgetOpenUpdates GetByWidgetIdAndVersion(int widgetId, string version)
        {
            return _query.FirstOrDefault(x => x.WidgetId == widgetId && x.Version.Equals(version, StringComparison.CurrentCultureIgnoreCase));
        }

        public IQueryable<WidgetOpenUpdates> GetAll()
        {
            return _query;
        }

        public IQueryable<WidgetOpenUpdates> GetDownloadedUpdates()
        {
            return _query.Where(u => u.IsDownloaded);
        }

        public void Add(WidgetOpenUpdates openUpdate)
        {
            _query.Add(openUpdate);
        }

        public void Remove(WidgetOpenUpdates openUpdate)
        {
            _query.Remove(openUpdate);
        }

        public bool IsVersionAdded(string version)
        {
            return _query.Any(x => x.Version == version);
        }

        public IQueryable<WidgetOpenUpdates> GetNotDownloadedUpdates()
        {
            return _query.Where(x => !x.IsDownloaded);
        }
    }
}
