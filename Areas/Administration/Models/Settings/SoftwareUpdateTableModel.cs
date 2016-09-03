namespace Topsite.Areas.Administration.Models.Settings
{
    public class SoftwareUpdateTableModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public bool IsDownloading { get; set; }
        public bool IsDownloaded { get; set; }
        public int Progress { get; set; }
    }
}