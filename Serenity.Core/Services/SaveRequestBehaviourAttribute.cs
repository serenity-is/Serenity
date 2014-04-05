using System;

namespace Serenity.Services
{
    public abstract class SaveRequestBehaviourAttribute : Attribute
    {
        public virtual void OnValidateRequest(ISaveRequestHandler handler)
        {
        }

        public virtual void OnSetInternalFields(ISaveRequestHandler handler)
        {

        }

        public virtual void OnBeforeSave(ISaveRequestHandler handler)
        {

        }

        public virtual void OnAfterSave(ISaveRequestHandler handler)
        {

        }

        public virtual void OnReturn(ISaveRequestHandler handler)
        {

        }
    }
}