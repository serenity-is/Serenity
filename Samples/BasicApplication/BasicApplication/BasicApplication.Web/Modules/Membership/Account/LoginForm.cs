
namespace BasicApplication.Membership.Forms
{
    using System;
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using System.Collections.Generic;
    using System.ComponentModel;

    [FormScript("Membership.Login")]
    [BasedOnRow(typeof(Administration.Entities.UserRow))]
    public class LoginForm
    {
        [Placeholder("default username is 'admin'")]
        public String Username { get; set; }
        [PasswordEditor, Placeholder("default password for admin user is 'serenity'"), Required(true)]
        public String Password { get; set; }
    }
}