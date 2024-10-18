using Serene.Administration;

namespace Serene;

[Serializable]
public class UserDefinition : IUserDefinition, IHasPassword
{
    public string Id { get { return UserId.ToInvariant(); } }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string UserImage { get; set; }
    public short IsActive { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public string Source { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? LastDirectoryUpdate { get; set; }
    public bool HasPassword => PasswordSalt != "unassigned";
}