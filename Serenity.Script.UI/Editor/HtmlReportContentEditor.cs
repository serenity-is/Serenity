using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Html Content (Report Compatible Limited Set)"), OptionsType(typeof(HtmlContentEditorOptions))]
    [Imported(ObeysTypeSystem = true), Element("<textarea />")]
    public class HtmlReportContentEditor : HtmlContentEditor, IStringValue
    {
        public HtmlReportContentEditor(jQueryObject textArea, HtmlContentEditorOptions opt)
            : base(textArea, opt)
        {
        }

        protected override CKEditorConfig GetConfig()
        {
            var config = base.GetConfig();
            config.RemoveButtons += ",Image,Table,HorizontalRule,Anchor,Blockquote,CreatePlaceholder,BGColor," + 
                "JustifyLeft,JustifyCenter,JustifyRight,JustifyBlock,Superscript";
            return config;
        }
    }
}