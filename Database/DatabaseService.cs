using System;
using System.Data.Common;
using System.Data.Entity;

namespace Database
{
    public class DatabaseService : IDisposable
    {
        private readonly DataContent _database;
        private bool _disposed;

        public DatabaseService()
        {
            _database = new DataContent();
        }

        public void ExecuteQuery(string query, DbTransaction transaction = null)
        {
            if (transaction != null)
            {
                _database.Database.UseTransaction(transaction);    
            }
            
            _database.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, query);
        }

        public DbTransaction BeginTransaction()
        {
            _database.Database.Connection.Open();
            _database.Database.Initialize(true);
             return _database.Database.Connection.BeginTransaction();
        }

        /// <summary>
        /// Creates the database if it doesn't exist yet
        /// </summary>
        public bool CreateDatabase()
        {
            return _database.Database.CreateIfNotExists();
        }

        public void ResetIdentity(string table)
        {
            _database.Database.ExecuteSqlCommand(string.Format("DBCC CHECKIDENT('{0}', RESEED, 0)", table));
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

        ~DatabaseService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
