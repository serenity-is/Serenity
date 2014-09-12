
namespace BasicApplication.Membership
{
    using Serenity.Services;

    public class LoginRequest : ServiceRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}