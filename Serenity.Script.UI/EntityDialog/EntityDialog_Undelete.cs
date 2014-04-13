using jQueryApi;
using System;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions : class, new()
    {
        protected virtual ServiceCallOptions<UndeleteResponse> GetUndeleteOptions(Action<UndeleteResponse> callback)
        {
            return new ServiceCallOptions<UndeleteResponse>();
        }

        protected virtual void UndeleteHandler(ServiceCallOptions<UndeleteResponse> options, Action<UndeleteResponse> callback)
        {
            Q.ServiceCall(options);
        }

        protected virtual void Undelete(Action<UndeleteResponse> callback)
        {
            var self = this;
            var baseOptions = new ServiceCallOptions<UndeleteResponse>();
            baseOptions.Service = this.GetService() + "/Undelete";

            var request = new UndeleteRequest();
            request.EntityId = EntityId.As<long>();
            baseOptions.Request = request;

            baseOptions.OnSuccess = delegate(UndeleteResponse response)
            {
                if (callback != null)
                    callback(response);
                self.element.TriggerHandler("ondatachange", new object[] { 
                    new DataChangeInfo { EntityId = self.EntityId, Entity = this.entity, Type = "undelete" }
                });
            };

            var thisOptions = GetUndeleteOptions(callback);
            var finalOptions = jQuery.ExtendObject(baseOptions, thisOptions);

            //if ( != null && EntityObject._undeleteHandler != null)
            //    EntityObject._undeleteHandler(this, finalOptions, callback);
            //else
            UndeleteHandler(finalOptions, callback);
        }

    }
}