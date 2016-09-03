using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class ThemeUpdateSettingsRepository : IRepository<ThemeUpdateSettings>
    {
        private readonly DbSet<ThemeUpdateSettings> _query;

        public ThemeUpdateSettingsRepository(DbSet<ThemeUpdateSettings> settings)
        {
            _query = settings;
        }

        public ThemeUpdateSettings GetSettings()
        {
            return _query.Find(1);
        }

        public ThemeUpdateSettings GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ThemeUpdateSettings> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Insert(ThemeUpdateSettings themeUpdateSettings)
        {
            _query.Add(themeUpdateSettings);
        }
    }
}
