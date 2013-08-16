using jQueryApi;
using System;

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
            opt.Service = this.entityType.Value.Replace('.', '/') + "/" + (this.IsEditMode ? "Update" : "Create");
            opt.OnSuccess = delegate(ServiceResponse response)
            {
                self.OnSaveSuccess(response);
                if (callback != null)
                    callback(response);
                self.element.Trigger("ondatachange", new object[] { self.IsEditMode ? "update" : "create" });
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
                string idField = entityIdField.Value;
                if (idField != null)
                    Type.SetField(entity, idField, this.EntityId);
            }

            return entity;
        }

        protected virtual SaveRequest<TEntity> GetSaveRequest()
        {
            var entity = GetSaveEntity();

            SaveRequest<TEntity> req = new SaveRequest<TEntity>();
            req.Entity = entity;

            if (!this.IsEditMode)
            {
                // TODO: attachments
                //AttachmentEntity[] att = this.GetPendingAttachments();
                //if (att != null)
                //    ((CreateRequest)(object)req).Attachments = this.GetPendingAttachments();
            }

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
            return ValidationHelper.TriggerSubmit(this.ById("Form"), () => self.ValidateBeforeSave(), delegate
            {
                self.Save_SubmitHandler(callback);
            });
        }

        protected void SaveHandler(ServiceCallOptions options, Action<ServiceResponse> callback)
        {
            Q.ServiceCall(options);
        }
    }
}