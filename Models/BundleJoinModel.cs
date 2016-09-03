using System.Collections.Generic;
using System.Web.Mvc;

namespace Topsite.Models
{
    public class BundleJoinModel
    {
        public JoinusModel JoinusModel { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public bool IsPingEnabled { get; set; }

        public BundleJoinModel()
        {
            JoinusModel = new JoinusModel();
        }
    }
}