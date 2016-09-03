using System.ComponentModel.DataAnnotations;
using Localization.Languages.Models.Shared;

namespace Topsite.Models
{
    public class SearchModel
    {
        public SearchModel(string searchText, string baseUrl)
        {
            Text = searchText;
            BaseUrl = baseUrl;
        }

        public SearchModel() { }

        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public string Text { get; set; }

        public readonly string BaseUrl;
    }
}