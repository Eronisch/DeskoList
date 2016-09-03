namespace Core.Business.Settings
{
    /// <summary>
    /// Uses the SettingsService to retrieve the settings
    /// Creates a new SettingsService object for each retrieval
    /// </summary>
    public static class SettingsHelper
    {
        public static string Title
        {
            get { return new SettingsService().GetTitle(); }
        }

        public static string ShortTitle
        {
            get { return new SettingsService().GetShortTitle(); }
        }

        public static string Slogan
        {
            get { return new SettingsService().GetSlogan(); }
        }

        public static string Description
        {
            get { return new SettingsService().GetDescription(); }
        }

        public static string Keywords
        {
            get { return new SettingsService().GetKeywords(); }
        }

        public static string ThemeFolderName
        {
            get { return new SettingsService().GetActiveThemeMap(); }
        }
    }
}
