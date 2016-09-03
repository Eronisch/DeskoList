using System;
using System.Collections.Generic;
using System.Linq;
using Core.Business.Settings;
using Database;

namespace Core.Business.Languages
{
    /// <summary>
    /// Manager for retrieving language information from the database
    /// </summary>
    public class LanguageService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;
        private const string DefaultLanguage = "en";


        public LanguageService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();
        }

        /// <summary>
        /// Removes the language from the database
        /// </summary>
        /// <param name="languageId"></param>
        public void RemoveLanguage(int languageId)
        {
            _unitOfWorkRepository.LanguageRepository.Remove(Get(languageId));
            _unitOfWorkRepository.SaveChanges();
        }

        /// <summary>
        /// Gets the language from the database with a specific language id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Database.Entities.Languages Get(int id)
        {
            return _unitOfWorkRepository.LanguageRepository.GetById(id);
        }

        /// <summary>
        /// Get the supported languages from the database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Database.Entities.Languages> GetInstalledLanguages()
        {
            return _unitOfWorkRepository.LanguageRepository.GetAll();
        }

        /// <summary>
        /// Checks if the plugin supports the selected language, if not it will return the default "en"
        /// </summary>
        /// <param name="scriptAvailableLanguages">Supported plugin languages</param>
        /// <returns></returns>
        public string GetDisplayLanguage(IEnumerable<string> scriptAvailableLanguages)
        {
            var settingsService = new SettingsService();

            string selectedLanguage = settingsService.GetActiveLanguageAbbreviation();

            if (IsLanguageSupported(selectedLanguage, scriptAvailableLanguages))
            {
                return selectedLanguage;
            }

            return DefaultLanguage;
        }

        /// <summary>
        /// Check if the language (culture) is supported
        /// </summary>
        /// <returns></returns>
        public bool IsLanguageSupported(string culture)
        {
            return GetInstalledLanguages()
                .Any(i => i.Culture.Equals(culture, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Adds the language if it doesn't exist yet
        /// </summary>
        /// <param name="abbreviation"></param>
        /// <param name="isoName"></param>
        /// <param name="displayName"></param>
        public void AddLanguage(string abbreviation, string isoName, string displayName)
        {
            if (!_unitOfWorkRepository.LanguageRepository.Exists(abbreviation))
            {
                _unitOfWorkRepository.LanguageRepository.Add(new Database.Entities.Languages
                {
                    Name = displayName,
                    Abbreviation = abbreviation,
                    Culture = isoName
                });

                _unitOfWorkRepository.SaveChanges();
            }
        }

        private bool IsLanguageSupported(string selectedLanguage, IEnumerable<string> scriptAvailableLanguages)
        {
            return scriptAvailableLanguages.Any(availableLanguage => availableLanguage.Equals(selectedLanguage, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
