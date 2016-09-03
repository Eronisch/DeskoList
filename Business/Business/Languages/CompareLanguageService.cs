using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Business.File;

namespace Core.Business.Languages
{
    /// <summary>
    /// Service for comparing the languages
    /// </summary>
    public class CompareLanguageService
    {
        private readonly string _defaultFilePath;
        private readonly string _relativeFilePathToCheck;
        private readonly string _languageAbbreviation;

        public CompareLanguageService(string relativeFilePathToCheck, string languageAbbrevation)
        {
            _defaultFilePath = GetDefaultLocalizationFilePath();
            _relativeFilePathToCheck = GetFilePathToCheck(relativeFilePathToCheck);
            _languageAbbreviation = languageAbbrevation;
        }

        public IList<TypeInfo> GetDifferences()
        {
            var defaultReflectionTypes = new ReflectionLanguageService(_defaultFilePath).Get().ToList();
            var checkResourceDifference = new ResourceManagerCompareService(_relativeFilePathToCheck, _languageAbbreviation, defaultReflectionTypes);

            return checkResourceDifference.Compare();
        }

        private string GetFilePathToCheck(string relativeFilePathToCheck)
        {
            return Path.Combine(FileService.GetBaseDirectory(), relativeFilePathToCheck);
        }

        private string GetDefaultLocalizationFilePath()
        {
            return Path.Combine(FileService.GetBinPath(), "Localization.dll");
        }
    }
}
