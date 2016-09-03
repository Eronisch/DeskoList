using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class LinksRepository :  IRepository<Links>
    {
        private readonly DbSet<Links> _dbSetLinks;

        public LinksRepository(DbSet<Links> dbSet)
        {
            _dbSetLinks = dbSet;
        }

        public Links GetById(int id)
        {
            return _dbSetLinks.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<Links> GetAll()
        {
            return _dbSetLinks;
        }

        public IQueryable<Links> GetAll(int amount)
        {
            return GetAll().Take(amount);
        }

        public void Remove(int id)
        {
            _dbSetLinks.Remove(_dbSetLinks.First(x => x.Id == id));
        }

        public void Add(string name, string link)
        {
            _dbSetLinks.Add(new Links
            {
                Name = name,
                Link = link
            });
        }
    }
}
