using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Business.Download;
using Core.Models.ThirdParty;
using Core.Models.Update;

namespace Core.Business.Plugin
{
    /// <summary>
    /// Singleton manager for adding and retrieving plugin downloads
    /// </summary>
    public class PluginDownloadManager : UpdateDownloadManager
    {
        private PluginDownloadManager()
        {
            _thirdPartydownloadUpdates = new ConcurrentDictionary<int, ThirdPartyDownloadUpdate>();
        }

        private static readonly PluginDownloadManager Instance = new PluginDownloadManager();

        private readonly ConcurrentDictionary<int, ThirdPartyDownloadUpdate> _thirdPartydownloadUpdates;

        public static PluginDownloadManager Manager
        {
            get
            {
                return Instance;
            }
        }

        /// <summary>
        /// Get a specific download
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public new ThirdPartyDownloadUpdate GetDownload(int taskId)
        {
            return _thirdPartydownloadUpdates.First(u => u.Value.TaskId == taskId).Value;
        }

        /// <summary>
        /// Add a new download
        /// </summary>
        /// <param name="downloadId"></param>
        /// <param name="pluginId"></param>
        /// <param name="downloadUpdate"></param>
        public void AddDownload(int downloadId, int pluginId, DownloadUpdate downloadUpdate)
        {
            var thirdPartyDownload = (ThirdPartyDownloadUpdate) downloadUpdate;
            thirdPartyDownload.ExtraId = pluginId;

            _thirdPartydownloadUpdates.TryAdd(downloadId, thirdPartyDownload);

            base.AddDownload(downloadUpdate.TaskId, downloadUpdate);
        }

        /// <summary>
        /// Get all the updates that are downloading
        /// </summary>
        /// <returns></returns>
        public new IEnumerable<ThirdPartyDownloadUpdate> GetDownloads()
        {
            return base.GetDownloads().Select(u => GetDownload(u.TaskId));
        }

        /// <summary>
        /// Remove a specific download
        /// </summary>
        /// <param name="taskId"></param>
        public new void RemoveDownload(int taskId)
        {
            ThirdPartyDownloadUpdate downloadUpdate;
            _thirdPartydownloadUpdates.TryRemove(GetKey(taskId), out downloadUpdate);

            base.RemoveDownload(taskId);
        }

        private int GetKey(int taskId)
        {
            return _thirdPartydownloadUpdates.First(u => u.Value.TaskId == taskId).Key;
        }
    }
}
