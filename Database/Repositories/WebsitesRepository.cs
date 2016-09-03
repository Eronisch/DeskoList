using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class WebsiteRepository : IRepository<Websites>
    {
        private readonly DbSet<Websites> _query;

        public WebsiteRepository(DbSet<Websites> settings)
        {
            _query = settings;
        }

        public Websites GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public Websites GetById(int id, bool eagerLoadRedirects, bool eagerLoadVotes, bool eagerLoadRatings, bool eagerLoadUsers)
        {
            var myQuery = _query.Where(x => x.Id == id);

            if (eagerLoadRedirects)
            {
                myQuery = myQuery.Include(x => x.WebsiteOut);
            }

            if (eagerLoadVotes)
            {
                myQuery = myQuery.Include(x => x.WebsiteIn);
            }

            if (eagerLoadRatings)
            {
                myQuery = myQuery.Include(x => x.WebsiteRating);
            }

            if (eagerLoadUsers)
            {
                myQuery = myQuery.Include(x => x.Users);
            }

            return myQuery.FirstOrDefault();
        }

        public void AddWebsite(Websites website)
        {
            _query.Add(website);
        }

        public IEnumerable<Websites> GetWebsitesFromUser(int userId)
        {
            return GetAll().Where(x => x.UserID == userId).OrderBy(x => x.Title);
        }

        public void RemoveWebsite(Websites website)
        {
            _query.Remove(website);
        }

        public IQueryable<Websites> GetAll(bool includeBanned)
        {
            var query = GetAll();

            if (includeBanned)
            {
                query = query.Where(x => x.Enabled && (x.Users.BannedEndDate == null || DateTime.Now > x.Users.BannedEndDate));
            }

            return query.OrderByDescending(x=> x.Id);
        }

        public int GetAmountWebsites()
        {
            return GetDefaultAmount().Count();
        }

        public int GetAmountWebsites(int year, int month)
        {
            return GetDefaultAmount().Count(x => x.DateAdded.Year == year && x.DateAdded.Month == month);
        }

        private IQueryable<Websites> GetDefaultAmount()
        {
            return GetAll(includeBanned: false);
        }

        public IQueryable<Websites> GetPage(bool includeUsers, bool includeBanned, int page, int amount)
        {
            return GetAll(includeBanned).OrderByDescending(x=> x.Id).Skip((page - 1)*amount).Take(amount);
        }

        public IQueryable<Websites> GetAll()
        {
            return _query.Include(x => x.Users);
        }
    }
}
