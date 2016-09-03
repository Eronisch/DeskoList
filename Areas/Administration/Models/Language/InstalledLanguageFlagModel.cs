using System.IO;
using Core.Business.File;

namespace Topsite.Areas.Administration.Models.Language
{
    public class InstalledLanguageFlagModel
    {
        private const string FlagPath = "Areas/Administration/Content/Img/flags/";

        public readonly int Id;
        public readonly string Name;
        public readonly string FlagName;
        public readonly string Abbreviation;
        public readonly string Culture;

        public InstalledLanguageFlagModel(Database.Entities.Languages language)
        {
            Id = language.Id;
            Name = language.Name;
            FlagName = GetFlagName(language.Abbreviation);
            Abbreviation = language.Abbreviation;
            Culture = language.Culture;
        }

        private string GetFlagName(string languageAbbreviation)
        {
            if (FileService.FileExists(GetFlagPath(languageAbbreviation)))
            {
                return GetFlagImageName(languageAbbreviation);
            }

            return GetFlagImageName("unknown");
        }

        private string GetFlagPath(string languageAbbreviation)
        {
            return Path.Combine(FileService.GetBaseDirectory(), GetRelativeFlagPath(languageAbbreviation));
        }

        private string GetRelativeFlagPath(string languageAbbreviation)
        {
            return Path.Combine(FlagPath, GetFlagImageName(languageAbbreviation));
        }

        private string GetFlagImageName(string languageAbbreviation)
        {
            return string.Format("{0}.png", languageAbbreviation);
        }
    }
}