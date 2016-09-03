using System.Collections.Generic;
using Business.Models.Websites;

namespace Topsite.Models
{
    public class CodeModel
    {
        public IEnumerable<WebsiteModel> Websites { get; set; }
        public string TopsiteTitle { get; set; }
    }
}