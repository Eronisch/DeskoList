using System.Collections.Generic;
using System.IO;
using Core.Business.File;

namespace Core.Business.ThirdParty
{
    /// <summary>
    /// Abstract config that is used for retrieving file (locations) for widgets and plugins
    /// </summary>
    public abstract class AbstractConfigService
    {
        private readonly string _folderName;

        private readonly string _tempFolderName;

        protected AbstractConfigService(string folderName, string tempFolderName)
        {
            _folderName = folderName;
            _tempFolderName = tempFolderName;
        }

        public virtual string GetBaseThirdPartyPath()
        {
            return Path.Combine(FileService.GetBaseDirectory(), _folderName);
        }

        public virtual string GetRelativePath()
        {
            return _folderName;
        }

        public virtual string GetTempPathFolderPath()
        {
            return Path.Combine(FileService.GetBaseDirectory(), _tempFolderName);
        }

        public virtual string GetTempFolderName()
        {
            return _tempFolderName;
        }

        public virtual string GetAreaPath(string areaName)
        {
            return Path.Combine(GetBaseThirdPartyPath(), areaName);
        }

        public virtual string GetRelativeAreaPath(string areaName)
        {
            return Path.Combine(_folderName, areaName);
        }

        public virtual string GetTempFolderPath(string tempFolderName)
        {
            return Path.Combine(GetTempPathFolderPath(), tempFolderName);
        }

        public virtual string GetSettingsPath(string areaName)
        {
            return Path.Combine(GetAreaPath(areaName), "Settings.xml");
        }

        public virtual string GetTempSettingsPath(string tempFolderName)
        {
            return Path.Combine(GetTempFolderPath(tempFolderName), "Settings.xml");
        }

        public virtual IEnumerable<string> GetDlls(string areaName)
        {
            return Directory.GetFiles(Path.Combine(GetAreaPath(areaName), "bin"), "*.dll");
        }

        public virtual IEnumerable<string> GetTempDlls(string tempWidgetName)
        {
            return Directory.GetFiles(Path.Combine(GetTempFolderPath(tempWidgetName), "bin"), "*.dll");
        }
    }
}
