using System;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class AccountRepository : IRepository<Users>
    {
        private readonly DbSet<Users> _query;

        public AccountRepository(DbSet<Users> settings)
        {
            _query = settings;
        }

        public Users GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public Users GetByUsername(string username)
        {
            return _query.FirstOrDefault(
                x =>
                    x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
        }

        public Users GetByEmail(string email)
        {
            return _query.FirstOrDefault(
                x =>
                    x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase));
        }

        public int GetAmountUsers()
        {
            return GetAll(includeBanned: false).Count();
        }

        public int GetAmountUsers(int year, int month)
        {
            return GetAll(includeBanned: false).Count(x => x.RegistrationDate.Year == year && x.RegistrationDate.Month == month);
        }

        public int GetAmountActiveUsers()
        {
            var dateMinusSixMonth = DateTime.Now.AddMonths(-6);

            return _query.Count(x =>
                (x.BannedEndDate == null || DateTime.Now > x.BannedEndDate)
                && x.LastLoginDate >= dateMinusSixMonth);
        }

        public Users GetByEmailVerificationCodeAndUserId(int userId, string code)
        {
            return _query.FirstOrDefault(x => x.Id == userId && x.EmailVerificationCode == code);
        }

        public Users GetByNewEmailVerificationCodeAndUserId(int userId, string code)
        {
            return _query.FirstOrDefault(x => x.Id == userId && x.NewEmailVerificationCode == code);
        }

        /// <summary>
        /// Get all accounts excluding banned users
        /// </summary>
        /// <returns></returns>
        public IQueryable<Users> GetAll()
        {
            return GetAll(includeBanned: false);
        }

        public IQueryable<Users> GetUnverifiedUsers(int amountUsers)
        {
            return GetAll()
                .Where(x => !x.IsAdminVerified)
                .Take(amountUsers)
                .OrderByDescending(x => x.RegistrationDate)
                .Include(x => x.Websites);
        }

        public IQueryable<Users> GetAll(bool includeBanned)
        {
            return includeBanned ? _query : _query.Where(x => x.BannedEndDate == null || DateTime.Now > x.BannedEndDate);
        }

        public bool IsUniqueUsername(string username)
        {
            return !_query.Any(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));

        }

        public bool IsUniqueEmail(string email, int? excludeUserId = null)
        {
            return !_query.Any(x => (x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase)) && (excludeUserId == null || x.Id != excludeUserId));
        }

        public Users AddAccount(string username, string password, string email, int securityQuestion, string securityAnswer,  bool isEmailVerifiedRequired, bool isAdmin, bool isAdminVerified, string emailVerificationCode = null)
        {
            var currentDate = DateTime.Now;

            var user = new Users
            {
                Email = email,
                Answer = securityAnswer,
                Question = securityQuestion,
                IsEmailVerified = !isEmailVerifiedRequired,
                RegistrationDate = currentDate,
                LastLoginDate = currentDate,
                Password = password,
                Username = username,
                EmailVerificationCode = emailVerificationCode,
                IsAdmin = isAdmin,
                IsAdminVerified = isAdminVerified
            };

            _query.Add(user);

            return user;
        }

        public bool IsUniqueEmailVerificationCode(string code)
        {
            return !_query.Any(x => x.EmailVerificationCode.Equals(code));
        }

        public bool IsUniqueNewEmailVerificationCode(string code)
        {
            return !_query.Any(x => x.NewEmailVerificationCode.Equals(code));
        }

        public void DeleteAccount(int userId)
        {
            _query.Remove(_query.First(x => x.Id == userId));
        }

        public bool IsAdmin(int userId)
        {
            var user = GetById(userId);

            return user != null && user.IsAdmin;
        }

        public bool IsAdmin(string username)
        {
            var user = GetByUsername(username);

            return user != null && user.IsAdmin;
        }

        public int GetAmountUnverifiedUsers()
        {
            return GetAll().Count(x => !x.IsAdminVerified);
        }
    }
}
