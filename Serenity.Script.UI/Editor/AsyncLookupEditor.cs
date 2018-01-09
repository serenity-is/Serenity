using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true), Editor, OptionsType(typeof(LookupEditorOptions))]
    public class AsyncLookupEditor : LookupEditorBase<LookupEditorOptions, object>, IAsyncInit
    {
        public AsyncLookupEditor(jQueryObject hidden, LookupEditorOptions opt)
            : base(hidden, opt)
        {
        }

        protected override string GetLookupKey()
        {
            return options.LookupKey ?? base.GetLookupKey();
        }
    }
}