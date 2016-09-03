namespace Core.Models.Websites
{
    public enum AddWebsiteType
    {
        Success,
        EmptyBanner,
        InvalidBannerFileExtension,
        InvalidBannerFileSize,
        InvalidServerIp,
        InvalidServerPort,
        InvalidBannerUrl,
        Blacklist
    }
}
