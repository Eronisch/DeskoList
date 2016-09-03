namespace Core.Models.Settings
{
    public class SoftwareOpenUpdateModel
    {
        public SoftwareOpenUpdateModel(string name, string description, string version, string downloadUrl)
        {
            Name = name;
            Description = description;
            Version = version;
            DownloadUrl = downloadUrl;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Version { get; private set; }
        public string DownloadUrl { get; private set; }
    }
}
