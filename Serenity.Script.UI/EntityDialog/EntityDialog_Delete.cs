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
            baseOptions.Service = this.GetService() + "/Delete";

            var request = new DeleteRequest();
            request.EntityId = EntityId;
            baseOptions.Request = request;

            baseOptions.OnSuccess = response =>
            {
                self.OnDeleteSuccess(response);
                if (callback != null)
                    callback(response);
                self.element.TriggerHandler("ondatachange", new object[] { 
                    new DataChangeInfo { EntityId = request.EntityId, Entity = this.entity, Type = "delete" } 
                });
            };

            var thisOptions = GetDeleteOptions(callback);

            var finalOptions = jQuery.ExtendObject(
                baseOptions, thisOptions);

            DeleteHandler(finalOptions, callback);
        }

        protected virtual void OnDeleteSuccess(DeleteResponse response)
        {
            
        }
    }
}