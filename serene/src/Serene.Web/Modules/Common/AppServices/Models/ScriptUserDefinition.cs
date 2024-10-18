namespace Serene;

/// <summary>
/// This data will be available from script code using a dynamic script.
/// Add properties you need from script code and set them in UserEndpoint.GetUserData.
/// </summary>
[ScriptInclude]
public class ScriptUserDefinition
{
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public bool IsAdmin { get; set; }
    public Dictionary<string, bool> Permissions { get; set; }
}