using System;

namespace Serenity.ComponentModel
{
    public class EditorFilteringAttribute : CustomFilteringAttribute
    {
        public EditorFilteringAttribute()
            : base("Editor")
        {
        }

        public EditorFilteringAttribute(string editorType)
            : base("Editor")
        {
            EditorType = editorType;
        }

        public EditorFilteringAttribute(Type editorTypeAttribute)
            : base("Editor")
        {
            EditorType = ((EditorTypeAttribute)Activator.CreateInstance(editorTypeAttribute)).EditorType;
        }

        public String EditorType
        {
            get { return GetOption<String>("editorType"); }
            set { SetOption("editorType", value); }
        }

        public Boolean UseRelative
        {
            get { return GetOption<Boolean>("useRelative"); }
            set { SetOption("useSearch", value); }
        }

        public Boolean UseLike
        {
            get { return GetOption<Boolean>("useLike"); }
            set { SetOption("useLike", value); }
        }
    }
}