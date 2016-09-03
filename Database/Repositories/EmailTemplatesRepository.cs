using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class EmailTemplatesRepository : IRepository<EmailTemplates>
    {
        private readonly DbSet<EmailTemplates> _query;

        public EmailTemplatesRepository(DbSet<EmailTemplates> settings)
        {
            _query = settings;
        }

        public EmailTemplates GetById(int id)
        {
            return _query.First(x => x.Id == id);
        }

        public IQueryable<EmailTemplates> GetAll()
        {
            return _query;
        }

        public void AddEmailTemplate(EmailTemplates emailTemplate)
        {
            _query.Add(emailTemplate);
        }
    }
}
