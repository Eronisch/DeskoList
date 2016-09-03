using System;
using System.Data.Entity;
using Database.Entities;

namespace Database
{
    public abstract class DataRepository<T> : IDisposable where T : class
    {
        private readonly deskoTopsiteEntities _database;
        private bool _disposed;

        protected DataRepository()
        {
            _database = new deskoTopsiteEntities();
        }

        private DbSet<T> _Query { get; set; }

        protected DbSet<T> Query
        {
            get { return _Query; }
            set
            {
                _Query = _database.Set<T>();
            }
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

        ~DataRepository()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Add(T entity)
        {
            Query.Add(entity);
        }

        public void Remove(T entity)
        {
            Query.Remove(entity);
        }

        public void SaveChanges()
        {
            _database.SaveChanges();
        }
    }
}
