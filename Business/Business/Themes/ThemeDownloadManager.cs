using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Business.Download;
using Core.Models.ThirdParty;
using Core.Models.Update;

namespace Core.Business.Themes
{
    public class ThemeDownloadManager : UpdateDownloadManager
    {
        private ThemeDownloadManager()
        {
            _thirdPartydownloadUpdates = new ConcurrentDictionary<int, ThirdPartyDownloadUpdate>();
        }

        private static readonly ThemeDownloadManager Instance = new ThemeDownloadManager();

        private readonly ConcurrentDictionary<int, ThirdPartyDownloadUpdate> _thirdPartydownloadUpdates;

        public static ThemeDownloadManager Manager
        {
            get
            {
                return Instance;
            }
        }

        public new ThirdPartyDownloadUpdate GetDownload(int taskId)
        {
            return _thirdPartydownloadUpdates.First(u => u.Value.TaskId == taskId).Value;
        }

        public void AddDownload(int downloadId, int widgetId, DownloadUpdate downloadUpdate)
        {
            var thirdPartyDownload = (ThirdPartyDownloadUpdate)downloadUpdate;
            thirdPartyDownload.ExtraId = widgetId;

            _thirdPartydownloadUpdates.TryAdd(downloadId, thirdPartyDownload);

            base.AddDownload(downloadUpdate.TaskId, downloadUpdate);
        }

        public new IEnumerable<ThirdPartyDownloadUpdate> GetDownloads()
        {
            return base.GetDownloads().Select(u => GetDownload(u.TaskId));
        }

        public new void RemoveDownload(int taskId)
        {
            ThirdPartyDownloadUpdate downloadUpdate;
            _thirdPartydownloadUpdates.TryRemove(GetKey(taskId), out downloadUpdate);

            base.RemoveDownload(taskId);
        }

        public int GetKey(int taskId)
        {
            return _thirdPartydownloadUpdates.First(u => u.Value.TaskId == taskId).Key;
        }
    }
}
