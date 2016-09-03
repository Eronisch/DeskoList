using System;

namespace Topsite.Areas.Administration.Models.Settings
{
    public class SettingsUpdateModel
    {
        public DateTime? LastCheckedForUpdates { get; set; }
        public bool IsDownloadingUpdates { get; set; }
        public int AmountUpdatesAvailable { get; set; }
        public UpdateAvailableStatus StatusUpdates { get; set; }
        public bool IsDownloading { get; set; }
        public bool IsInstalling { get; set; }
        public bool IsChecking { get; set; }

        public bool SingleUpdateAvailable
        {
            get { return AmountUpdatesAvailable == 1; }
        }

        public bool MultipleUpdatesAvailable
        {
            get { return AmountUpdatesAvailable > 1; }
        }

        public bool UpdatesAvailable
        {
            get { return AmountUpdatesAvailable > 0; }
        }
    }
}