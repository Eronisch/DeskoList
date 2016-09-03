using System;

namespace Core.Models.Account
{
    public class AccountModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? BannedStartDate { get; set; }
        public DateTime? BannedEndDate { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public int SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public bool IsAdminVerified { get; set; }
    }
}
