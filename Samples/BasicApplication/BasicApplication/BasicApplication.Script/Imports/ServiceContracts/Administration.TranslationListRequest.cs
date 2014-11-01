
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
    public partial class TranslationListRequest : ListRequest
    {
        public String SourceLanguageID { get; set; }
        public String TargetLanguageID { get; set; }
    }
    
}

