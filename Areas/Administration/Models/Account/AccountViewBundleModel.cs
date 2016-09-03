using System.Collections.Generic;
using Core.Models.Account;
using Core.Models.Websites;

namespace Topsite.Areas.Administration.Models.Account
{
    public class AccountViewBundleModel
    {
        public AccountModel Account { get; set; }
        public IEnumerable<WebsiteModel> Websites { get; set; }
    }
}