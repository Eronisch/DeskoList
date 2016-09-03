namespace Core.Models.Update
{
    public class DownloadUpdate : OpenUpdateModel
    {
        public int TaskId { get; private set; }
        public string DownloadPath { get; private set; }
        public int ProgressPercentage { get; private set; }

        public DownloadUpdate(string name, string description, string version, string downloadUrl, string deskoVersion, int taskId, string downloadPath, int progressPercentage) : base(name, description, version, downloadUrl, deskoVersion)
        {
            TaskId = taskId;
            DownloadPath = downloadPath;
            ProgressPercentage = progressPercentage;
        }

        public void SetProgress(int percentage)
        {
            ProgressPercentage = percentage;
        }
    }
}
