namespace Core.Models.Pages
{
    public class PageModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int TypeId { get; set; }
        public int Order { get; set; }
        public bool Enabled { get; set; }
        public bool InMenu { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Information { get; set; }
    }
}