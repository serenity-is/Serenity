
namespace BasicApplication.Administration
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("Id"), NameProperty("LanguageName")]
    [FormKey("Administration.Language"), LocalTextPrefix("Administration.Language"), Service("Administration/Language")]
    public class LanguageDialog : EntityDialog<LanguageRow>, IAsyncInit
    {
    }
}