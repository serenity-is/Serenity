
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
    public partial class LanguageRow
    {
        public Int32? Id { get; set; }
        public String LanguageId { get; set; }
        public String LanguageName { get; set; }
    
        [Imported, PreserveMemberCase]
        public static class Fields
        {
            [InlineConstant] public const string Id = "Id";
            [InlineConstant] public const string LanguageId = "LanguageId";
            [InlineConstant] public const string LanguageName = "LanguageName";
        }
    }
    
}

