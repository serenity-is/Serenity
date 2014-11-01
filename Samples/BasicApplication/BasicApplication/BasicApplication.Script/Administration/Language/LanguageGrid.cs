
namespace BasicApplication.Administration
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [ColumnsKey("Administration.Language"), IdProperty("Id"), NameProperty("LanguageName")]
    [DialogType(typeof(LanguageDialog)), LocalTextPrefix("Administration.Language"), Service("Administration/Language")]
    public class LanguageGrid : EntityGrid<LanguageRow>, IAsyncInit
    {
        public LanguageGrid(jQueryObject container)
            : base(container)
        {
        }
    }
}