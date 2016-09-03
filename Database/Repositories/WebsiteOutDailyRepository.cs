using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class WebsiteOutDailyRepository : IRepository<WebsiteOutDaily>
    {
        private readonly DbSet<WebsiteOutDaily> _query;

        public WebsiteOutDailyRepository(DbSet<WebsiteOutDaily> settings)
        {
            _query = settings;
        }

        public WebsiteOutDaily GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<WebsiteOutDaily> GetAll()
        {
            return _query;
        }

        public void Add(WebsiteOutDaily websiteOutDaily)
        {
            _query.Add(websiteOutDaily);
        }

        public WebsiteOutDaily GetByDate(DateTime dateTime)
        {
            return _query.FirstOrDefault(x => x.Date.Year == dateTime.Year
                                       && x.Date.Month == dateTime.Month
                                       && x.Date.Day == dateTime.Day);
        }

        public void CleanWebsiteOutDaily()
        {
            var removeDate = DateTime.Now.AddMonths(-7);

            _query.Where(x => x.Date.Year <= removeDate.Year && x.Date.Month <= removeDate.Month).ToList().Clear();
        }

        public int GetUniqueWebsiteOut(DateTime date)
        {
            var record = _query.FirstOrDefault(x => x.Date.Year == date.Year && x.Date.Month == date.Month && x.Date.Day == date.Day);

            if (record != null)
            {
                return record.UniqueOut;
            }

            return 0;
        }
    }
}
