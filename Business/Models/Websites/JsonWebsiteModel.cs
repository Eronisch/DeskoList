using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.Models.Websites
{
    public class JsonWebsiteBundleModel
    {
        public IEnumerable<JsonWebsiteModel> Websites { get; set; }
        public int AmountPages { get; set; }

        public JsonWebsiteBundleModel()
        {
            Websites = new Collection<JsonWebsiteModel>();
        }
    }

    public class JsonWebsiteModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Date { get; set; }
        public string Edit { get; set; }
        public string Delete { get; set; }
    }
}
