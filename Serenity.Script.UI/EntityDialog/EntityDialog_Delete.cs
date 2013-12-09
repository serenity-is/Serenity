using jQueryApi;
using System;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions : class, new()
    {
        protected virtual ServiceCallOptions<DeleteResponse> GetDeleteOptions(Action<DeleteResponse> callback)
        {
            return new ServiceCallOptions<DeleteResponse>();
        }

        protected virtual void DeleteHandler(ServiceCallOptions<DeleteResponse> options, Action<DeleteResponse> callback)
        {
            Q.ServiceCall(options);
        }

        protected virtual void DoDelete(Action<DeleteResponse> callback)
        {
            var self = this;
            var baseOptions = new ServiceCallOptions<DeleteResponse>();
            baseOptions.Service = this.GetEntityType().Replace('.', '/') + "/Delete";

            var request = new DeleteRequest();
            request.EntityId = EntityId.As<long>();
            baseOptions.Request = request;

            baseOptions.OnSuccess = response =>
            {
                if (callback != null)
                    callback(response);
                self.element.Trigger("ondatachange", new object[] { "delete" });
            };

            var thisOptions = GetDeleteOptions(callback);

            var finalOptions = jQuery.ExtendObject(
                baseOptions, thisOptions);

            //if (Entity._deleteHandler != null)
            //    EntityObject._deleteHandler(this, finalOptions, callback);
            //else
            DeleteHandler(finalOptions, callback);
        }
    }
}