
namespace Serene.Membership;

public class ActivateEmailModel
{
    public string? Username { get; set; }
    public string? DisplayName { get; set; }
    public string? ActivateLink { get; set; }
    /// <summary>The hours that the link works.</summary>
    public int LinkHours { get; set; }
}