namespace Topsite.Areas.Administration.Models.ThirdParty
{
    public class ThirdPartyCookie
    {
        public ThirdPartyCookie(string message, bool isSuccess)
        {
            Message = message;
            IsSuccess = isSuccess;
        }

        public string Message { get; private set; }
        public bool IsSuccess { get; private set; }
    }
}