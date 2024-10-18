namespace Serene.Membership;

[FormScript("Membership.Login")]
[BasedOnRow(typeof(Administration.UserRow), CheckNames = true)]
public class LoginRequest : ServiceRequest
{
    [Placeholder("user name")]
    public string Username { get; set; }
    [PasswordEditor, Required(true), Placeholder("password")]
    public string Password { get; set; }
}