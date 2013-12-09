using jQueryApi;
using jQueryApi.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Html;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions : class, new()
    {
        public void LoadNewAndOpenDialog()
        {
            LoadResponse(new RetrieveResponse<TEntity>());
            this.element.Dialog().Open();
        }

        public virtual void LoadEntityAndOpenDialog(TEntity entity)
        {
            LoadResponse(new RetrieveResponse<TEntity> { Entity = entity });
            this.element.Dialog().Open();
        }

        public virtual void LoadResponse(RetrieveResponse<TEntity> data)
        {
            data = data ?? new RetrieveResponse<TEntity>();

            OnLoadingData(data);

            var entity = data.Entity ?? new TEntity();

            BeforeLoadEntity(entity);

            LoadEntity(entity);

            Entity = entity;

            AfterLoadEntity();
        }

        protected virtual void LoadEntity(TEntity entity)
        {
            string idField = GetEntityIdField();
            if (idField != null)
                this.EntityId = (long?)(Type.GetField(entity, idField).ConvertToId());

            this.Entity = entity;
            //SetInputsDisabled(this.Form[0], Type.GetField(ent, "IsActive") == (object)false);

            if (this.propertyGrid != null)
            {
                this.propertyGrid.Mode = this.IsEditMode ? PropertyGridMode.Update : PropertyGridMode.Insert;
                this.propertyGrid.Load(entity);
            }

            //this.ResetValidation();
        }

        protected virtual void BeforeLoadEntity(TEntity entity)
        {
        }

        protected virtual void AfterLoadEntity()
        {
            // TODO: düzelt
            //AfterLoadEntity_AttachmentPanel();
            //AfterLoadEntity_AuditLogPanel();
            UpdateInterface();
            UpdateTitle();
        }

        public void LoadByIdAndOpenDialog(long entityId)
        {
            var self = this;
            LoadById(entityId, response => Window.SetTimeout(() => self.element.Dialog().Open(), 0));
        }

        protected virtual void OnLoadingData(RetrieveResponse<TEntity> data)
        {
        }

        protected virtual ServiceCallOptions<RetrieveResponse<TEntity>> GetLoadByIdOptions(long id,
            Action<RetrieveResponse<TEntity>> callback)
        {
            return new ServiceCallOptions<RetrieveResponse<TEntity>>();
        }

        protected virtual RetrieveRequest GetLoadByIdRequest(long id)
        {
            RetrieveRequest request = new RetrieveRequest();
            request.EntityId = id;
            return request;
        }

        public void ReloadById()
        {
            LoadById(this.EntityId.Value, null);
        }

        public void LoadById(long id, Action<RetrieveResponse<TEntity>> callback)
        {
            var baseOptions = new ServiceCallOptions<RetrieveResponse<TEntity>>();
            baseOptions.Service = this.GetEntityType().Replace('.', '/') + "/Retrieve";
            baseOptions.BlockUI = false;

            baseOptions.Request = GetLoadByIdRequest(id);

            var self = this;
            baseOptions.OnSuccess = delegate(RetrieveResponse<TEntity> response)
            {
                self.LoadResponse(response);

                if (callback != null)
                    callback(response);
            };

            baseOptions.OnCleanup = delegate()
            {
                if (self.validator != null)
                    Q.Externals.ValidatorAbortHandler(self.validator);
            };

            var thisOptions = GetLoadByIdOptions(id, callback);
            var finalOptions = jQuery.ExtendObject(baseOptions, thisOptions);

            //if (EntityObject != null && EntityObject._loadByIdHandler != null)
            //    EntityObject._loadByIdHandler(this, finalOptions, callback);
            //else
            LoadByIdHandler(finalOptions, callback);
        }

        protected void LoadByIdHandler(ServiceCallOptions<RetrieveResponse<TEntity>> options, Action<RetrieveResponse<TEntity>> callback)
        {
            Q.ServiceCall(options);
        }
    }
}