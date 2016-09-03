using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class LoginTokensRepository
    {
        private readonly DbSet<LoginTokens> _query;

        public LoginTokensRepository(DbSet<LoginTokens> dbSet)
        {
            _query = dbSet;
        }

        public void Add(LoginTokens userExtraInfo)
        {
            _query.Add(userExtraInfo);
        }

        public bool IsUniqueSelector(string selector)
        {
            return !_query.Any(x => x.Selector == selector);
        }

        public LoginTokens Get(string selector, DateTime dateTime)
        {
            return _query.Include(x => x.Users).FirstOrDefault(x => x.Selector == selector && x.Expires > dateTime);
        }

        public void RemoveToken(LoginTokens loginToken)
        {
            _query.Remove(loginToken);
        }

        public void RemoveLogin(long userId, string selector)
        {
            var loginToken = _query.FirstOrDefault(l => l.UserId == userId && l.Selector == selector);

            if (loginToken != null)
            {
                _query.Remove(loginToken);
            }
        }
    }
}
