using Serenity.Data;
using System;

namespace Serenity.Services
{
    public abstract class BaseSaveDeleteBehavior : BaseSaveBehavior, IDeleteBehavior, IDeleteExceptionBehavior
    {
        public virtual void OnPrepareQuery(IDeleteRequestHandler handler, SqlQuery query)
        {
        }

        public virtual void OnValidateRequest(IDeleteRequestHandler handler)
        {
        }

        public virtual void OnBeforeDelete(IDeleteRequestHandler handler)
        {
        }

        public virtual void OnAfterDelete(IDeleteRequestHandler handler)
        {
        }

        public virtual void OnAudit(IDeleteRequestHandler handler)
        {
        }

        public virtual void OnReturn(IDeleteRequestHandler handler)
        {
        }

        public virtual void OnException(IDeleteRequestHandler handler, Exception exception)
        {
        }
    }
}