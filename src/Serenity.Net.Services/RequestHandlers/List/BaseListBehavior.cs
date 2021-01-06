using Serenity.Data;

namespace Serenity.Services
{
    public abstract class BaseListBehavior : IListBehavior
    {
        public virtual void OnValidateRequest(IListRequestHandler handler)
        {
        }

        public virtual void OnPrepareQuery(IListRequestHandler handler, SqlQuery query)
        {
        }

        public virtual void OnApplyFilters(IListRequestHandler handler, SqlQuery query)
        {
        }

        public virtual void OnBeforeExecuteQuery(IListRequestHandler handler)
        {
        }

        public virtual void OnAfterExecuteQuery(IListRequestHandler handler)
        {
        }

        public virtual void OnReturn(IListRequestHandler handler)
        {
        }
    }
}