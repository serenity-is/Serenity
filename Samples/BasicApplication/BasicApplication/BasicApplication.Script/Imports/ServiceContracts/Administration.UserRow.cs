
namespace BasicApplication.Administration
{
    using Serenity;
    using Serenity.ComponentModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [Imported, Serializable, PreserveMemberCase]
    public partial class UserRow
    {
        public Int32? UserId { get; set; }
        public String Username { get; set; }
        public String Source { get; set; }
        public String PasswordHash { get; set; }
        public String PasswordSalt { get; set; }
        public String DisplayName { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }
        public String PasswordConfirm { get; set; }
        public Int32? InsertUserId { get; set; }
        public String InsertDate { get; set; }
        public Int32? UpdateUserId { get; set; }
        public String UpdateDate { get; set; }
        public Int16? IsActive { get; set; }
    
        [Imported, PreserveMemberCase]
        public static class Fields
        {
            [InlineConstant] public const string UserId = "UserId";
            [InlineConstant] public const string Username = "Username";
            [InlineConstant] public const string Source = "Source";
            [InlineConstant] public const string PasswordHash = "PasswordHash";
            [InlineConstant] public const string PasswordSalt = "PasswordSalt";
            [InlineConstant] public const string DisplayName = "DisplayName";
            [InlineConstant] public const string Email = "Email";
            [InlineConstant] public const string Password = "Password";
            [InlineConstant] public const string PasswordConfirm = "PasswordConfirm";
            [InlineConstant] public const string InsertUserId = "InsertUserId";
            [InlineConstant] public const string InsertDate = "InsertDate";
            [InlineConstant] public const string UpdateUserId = "UpdateUserId";
            [InlineConstant] public const string UpdateDate = "UpdateDate";
            [InlineConstant] public const string IsActive = "IsActive";
        }
    }
    
}

