namespace Serene.Administration.Forms;

[ColumnsScript("Administration.Role")]
[BasedOnRow(typeof(RoleRow), CheckNames = true)]
public class RoleColumns
{
    [EditLink, DisplayName("Db.Shared.RecordId"), AlignRight]
    public int RoleId { get; set; }
    [EditLink, Width(300)]
    public string RoleName { get; set; }
}