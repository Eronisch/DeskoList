using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class ElmahRepository : IRepository<ELMAH_Error>
    {
        private readonly DbSet<ELMAH_Error> _dbSet;

        public ElmahRepository(DbSet<ELMAH_Error> dbSet)
        {
            _dbSet = dbSet;
        }

        public ELMAH_Error GetById(Guid id)
        {
            return _dbSet.FirstOrDefault(x => x.ErrorId == id);
        }

        public ELMAH_Error GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ELMAH_Error> GetAll()
        {
            return _dbSet.OrderByDescending(x => x.TimeUtc);
        }
    }
}
