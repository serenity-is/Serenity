using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Html İçerik"), OptionsType(typeof(HtmlContentEditorOptions))]
    [Element("<textarea />")]
    public class HtmlContentEditor : Widget<HtmlContentEditorOptions>, IStringValue
    {
        private bool instanceReady;

        public HtmlContentEditor(jQueryObject textArea, HtmlContentEditorOptions opt)
            : base(textArea, opt)
        {
            IncludeCKEditor();

            string id = textArea.GetAttribute("id");
            if (id.IsTrimmedEmpty())
            {
                textArea.Attribute("id", this.uniqueName);
                id = this.uniqueName;
            }

            if (options.Cols != null)
                textArea.Attribute("cols", options.Cols.Value.ToString());

            if (options.Rows != null)
                textArea.Attribute("rows", options.Rows.Value.ToString());

            var self = this;

            var config = GetConfig();
            CKEditor.Replace(id, config);

            this.AddValidationRule(this.uniqueName, e =>
            {
                if (e.HasClass("required"))
                {
                    var value = self.Value.TrimToNull();
                    if (value == null)
                        return Q.Text("Validation.Required");
                }

                return null;
            });
        }

        protected virtual void InstanceReady(CKEditorEventArgs x)
        {
            instanceReady = true;
            J(x.Editor.Container.Element).AddClass(this.element.GetAttribute("class"));
            this.element.AddClass("select2-offscreen").CSS("display", "block"); // validasyonun çalışması için
            x.Editor.SetData(this.element.GetValue());
        }

        protected virtual CKEditorConfig GetConfig()
        {
            var self = this;

            return new CKEditorConfig
            {
                CustomConfig = "",
                Language = "tr",
                BodyClass = "s-HtmlContentBody",
                On = new CKEditorEvents
                {
                    InstanceReady = x => self.InstanceReady(x),
                    Change = x => {
                        x.Editor.UpdateElement();
                        self.Element.TriggerHandler("change");
                    }
                },
                ToolbarGroups = new CKToolbarGroup[] 
                {
                    new CKToolbarGroup { Name = "clipboard", Groups = new string[] { "clipboard", "undo" } },
                    new CKToolbarGroup { Name = "editing", Groups = new string[] { "find", "selection", "spellchecker" } },
                    new CKToolbarGroup { Name = "insert", Groups = new string[] { "links", "insert", "blocks", "bidi", "list", "indent" } },
                    new CKToolbarGroup { Name = "forms", Groups = new string[] { "forms", "mode", "document", "doctools", "others", "about", "tools" } },
                    new CKToolbarGroup { Name = "colors" },
                    new CKToolbarGroup { Name = "basicstyles", Groups = new string[] { "basicstyles", "cleanup" } },
                    new CKToolbarGroup { Name = "align" },
                    new CKToolbarGroup { Name = "styles" }
                },
                RemoveButtons = "SpecialChar,Anchor,Subscript,Styles",
                FormatTags = "p;h1;h2;h3;pre",
                RemoveDialogTabs = "image:advanced;link:advanced",
                ContentsCss = Q.ResolveUrl("~/content/site/site.htmlcontent.css"),
                Entities = false,
                EntitiesLatin = false,
                EntitiesGreek = false,
                AutoUpdateElement = true,
                Height = (options.Rows == null || options.Rows == 0) ? null : ((options.Rows * 20) + "px")
            };
        }

        private CKEditorInstance GetEditorInstance()
        {
            string id = this.element.GetAttribute("id");
            return CKEditor.Instances[id];
        }

        public override void Destroy()
        {
            var instance = GetEditorInstance();
            if (instance != null)
                instance.Destroy();

            base.Destroy();
        }

        public string Value
        {
            get 
            {
                var instance = GetEditorInstance();
                if (instanceReady && instance != null)
                    return instance.GetData();
                else
                    return this.element.GetValue();
            }
            set 
            {
                var instance = GetEditorInstance();
                this.element.Value(value);
                if (instanceReady && instance != null)
                    instance.SetData(value);
            }
        }

        private static void IncludeCKEditor()
        {
            dynamic window = Window.Instance;
            if (window.CKEDITOR != null)
                return;

            var script = J("CKEditorScript");
            if (script.Length > 0)
                return;

            J("<script/>")
                .Attribute("type", "text/javascript")
                .Attribute("id", "CKEditorScript")
                .Attribute("src", Q.ResolveUrl("~/Scripts/CKEditor/ckeditor.js"))
                .AppendTo(Window.Document.Head);

        }
    }

    [Serializable, Reflectable]
    public class HtmlContentEditorOptions
    {
        public HtmlContentEditorOptions()
        {
            Cols = 80;
            Rows = 6;
        }

        [Hidden]
        public int? Cols { get; set; }
        [Hidden]
        public int? Rows { get; set; }
    }
}