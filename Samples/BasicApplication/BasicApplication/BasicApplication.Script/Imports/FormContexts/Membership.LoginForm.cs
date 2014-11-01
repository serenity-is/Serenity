
namespace BasicApplication.Membership
{
    using Serenity;
    using Serenity.ComponentModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public partial class LoginForm : PrefixedContext
    {
        public LoginForm(string idPrefix) : base(idPrefix) {}
    
        public StringEditor Username { get { return ById<StringEditor>("Username"); } }
        public PasswordEditor Password { get { return ById<PasswordEditor>("Password"); } }
    }
}

