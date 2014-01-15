using System;
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
    }

    /// <summary>
    /// CKEDITOR import
    /// </summary>
    [Imported, Serializable]
    public class CKEditorInstance
    {
        public CKElement Container { get { return null; } }
        public Element Element { get { return null; } }
        public string Id { get { return null; } }
        public string Name { get { return null; } }

        public void Destroy()
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