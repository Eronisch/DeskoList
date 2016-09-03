namespace Core.Models.Websites
{
    public class RatingModel
    {
        public int Rating { get; set; }
        public string ServerName { get; set; }
        public int WebsiteId { get; set; }

        public RatingModel()
        {
            Rating = 5;
        }
    }
}