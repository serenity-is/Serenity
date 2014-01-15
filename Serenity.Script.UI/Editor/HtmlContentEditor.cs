using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("Çok Satırlı Metin", typeof(HtmlContentEditorOptions))]
    [Element("<textarea />")]
    public class HtmlContentEditor : Widget<HtmlContentEditorOptions>, IStringValue
    {
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

            CKEditor.Replace(id, new CKEditorConfig
            {
                CustomConfig = "",
                Language = "tr",
                BodyClass = "s-HtmlContentBody"
            });

            CKEditor.On("instanceReady", delegate
            {
                CKEditorInstance instance = CKEditor.Instances[id];
                J(instance.Container.Element).AddClass(textArea.GetAttribute("class"));
            });
        }

        public override void Destroy()
        {
            string id = this.element.GetAttribute("id");
            CKEditorInstance instance = CKEditor.Instances[id];
            if (instance != null)
                instance.Destroy();

            base.Destroy();
        }

        public string Value
        {
            get { return this.element.GetValue(); }
            set { this.element.Value(value); }
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

            CKEditor.Config.ToolbarGroups = new CKToolbarGroup[] 
            {
                new CKToolbarGroup { Name = "clipboard", Groups = new string[] { "clipboard", "undo" } },
                new CKToolbarGroup { Name = "editing", Groups = new string[] { "find", "selection", "spellchecker" } },
                new CKToolbarGroup { Name = "insert", Groups = new string[] { "links", "insert", "blocks", "bidi", "list", "indent" } },
                new CKToolbarGroup { Name = "forms", Groups = new string[] { "forms", "mode", "document", "doctools", "others", "about", "tools" } },
                new CKToolbarGroup { Name = "colors" },
                new CKToolbarGroup { Name = "basicstyles", Groups = new string[] { "basicstyles", "cleanup" } },
                new CKToolbarGroup { Name = "align" },
                new CKToolbarGroup { Name = "styles" }
            };

            CKEditor.Config.RemoveButtons = "SpecialChar,Anchor,Subscript,Styles";
            CKEditor.Config.FormatTags = "p;h1;h2;h3;pre";
            CKEditor.Config.RemoveDialogTabs = "image:advanced;link:advanced";
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