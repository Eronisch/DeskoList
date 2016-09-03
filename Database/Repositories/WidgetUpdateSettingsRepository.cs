using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class WidgetUpdateSettingsRepository : IRepository<WidgetUpdateSettings>
    {
        private readonly DbSet<WidgetUpdateSettings> _query;

        public WidgetUpdateSettingsRepository(DbSet<WidgetUpdateSettings> settings)
        {
            _query = settings;
        }

        public WidgetUpdateSettings GetSettings()
        {
            return _query.Find(1);
        }

        public WidgetUpdateSettings GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<WidgetUpdateSettings> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Insert(WidgetUpdateSettings settings)
        {
            _query.Add(settings);
        }
    }
}
