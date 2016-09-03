using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class PluginOpenUpdatesRepository : IRepository<PluginOpenUpdates>
    {
        private readonly DbSet<PluginOpenUpdates> _query;

        public PluginOpenUpdatesRepository(DbSet<PluginOpenUpdates> dbSet)
        {
            _query = dbSet;
        }

        public PluginOpenUpdates GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public PluginOpenUpdates GetByPluginIdAndVersion(int pluginId, string version)
        {
            return _query.FirstOrDefault(x => x.PluginId == pluginId && x.Version.Equals(version, StringComparison.CurrentCultureIgnoreCase));
        }

        public IQueryable<PluginOpenUpdates> GetAll()
        {
            return _query;
        }

        public IQueryable<PluginOpenUpdates> GetDownloadedUpdates()
        {
            return _query.Where(u => u.IsDownloaded);
        }

        public void Add(PluginOpenUpdates openUpdate)
        {
            _query.Add(openUpdate);
        }

        public void Remove(PluginOpenUpdates openUpdate)
        {
            _query.Remove(openUpdate);
        }

        public bool IsVersionAdded(string version)
        {
            return _query.Any(x => x.Version == version);
        }

        public IQueryable<PluginOpenUpdates> GetNotDownloadedUpdates()
        {
            return _query.Where(x => !x.IsDownloaded);
        }
    }
}
