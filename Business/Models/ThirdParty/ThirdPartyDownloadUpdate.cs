using Core.Models.Update;

namespace Core.Models.ThirdParty
{
    public class ThirdPartyDownloadUpdate : DownloadUpdate
    {
        /// <summary>
        /// Third party id (plugin id, widget id, theme id)
        /// </summary>
        public int ExtraId { get; set; }

        public ThirdPartyDownloadUpdate(string name, string description, string version, string downloadUrl, string deskoVersion, int taskId, string downloadPath, int progressPercentage, int extraId) : base(name, description, version, downloadUrl, deskoVersion, taskId, downloadPath, progressPercentage)
        {
            ExtraId = extraId;
        }
    }
}
