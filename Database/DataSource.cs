using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;

namespace Database
{
    public abstract class DataSource : IDisposable
    {
        private readonly DataContent _database;
        private bool _disposed;

        protected DataSource()
        {
            _database = new DataContent();
        }

        protected DataContent Query
        {
            get { return _database; }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _database.Dispose();
                }

                _disposed = true;
            }
        }

        ~DataSource()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void UpdateEntity<T>(T entity) where T : class
        {
             _database.Set<T>().AddOrUpdate(entity);
        }

        public void SaveChanges()
        {
            try
            {
                _database.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ObjectContext context = ((IObjectContextAdapter)_database).ObjectContext;

                context.Refresh(RefreshMode.StoreWins, ex.Entries.Where(e => e.State != EntityState.Deleted));
            }
        }

        public async Task SaveChangesAsync()
        {
           await _database.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes all the records in the table
        /// </summary>
        /// <param name="table"></param>
        public void Truncate(string table)
        {
            _database.Database.ExecuteSqlCommand(string.Format("TRUNCATE TABLE {0}", table));
        }
    }
}
