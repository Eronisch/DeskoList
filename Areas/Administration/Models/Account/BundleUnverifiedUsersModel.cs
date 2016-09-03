using System.Collections.Generic;

namespace Topsite.Areas.Administration.Models.Account
{
    public class BundleUnverifiedUsersModel
    {
        public IEnumerable<UnverifiedUserModel> UnverifiedUsers { get; set; }
        public int AmountUnverifiedUsers { get; set; }
    }
}