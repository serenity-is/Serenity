using Serenity.Data;
using System;

namespace Serenity.Services
{
    /// <summary>
    /// Base type for explicitly activated save behavior attributes
    /// </summary>
    public abstract class SaveRequestBehaviorAttribute : Attribute, ISaveBehavior
    {
        /// <summary>Called when query to load old entity is built</summary>
        /// <param name="handler">Calling save request handler</param>
        public virtual void OnPrepareQuery(ISaveRequestHandler handler, SqlQuery query)
        {
        }

        /// <summary>Called when save request is validated</summary>
        /// <param name="handler">Calling save request handler</param>
        public virtual void OnValidateRequest(ISaveRequestHandler handler)
        {
        }

        /// <summary>Called when internal fields in row is being set</summary>
        /// <param name="handler">Calling save request handler</param>
        public virtual void OnSetInternalFields(ISaveRequestHandler handler)
        {
        }

        /// <summary>Called just before row is inserted to / updated in database</summary>
        /// <param name="handler">Calling save request handler</param>
        public virtual void OnBeforeSave(ISaveRequestHandler handler)
        {
        }

        /// <summary>Called after row is inserted to / updated in database</summary>
        /// <param name="handler">Calling save request handler</param>
        public virtual void OnAfterSave(ISaveRequestHandler handler)
        {
        }

        /// <summary>Called after row is inserted to / updated and auditing should be performed</summary>
        /// <param name="handler">Calling save request handler</param>
        public virtual void OnAudit(ISaveRequestHandler handler)
        {
        }

        /// <summary>Called before handler is returning the result</summary>
        /// <param name="handler">Calling save request handler</param>
        public virtual void OnReturn(ISaveRequestHandler handler)
        {
        }
    }

    [Obsolete("Use SaveRequestBehaviorAttribute")]
    public abstract class SaveRequestBehaviourAttribute : SaveRequestBehaviorAttribute
    {
    }
}