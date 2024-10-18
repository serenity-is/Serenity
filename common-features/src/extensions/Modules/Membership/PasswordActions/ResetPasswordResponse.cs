namespace Serenity.Extensions;

[ScriptInclude]
public class ResetPasswordResponse : ServiceResponse
{
    public bool RedirectHome { get; set; }
}