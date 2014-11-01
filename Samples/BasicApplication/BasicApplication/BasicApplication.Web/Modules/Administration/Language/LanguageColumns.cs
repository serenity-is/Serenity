
namespace BasicApplication.Administration.Forms
{
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.IO;

    [ColumnsScript("Administration.Language")]
    [BasedOnRow(typeof(Entities.LanguageRow))]
    public class LanguageColumns
    {
        [EditLink]
        public String LanguageId { get; set; }
        [EditLink]
        public String LanguageName { get; set; }
    }
}