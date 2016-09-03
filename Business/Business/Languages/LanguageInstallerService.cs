using System.Globalization;
using System.IO;
using Core.Business.File;

namespace Core.Business.Languages
{
    public class LanguageInstallerService
    {
        private readonly Stream _languageDllFile;
        private readonly string _languageFileName;
        private readonly string _cultureName;
        private readonly string _cultureAbbreviation;
        private readonly string _cultureIso;

        /// <summary>
        /// Expects a dll file
        /// </summary>
        /// <param name="languageDllFile"></param>
        /// <param name="languageFileName"></param>
        public LanguageInstallerService(Stream languageDllFile, string languageFileName)
        {
            _languageDllFile = languageDllFile;
            _languageFileName = languageFileName;

            var culture = GetCulture();

            _cultureAbbreviation = GetCultureAbbreviation(culture);
            _cultureName = GetCultureName(culture);
            _cultureIso = GetCultureIso(culture);
        }

        /// <summary>
        /// Get the abbreviation of the language
        /// </summary>
        /// <returns></returns>
        public string GetAbbreviation()
        {
            return _cultureAbbreviation;
        }

        /// <summary>
        /// Get the filepath of the
        /// </summary>
        /// <returns>A path to the language dll, the path may not exist yet depending on whether the method install has been called</returns>
        public string GetRelativeDllFilePath()
        {
            return string.Format("{0}/{1}/Localization.resources.dll", FileService.GetBinPath(), _cultureIso);
        }

        /// <summary>
        /// Install the language
        /// Will restart the website
        /// </summary>
        public void Install()
        {
            AddToDatabase();

            AddDll();
        }

        private CultureInfo GetCulture()
        {
            return new CultureInfo(Path.GetFileNameWithoutExtension(_languageFileName));
        }

        private string GetCultureAbbreviation(CultureInfo culture)
        {
            return culture.TwoLetterISOLanguageName;
        }

        private string GetCultureIso(CultureInfo culture)
        {
            return culture.TwoLetterISOLanguageName;
        }

        private string GetCultureName(CultureInfo culture)
        {
            return culture.DisplayName;
        }

        private void AddToDatabase()
        {
            var languageService = new LanguageService();

            languageService.AddLanguage(_cultureAbbreviation, _cultureIso, _cultureName);
        }

        private void AddDll()
        {
            FileService.SaveFile(_languageDllFile, string.Format("{0}/{1}", FileService.GetBinPath(), _cultureIso));
        }
    }
}
