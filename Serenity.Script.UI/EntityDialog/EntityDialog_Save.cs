using jQueryApi;
using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions : class, new()
    {
        protected virtual bool ValidateBeforeSave()
        {
            return true;
        }

        protected virtual ServiceCallOptions GetSaveOptions(Action<ServiceResponse> callback)
        {
            var self = this;
            var opt = new ServiceCallOptions();
            opt.Service = this.GetService() + "/" + (this.IsEditMode ? "Update" : "Create");
            opt.OnSuccess = delegate(ServiceResponse response)
            {
                self.OnSaveSuccess(response);
                if (callback != null)
                    callback(response);

                var dci = new DataChangeInfo
                {
                    Type = self.IsEditMode ? "update" : "create",
                    Entity = (opt.Request ?? new object()).As<SaveRequest<object>>().Entity,
                    EntityId = self.IsEditMode ? this.EntityId : (response ?? new object()).As<SaveResponse>().EntityId
                };

                self.element.TriggerHandler("ondatachange", new object[] { dci });
            };
            opt.OnCleanup = delegate()
            {
                if (self.validator != null)
                    Q.Externals.ValidatorAbortHandler(self.validator);
            };
            opt.Request = GetSaveRequest();
            return opt;
        }

        protected virtual TEntity GetSaveEntity()
        {
            var entity = new TEntity();

            if (this.propertyGrid != null)
                this.propertyGrid.Save(entity);

            if (this.IsEditMode)
            {
                string idField = GetEntityIdField();
                if (idField != null)
                    entity.As<JsDictionary>()[idField] = this.EntityId;
            }

            return entity;
        }

        protected virtual SaveRequest<TEntity> GetSaveRequest()
        {
            var entity = GetSaveEntity();

            SaveRequest<TEntity> req = new SaveRequest<TEntity>();
            req.Entity = entity;

            return req;
        }

        protected virtual void OnSaveSuccess(ServiceResponse response)
        {
        }

        protected virtual void Save_SubmitHandler(Action<ServiceResponse> callback)
        {
            var options = GetSaveOptions(callback);
            SaveHandler(options, callback);
        }

        protected virtual bool Save(Action<ServiceResponse> callback)
        {
            var self = this;
            return ValidationHelper.Submit(this.ById("Form"), () => self.ValidateBeforeSave(), delegate
            {
                self.Save_SubmitHandler(callback);
            });
        }

        protected virtual void SaveHandler(ServiceCallOptions options, Action<ServiceResponse> callback)
        {
            Q.ServiceCall(options);
        }
    }

    [Imported, Serializable]
    public class DataChangeInfo
    {
        public string Type { get; set; }
        public Int64? EntityId { get; set; }
        public object Entity { get; set; }
    }
}