using Serenity.Data;

namespace Serenity.Services
{
    public abstract class BaseRetrieveBehavior : IRetrieveBehavior
    {
        public virtual void OnValidateRequest(IRetrieveRequestHandler handler)
        {
        }

        public virtual void OnPrepareQuery(IRetrieveRequestHandler handler, SqlQuery query)
        {
        }

        public virtual void OnApplyFilters(IRetrieveRequestHandler handler, SqlQuery query)
        {
        }

        public virtual void OnBeforeExecuteQuery(IRetrieveRequestHandler handler)
        {
        }

        public virtual void OnAfterExecuteQuery(IRetrieveRequestHandler handler)
        {
        }

        public virtual void OnReturn(IRetrieveRequestHandler handler)
        {
        }
    }
}