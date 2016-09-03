using System.Collections.Generic;
using Core.Business.Download;
using Core.Models.Update;

namespace Core.Business.Software
{
    public class SoftwareDownloadManager : UpdateDownloadManager
    {
        private SoftwareDownloadManager() { }

        private static readonly SoftwareDownloadManager Instance = new SoftwareDownloadManager();

        public static SoftwareDownloadManager Manager
        {
            get
            {
                return Instance;
            }
        }

        public new DownloadUpdate GetDownload(int taskId)
        {
            return base.GetDownload(taskId);
        }

        public new void AddDownload(int taskId, DownloadUpdate downloadUpdate)
        {
            base.AddDownload(taskId, downloadUpdate);
        }

        public new IEnumerable<DownloadUpdate> GetDownloads()
        {
            return base.GetDownloads();
        }

        public new void RemoveDownload(int taskId)
        {
            base.RemoveDownload(taskId);
        }
    }
}
