using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class LoginFailsRepository : IRepository<LoginFails>
    {
        private readonly DbSet<LoginFails> _query;

        public LoginFailsRepository(DbSet<LoginFails> settings)
        {
            _query = settings;
        }

        public LoginFails GetById(int id)
        {
            throw new NotImplementedException();
        }

        public LoginFails GetById(long id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<LoginFails> GetAll()
        {
            return _query;
        }

        public LoginFails GetByIp(string ip)
        {
            return _query.FirstOrDefault(x => x.Ip == ip);
        }

        public void AddOrUpdate(LoginFails loginFail)
        {
            _query.AddOrUpdate(loginFail);
        }

        public void Remove(long loginFailId)
        {
            _query.Remove(GetById(loginFailId));
        }
    }
}
