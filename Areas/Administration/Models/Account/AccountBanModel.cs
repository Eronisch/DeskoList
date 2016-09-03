using System;

namespace Topsite.Areas.Administration.Models.Account
{
    public class AccountBanModel
    {
        public AccountBanModel() { }

        public AccountBanModel(int accountId)
        {
            UserId = accountId;
        }

        public int UserId { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPermanent { get; set; }
    }
}