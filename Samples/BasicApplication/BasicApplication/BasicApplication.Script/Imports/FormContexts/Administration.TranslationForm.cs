
namespace BasicApplication.Administration
{
    using Serenity;
    using Serenity.ComponentModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public partial class TranslationForm : PrefixedContext
    {
        public TranslationForm(string idPrefix) : base(idPrefix) {}
    
        public StringEditor TextKey { get { return ById<StringEditor>("TextKey"); } }
        public StringEditor LanguageId { get { return ById<StringEditor>("LanguageId"); } }
        public StringEditor Translation { get { return ById<StringEditor>("Translation"); } }
    }
}

