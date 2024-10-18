namespace Serenity.Extensions;

[ScriptInclude]
public class SendResetPasswordResponse : ServiceResponse
{
    public string DemoLink { get; set; }    
}