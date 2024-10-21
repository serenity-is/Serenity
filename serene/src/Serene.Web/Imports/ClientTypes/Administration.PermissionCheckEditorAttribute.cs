using Serenity;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Serene.Administration;

public partial class PermissionCheckEditorAttribute : CustomEditorAttribute
{
    public const string Key = "Serene.Administration.PermissionCheckEditor";

    public PermissionCheckEditorAttribute()
        : base(Key)
    {
    }

    public object ImplicitPermissions
    {
        get { return GetOption<object>("implicitPermissions"); }
        set { SetOption("implicitPermissions", value); }
    }

    public object RolePermissions
    {
        get { return GetOption<object>("rolePermissions"); }
        set { SetOption("rolePermissions", value); }
    }

    public bool ShowRevoke
    {
        get { return GetOption<bool>("showRevoke"); }
        set { SetOption("showRevoke", value); }
    }

    public object Value
    {
        get { return GetOption<object>("value"); }
        set { SetOption("value", value); }
    }
}