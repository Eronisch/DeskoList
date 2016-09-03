namespace Core.Models.Widgets
{
    public class WidgetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public byte OrderBy { get; set; }
        public byte Side { get; set; }
        public string Info { get; set; }
    }
}
