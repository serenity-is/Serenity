using System;
using System.Collections;
using jQueryApi.UI.Widgets;
using jQueryApi;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public interface IEditDialog : IDialog
    {
        void Load(object entityOrId, Action done, Action<object> fail);
    }

    [IncludeGenericArguments(false), ScriptName("EntityDialog")]
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>, IEditDialog
        where TEntity : class, new()
        where TOptions: class, new()
    {
        private TEntity entity;
        private object entityId;
        
        protected EntityDialog(TOptions opt = null)
            : base(opt)
        {
            if (!IsAsyncWidget())
            {
                #pragma warning disable 618
                InitPropertyGrid();
                InitLocalizationGrid();
                #pragma warning restore 618
            }
        }

        protected override Promise InitializeAsync()
        {
            return base.InitializeAsync()
                .ThenAwait(InitPropertyGridAsync)
                .ThenAwait(InitLocalizationGridAsync);
        }

        public override void Destroy()
        {
            if (propertyGrid != null)
            {
                propertyGrid.Destroy();
                propertyGrid = null;
            }

            if (localizationGrid != null)
            {
                localizationGrid.Destroy();
                localizationGrid = null;
            }

            this.undeleteButton = null;
            this.applyChangesButton = null;
            this.deleteButton = null;
            this.saveAndCloseButton = null;

            base.Destroy();
        }

        protected TEntity Entity
        {
            get { return entity; }
            set { entity = value ?? new object().As<TEntity>(); }
        }

        protected internal object EntityId
        {
            get { return entityId; }
            set { entityId = value; }
        }

        protected virtual string GetEntityNameFieldValue()
        {
            return (Entity.As<JsDictionary>()[GetEntityNameField()] ?? "").ToString();
        }

        protected virtual string GetEntityTitle()
        {
            if (!(this.IsEditMode))
                return String.Format(Q.Text("Controls.EntityDialog.NewRecordTitle"), GetEntitySingular());
            else
            {
                string title = (GetEntityNameFieldValue() ?? "");
                return String.Format(Q.Text("Controls.EntityDialog.EditRecordTitle"), GetEntitySingular(),
                    (title.IsEmptyOrNull() ? "" : " (" + title + ")"));
            }
        }

        protected virtual void UpdateTitle()
        {
            if (!isPanel)
                element.Dialog().Title = GetEntityTitle();
        }

        protected virtual bool IsCloneMode
        {
            get { return false; }
        }

        protected bool IsEditMode
        {
            get { return EntityId != null && !IsCloneMode; }
        }

        protected bool IsDeleted
        {
            get 
            { 
                if (EntityId == null)
                    return false;

                var value = Entity.As<JsDictionary>()[GetEntityIsActiveField()].As<Int32?>();
                if (value == null)
                    return false;

                return value < 0;
            }
        }

        protected bool IsNew
        {
            get { return EntityId == null; }
        }

        protected bool IsNewOrDeleted
        {
            get { return IsNew || this.IsDeleted; }
        }
    }

    [Imported, IncludeGenericArguments(false), ScriptName("EntityDialog")]
    public abstract class EntityDialog<TEntity> : EntityDialog<TEntity, object>
        where TEntity : class, new()
    {
        protected EntityDialog()
            : base(null)
        {
        }
    }
}