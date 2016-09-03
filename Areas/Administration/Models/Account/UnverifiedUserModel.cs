namespace Topsite.Areas.Administration.Models.Account
{
    public class UnverifiedUserModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string WebsiteTitle { get; set; }
        public string TimeAgo{ get; set; }
    }
}