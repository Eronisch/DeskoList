using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Models.Update;

namespace Core.Business.Download
{
    /// <summary>
    /// Used as a base class for system, widget and themes download managers
    /// </summary>
    public abstract class UpdateDownloadManager
    {
        private readonly ConcurrentDictionary<int, DownloadUpdate> _downloadingUpdates;

        protected UpdateDownloadManager()
        {
            _downloadingUpdates = new ConcurrentDictionary<int, DownloadUpdate>();
        }

        protected void AddDownload(int taskId, DownloadUpdate downloadUpdate)
        {
            _downloadingUpdates.TryAdd(taskId, downloadUpdate);
        }

        protected DownloadUpdate GetDownload(int taskId)
        {
            return _downloadingUpdates[taskId];
        }

        protected IEnumerable<DownloadUpdate> GetDownloads()
        {
            return _downloadingUpdates.Values;
        }

        protected void RemoveDownload(int taskId)
        {
            DownloadUpdate downloadUpdate;

            _downloadingUpdates.TryRemove(taskId, out downloadUpdate);
        }

        public bool IsDownloading(string version)
        {
            return _downloadingUpdates.Any(x => x.Value.Version == version);
        }

        /// <summary>
        /// Get the progress of the downloading update
        /// </summary>
        /// <param name="version"></param>
        /// <returns>Percentage of download, if the version isn't found it returns 0</returns>
        public int GetProgress(string version)
        {
            var download = _downloadingUpdates.FirstOrDefault(x => x.Value.Version == version);

            return download.Value != null ? download.Value.ProgressPercentage : 0;
        }

        public void UpdateProgress(int taskId, int progressPercentage)
        {
            _downloadingUpdates[taskId].SetProgress(progressPercentage);
        }
    }
}
