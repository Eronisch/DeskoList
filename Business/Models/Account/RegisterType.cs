namespace Core.Models.Account
{
    public enum RegisterType
    {
        Success,
        UsernameAlreadyTaken,
        EmailAlreadyTaken,
        BlackList,
        Captcha
    }
}
