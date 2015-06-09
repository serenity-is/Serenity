using jQueryApi;
using jQueryApi.UI.Widgets;
using System;
using System.Collections;
using System.Html;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions : class, new()
    {
        public void Load(object entityOrId, Action done, Action<object> fail)
        {
            fail.TryCatch(delegate()
            {
                if (entityOrId == null)
                {
                    LoadResponse(new RetrieveResponse<TEntity>());
                    done();
                    return;
                }

                var scriptType = Script.TypeOf(entityOrId);
                if (scriptType == "string" || scriptType == "number")
                {
                    var self = this;
                    var entityId = entityOrId.As<long>();
                    LoadById(entityId, response => Window.SetTimeout(done, 0));
                    return;
                }
                 
                var entity = entityOrId.As<TEntity>() ?? new TEntity();
                LoadResponse(new RetrieveResponse<TEntity> { Entity = entity });
                done();
            });
        }

        public void LoadNewAndOpenDialog()
        {
            LoadResponse(new RetrieveResponse<TEntity>());
            
            if (!isPanel)
                this.element.Dialog().Open();
        }

        public virtual void LoadEntityAndOpenDialog(TEntity entity)
        {
            LoadResponse(new RetrieveResponse<TEntity> { Entity = entity });

            if (!isPanel)
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
                this.EntityId = (long?)(entity.As<JsDictionary>()[idField]).ConvertToId();

            this.Entity = entity;

            if (this.propertyGrid != null)
            {
                this.propertyGrid.Mode = this.IsEditMode ? PropertyGridMode.Update : PropertyGridMode.Insert;
                this.propertyGrid.Load(entity);
            }
        }

        protected virtual void BeforeLoadEntity(TEntity entity)
        {
            localizationPendingValue = null;
            localizationLastValue = null;
        }

        protected virtual void AfterLoadEntity()
        {
            UpdateInterface();
            UpdateTitle();
        }

        public void LoadByIdAndOpenDialog(long entityId)
        {
            var self = this;
            LoadById(entityId, response => Window.SetTimeout(() => { if (!isPanel) self.element.Dialog().Open(); }, 0), () =>
            {
                if (!isPanel && !self.Element.Is(":visible"))
                    self.Element.Remove();
            });
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

        public void LoadById(long id, Action<RetrieveResponse<TEntity>> callback, Action fail = null)
        {
            var baseOptions = new ServiceCallOptions<RetrieveResponse<TEntity>>();
            baseOptions.Service = this.GetService() + "/Retrieve";
            baseOptions.BlockUI = true;

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

            LoadByIdHandler(finalOptions, callback, fail);
        }

        protected void LoadByIdHandler(ServiceCallOptions<RetrieveResponse<TEntity>> options, Action<RetrieveResponse<TEntity>> callback, Action fail)
        {
            var request = Q.ServiceCall(options);
            if (fail != null)
                request.Fail(fail);
        }
    }
}