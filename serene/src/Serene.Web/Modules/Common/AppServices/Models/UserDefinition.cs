namespace Serene;

[Serializable]
public class UserDefinition : IUserDefinition, IHasPassword
{
    public string Id { get { return UserId.ToInvariant(); } }
    public required string DisplayName { get; set; }
    public string? Email { get; set; }
    public string? UserImage { get; set; }
    public required short IsActive { get; set; }
    public required int UserId { get; set; }
    public required string Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    public string? Source { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? LastDirectoryUpdate { get; set; }
    public bool HasPassword => PasswordSalt != "unassigned";
}