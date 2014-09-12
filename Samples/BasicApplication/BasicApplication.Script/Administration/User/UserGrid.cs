
namespace BasicApplication.Administration
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("UserId"), NameProperty("Username"), IsActiveProperty("IsActive")]
    [DialogType(typeof(UserDialog)), LocalTextPrefix("Administration.User"), Service("Administration/User")]
    public class UserGrid : EntityGrid<UserRow>
    {
        public UserGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "UserId", Width = 55, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "Username", Width = 200, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "Source", Width = 80 });
            columns.Add(new SlickColumn { Field = "PasswordHash", Width = 80 });
            columns.Add(new SlickColumn { Field = "PasswordSalt", Width = 80 });
            columns.Add(new SlickColumn { Field = "InsertDate", Width = 80 });
            columns.Add(new SlickColumn { Field = "InsertUserId", Width = 80 });
            columns.Add(new SlickColumn { Field = "IsActive", Width = 80 });
            columns.Add(new SlickColumn { Field = "UpdateDate", Width = 80 });
            columns.Add(new SlickColumn { Field = "UpdateUserId", Width = 80 });
            columns.Add(new SlickColumn { Field = "DisplayName", Width = 80 });
            columns.Add(new SlickColumn { Field = "Email", Width = 80 });

            return columns;
        }
    }

    [Imported, Serializable, PreserveMemberCase]
    public partial class UserRow
    {
    }
}