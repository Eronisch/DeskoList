using System.Collections.Generic;

namespace Core.Business.Languages
{
    /// <summary>
    /// ElFinder (Filemanager Desko uses) config
    /// </summary>
    public class ElFinderConfig
    {
        /// <summary>
        /// Return the available languages ElFinder supports
        /// </summary>
        public IEnumerable<string> GetInstalledLanguages()
        {
            return new[]
                {
                    "ar", // Arabic
                    "bg", // Bulgarian
                    "ca", // Catalan
                    "cs", // Czech
                    "de", // German
                    "fr", // French
                    "hu", // Hungarian
                    "jp", // Japanese
                    "no", // Norwegian
                    "pl", // Polish
                    "pt", // Portuguese, well technically brazilian portuguese, but we are keeping it simple, i am sorry Portugueses. Feel free to criticize me
                    "ru", // Russian
                    "cn", // Chinese
                    "es", // Spanish
                    "en", // English
                    "nl", // Dutch
                };
        }
    }
}