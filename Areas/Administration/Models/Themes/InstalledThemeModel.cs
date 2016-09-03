namespace Topsite.Areas.Administration.Models.Themes
{
    public class InstalledThemeModel
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public string ThemeName { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string AuthorUrl { get; set; }
        public bool IsActive { get; set; }
        public string ImagePath { get; set; }
    }
}