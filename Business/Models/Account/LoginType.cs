namespace Core.Models.Account
{
    public enum LoginType
    {
        NoAccountFound,
        IncorrectPassword,
        Banned,
        NotVerified,
        NoPermission,
        Success,
        IpBlocked
    }
}
