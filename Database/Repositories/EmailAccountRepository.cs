using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class EmailAccountRepository : IRepository<EmailAccounts>
    {
        private readonly DbSet<EmailAccounts> _query;

        public EmailAccountRepository(DbSet<EmailAccounts> settings)
        {
            _query = settings;
        }

        public EmailAccounts GetInfoAccount()
        {
            return _query.First(x => x.Id == 1);
        }

        public EmailAccounts GetNoReply()
        {
            return _query.First(x => x.Id == 2);
        }

        public EmailAccounts GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<EmailAccounts> GetAll()
        {
            return _query;
        }

        public void AddEmailAccount(EmailAccounts emailAccount)
        {
            _query.Add(emailAccount);
        }
    }
}
