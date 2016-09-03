using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class NewsRepository : IRepository<News>
    {
        private readonly DbSet<News> _query;

        public NewsRepository(DbSet<News> settings)
        {
            _query = settings;
        }

        public News GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<News> GetAll()
        {
            return _query.Include(a => a.Users).OrderByDescending(x => x.Date);
        }

        public void RemoveArticle(News article)
        {
            _query.Remove(article);
        }

        public void AddArticle(News article)
        {
            _query.Add(article);
        }

        public IEnumerable<News> GetAll(int amount)
        {
            return GetAll().Take(amount).ToList();
        }
    }
}
