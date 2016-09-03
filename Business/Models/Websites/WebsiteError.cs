namespace Core.Models.Websites
{
    public class WebsiteError
    {
        public AddWebsiteType ErrorType { get; set; }
        public string ErrorMessage { get; set; }

        public WebsiteError()
        {
            ErrorMessage = string.Empty;
        }
    }
}
