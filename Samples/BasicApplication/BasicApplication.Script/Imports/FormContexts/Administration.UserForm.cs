
namespace BasicApplication.Administration
{
    using Serenity;
    using Serenity.ComponentModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public partial class UserForm : PrefixedContext
    {
        public UserForm(string idPrefix) : base(idPrefix) {}
    
        public StringEditor Username { get { return ById<StringEditor>("Username"); } }
        public StringEditor Source { get { return ById<StringEditor>("Source"); } }
        public StringEditor PasswordHash { get { return ById<StringEditor>("PasswordHash"); } }
        public StringEditor PasswordSalt { get { return ById<StringEditor>("PasswordSalt"); } }
        public DateEditor InsertDate { get { return ById<DateEditor>("InsertDate"); } }
        public IntegerEditor InsertUserId { get { return ById<IntegerEditor>("InsertUserId"); } }
        public StringEditor IsActive { get { return ById<StringEditor>("IsActive"); } }
        public DateEditor UpdateDate { get { return ById<DateEditor>("UpdateDate"); } }
        public IntegerEditor UpdateUserId { get { return ById<IntegerEditor>("UpdateUserId"); } }
        public StringEditor DisplayName { get { return ById<StringEditor>("DisplayName"); } }
        public StringEditor Email { get { return ById<StringEditor>("Email"); } }
    }
}

