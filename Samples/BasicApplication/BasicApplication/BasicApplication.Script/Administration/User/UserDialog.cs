
namespace BasicApplication.Administration
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("UserId"), NameProperty("Username"), IsActiveProperty("IsActive")]
    [FormKey("Administration.User"), LocalTextPrefix("Administration.User"), Service("Administration/User")]
    public class UserDialog : EntityDialog<UserRow>
    {
    }
}