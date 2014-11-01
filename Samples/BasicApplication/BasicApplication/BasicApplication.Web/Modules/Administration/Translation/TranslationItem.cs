

namespace BasicApplication.Administration
{
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using Serenity.Services;
    using System;
    using System.Data;

    [ScriptInclude]
    public class TranslationItem
    {
        public string Key { get; set; }
        public string SourceText { get; set; }
        public string TargetText { get; set; }
        public string CustomText { get; set; }
    }
}