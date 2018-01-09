using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public abstract class BaseEditorFiltering : BaseFiltering
    {
        protected Widget editor;

        protected BaseEditorFiltering(Type editorType)
        {
        }

        protected virtual bool UseEditor()
        {
            return false;
        }

        protected virtual bool UseIdField()
        {
            return false;
        }

        protected virtual object GetEditorOptions()
        {
            return null;
        }
    }
}