using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Core.Business.File;

namespace Core.Business.Software
{
    public class SoftwareSettings
    {
        private readonly XDocument _settingsDocument;

        public SoftwareSettings(string pathSettingsFile)
        {
            if (FileService.FileExists(pathSettingsFile))
            {
                _settingsDocument = XDocument.Load(pathSettingsFile);    
            }
        }

        public IEnumerable<string> GetRelativePathsToDelete()
        {
            return
                _settingsDocument != null ? _settingsDocument.Descendants("DeleteFiles")
                    .Elements("File")
                    .Select(x => x.Value) : new string[0];
        }
    }
}
