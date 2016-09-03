using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class PluginUpdateSettingsRepository : IRepository<PluginUpdateSettings>
    {
        private readonly DbSet<PluginUpdateSettings> _query;

        public PluginUpdateSettingsRepository(DbSet<PluginUpdateSettings> settings)
        {
            _query = settings;
        }

        public PluginUpdateSettings GetSettings()
        {
            return _query.Find(1);
        }

        public PluginUpdateSettings GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<PluginUpdateSettings> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Insert(PluginUpdateSettings pluginUpdateSettings)
        {
            _query.Add(pluginUpdateSettings);
        }
    }
}
