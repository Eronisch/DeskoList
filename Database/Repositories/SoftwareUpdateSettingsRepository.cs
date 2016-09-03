using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class SoftwareUpdateSettingsRepository : IRepository<SoftwareUpdateSettings>
    {
        private readonly DbSet<SoftwareUpdateSettings> _query;

        public SoftwareUpdateSettingsRepository(DbSet<SoftwareUpdateSettings> settings)
        {
            _query = settings;
        }

        public SoftwareUpdateSettings GetSettings()
        {
            return _query.Find(1);
        }

        public SoftwareUpdateSettings GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<SoftwareUpdateSettings> GetAll()
        {
            throw new NotImplementedException();
        }

        public void InsertSettings(SoftwareUpdateSettings settings)
        {
            _query.Add(settings);
        }
    }
}
