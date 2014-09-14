
namespace BasicApplication.Administration.Forms
{
    using System;
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using System.Collections.Generic;

    [FormScript("Administration.User")]
    [BasedOnRow(typeof(Entities.UserRow))]
    public class UserForm
    {
        public String Username { get; set; }
        public String Source { get; set; }
        public String PasswordHash { get; set; }
        public String PasswordSalt { get; set; }
        public DateTime InsertDate { get; set; }
        public Int32 InsertUserId { get; set; }
        public Int16 IsActive { get; set; }
        public DateTime UpdateDate { get; set; }
        public Int32 UpdateUserId { get; set; }
        public String DisplayName { get; set; }
        public String Email { get; set; }
    }
}