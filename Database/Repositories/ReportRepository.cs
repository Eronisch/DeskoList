using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class ReportRepository : IRepository<Reports>
    {
        private readonly DbSet<Reports> _query;

        public ReportRepository(DbSet<Reports> settings)
        {
            _query = settings;
        }

        public Reports GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<Reports> GetAll()
        {
            return _query.Include(x => x.Websites);
        }

        public void AddReport(Reports report)
        {
            _query.Add(report);
        }

        public void RemoveReport(Reports report)
        {
            _query.Remove(report);
        }

        public IQueryable<Reports> GetSpecificAmountOfReports(int amountReports)
        {
            return GetAll().Take(amountReports).OrderByDescending(x => x.Date);
        }
    }
}
