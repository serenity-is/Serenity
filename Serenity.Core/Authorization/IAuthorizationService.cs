namespace Serenity.Abstractions
{
    public interface IAuthorizationService
    {
        bool IsLoggedIn { get; }
        string Username { get; }
    }
}