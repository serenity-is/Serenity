using System;
using System.Collections;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Options for the CKEditor
    /// </summary>
    [Imported, Serializable]
    public class CKEditorConfig
    {
        public string CustomConfig { get; set; }
        public string ContentsCss { get; set; }
        public bool AutoUpdateElement { get; set; }
        public string DefaultLanguage { get; set; }
        public String Height { get; set; }
        public String Width { get; set; }
        public Int32 TabIndex { get; set; }
        public Int32 BaseFloatZIndex { get; set; }
        public string Language { get; set; }
        public string BodyClass { get; set; }
        public string Plugins { get; set; }
        public string ExtraPlugins { get; set; }
        public string RemovePlugins { get; set; }
        public CKToolbarGroup[] ToolbarGroups { get; set; }
        public string RemoveButtons { get; set; }
        [ScriptName("format_tags")]
        public string FormatTags { get; set; }
        public string RemoveDialogTabs { get; set; }
        public CKEditorEvents On { get; set; }

        public bool Entities { get; set; }
        [ScriptName("entities_latin")]
        public bool EntitiesLatin { get; set; }
        [ScriptName("entities_greek")]
        public bool EntitiesGreek { get; set; }
        [ScriptName("entities_additional")]
        public string EntitiesAdditional { get; set; }

        public string FilebrowserBrowseUrl { get; set; }
    }

    [Imported, Serializable]
    public class CKEditorEvents
    {
        public Action<CKEditorEventArgs> InstanceReady;
        public Action<CKEditorEventArgs> Change;
    }

    [Imported, Serializable]
    public class CKEditorEventArgs
    {
        public CKEditorInstance Editor;
    }

    [Imported, Serializable]
    public class CKToolbarGroup
    {
        public string Name { get; set; }
        public string[] Groups { get; set; }
    }

    /// <summary>
    /// CKEDITOR import
    /// </summary>
    [Imported, IgnoreNamespace, ScriptName("CKEDITOR")]
    public static class CKEditor
    {
        public static void Replace(string textAreaID, CKEditorConfig config)
        {
        }

        public static void On(string eventName, Action callback)
        {
        }

        public static readonly JsDictionary<string, CKEditorInstance> Instances;

        public static Action<CKEditorConfig> EditorConfig;

        public static CKEditorConfig Config;

        [InlineCode("(!!CKEDITOR.lang.languages[{language}])")]
        public static bool HasLanguage(string language)
        {
            return false;
        }
    }

    /// <summary>
    /// CKEDITOR import
    /// </summary>
    [Imported, ScriptNamespace("CKEDITOR"), ScriptName("editor")]
    public class CKEditorInstance
    {
        public CKElement Container;
        public Element Element;
        public string Id;
        public string Name;

        public void Destroy()
        {
        }

        public string GetData()
        {
            return null;
        }

        public void SetData(string data)
        {
        }

        public void UpdateElement()
        {
        }
    }

    [Imported, Serializable]
    public class CKElement
    {
        [ScriptName("$")]
        public Element Element { get { return null; } }
    }
}