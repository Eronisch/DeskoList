using System.Web.Mvc;

namespace Topsite.Models
{
    public class VoteModel
    {
        public int Id { get; set; }

        [AllowHtml]
        public string Redirect { get; set; }

        public string Answer { get; set; }

        public string WebsiteTitle { get; set; }
    }
}