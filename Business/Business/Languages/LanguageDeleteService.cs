using System.IO;
using Core.Business.File;

namespace Core.Business.Languages
{
    public class LanguageDeleteService
    {
        private readonly LanguageService _languageService;

        public LanguageDeleteService()
        {
            _languageService = new LanguageService();   
        }

        public void Delete(int languageId)
        {
            string abbreviation = GetAbbreviation(languageId);

            RemoveDbRecord(languageId);
            RemoveResourceFile(abbreviation);
        }

        private string GetAbbreviation(int languageId)
        {
            return _languageService.Get(languageId).Abbreviation;
        }

        private void RemoveResourceFile(string abbreviation)
        {
            FileService.RemoveDirectory(Path.Combine(FileService.GetBaseDirectory(), "bin", abbreviation), true);
        }

        private void RemoveDbRecord(int languageId)
        {
            var languageService = new LanguageService();
            languageService.RemoveLanguage(languageId);
        }
    }
}
