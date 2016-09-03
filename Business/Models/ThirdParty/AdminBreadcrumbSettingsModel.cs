namespace Core.Models.ThirdParty
{
    public class AdminBreadcrumbSettingsModel
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string LocalizedBase { get; set; }
        public string LocalizedTitle { get; set; }
        public string LocalizedControllerFriendlyName { get; set; }
        public string LocalizedActionFriendlyName { get; set; }
        public string LocalizedDescription { get; set; }
        public string Icon { get; set; }
    }
}
