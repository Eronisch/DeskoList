using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Database.Entities;

namespace Topsite.Areas.Administration.Models.Language
{
    public class InstallLanguageModel
    {
        public InstallLanguageModel() { }

        public InstallLanguageModel(bool compare, IEnumerable<Languages> installedLanguages, string selectedLanguage)
        {
            Compare = compare;
            Languages = GetLanguages(installedLanguages);
            SelectedLanguage = selectedLanguage;
        }

        #region Read only data

        public readonly IEnumerable<InstalledLanguageFlagModel> Languages;
        public readonly bool Compare;
        public readonly string SelectedLanguage;

        private IEnumerable<InstalledLanguageFlagModel> GetLanguages(
            IEnumerable<Languages> installedLanguages)
        {
            return installedLanguages.Select(l => new InstalledLanguageFlagModel(l));
        }

        #endregion

        #region User input

        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.InstallLanguageModel), Name = "Language")]
        [Required(ErrorMessageResourceType = typeof(Localization.Languages.Admin.Models.InstallLanguageModel), ErrorMessageResourceName = "NoFileSelected")]
        [FileExtensions(Extensions = ".dll", ErrorMessageResourceType = typeof(Localization.Languages.Admin.Models.InstallLanguageModel), ErrorMessageResourceName = "InvalidFile", ErrorMessage = null)]
        public HttpPostedFileBase LanguageDll { get; set; }

        #endregion
    }
}