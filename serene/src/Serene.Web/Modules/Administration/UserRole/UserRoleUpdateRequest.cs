namespace Serene.Administration;

public class UserRoleUpdateRequest : ServiceRequest
{
    public int? UserID { get; set; }
    public List<int> Roles { get; set; }
}