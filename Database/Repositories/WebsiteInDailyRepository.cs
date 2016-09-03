using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class WebsiteInDailyRepository : IRepository<WebsiteInDaily>
    {
        private readonly DbSet<WebsiteInDaily> _query;

        public WebsiteInDailyRepository(DbSet<WebsiteInDaily> settings)
        {
            _query = settings;
        }

        public WebsiteInDaily GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<WebsiteInDaily> GetAll()
        {
            return _query;
        }

        public void Add(WebsiteInDaily websiteInDaily)
        {
            _query.Add(websiteInDaily);
        }

        public WebsiteInDaily GetByDate(DateTime dateTime)
        {
            return _query.FirstOrDefault(x => x.Date.Year == dateTime.Year
                                       && x.Date.Month == dateTime.Month
                                       && x.Date.Day == dateTime.Day);
        }

        public void CleanWebsiteInDaily()
        {
            var removeDate = DateTime.Now.AddMonths(-7);

            _query.Where(x => x.Date.Year <= removeDate.Year && x.Date.Month <= removeDate.Month)
                .ToList()
                .ForEach(x => _query.Remove(x));
        }

        public int GetUniqueWebsiteIn(DateTime date)
        {
            var record = _query.FirstOrDefault(x => x.Date.Year == date.Year && x.Date.Month == date.Month && x.Date.Day == date.Day);

            if (record != null)
            {
                return record.UniqueIn;
            }

            return 0;
        }
    }
}
