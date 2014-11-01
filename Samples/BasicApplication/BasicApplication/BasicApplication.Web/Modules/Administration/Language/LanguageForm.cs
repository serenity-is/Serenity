
namespace BasicApplication.Administration.Forms
{
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.IO;

    [FormScript("Administration.Language")]
    [BasedOnRow(typeof(Entities.LanguageRow))]
    public class LanguageForm
    {
        public String LanguageId { get; set; }
        public String LanguageName { get; set; }
    }
}