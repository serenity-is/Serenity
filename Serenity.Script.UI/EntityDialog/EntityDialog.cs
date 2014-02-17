using jQueryApi.UI.Widgets;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        private TEntity entity;
        private Int64? entityId;
        
        protected EntityDialog(TOptions opt = null)
            : base(opt)
        {
            InitPropertyGrid();
            InitLocalizationGrid();
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

            if (validator != null)
            {
                this.ById("Form").Remove();
                validator = null;
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
            set { entity = value ?? new TEntity(); }
        }

        protected internal Nullable<Int64> EntityId
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
                return "Yeni " + GetEntitySingular();
            else
            {
                string title = (GetEntityNameFieldValue() ?? "") + " - #" + (this.EntityId.As<long>().ToString());
                return GetEntitySingular() + " Düzenle (" + title + ")";
            }
        }

        protected virtual void UpdateTitle()
        {
            element.Dialog().Title = GetEntityTitle();
        }

        /*
        protected void SetInputsDisabled(Element container, bool isDisabled)
        {
            Query inputs = J.Query(container)
                .find(":input")
                .not(".readonly");

            if (isDisabled)
                inputs.attr("disabled", "disabled");
            else
                inputs.removeAttr("disabled");
        }*/

        protected override string GetTemplateName()
        {
            var templateName = base.GetTemplateName();
            
            if (!Q.CanLoadScriptData("Template." + templateName) &&
                Q.CanLoadScriptData("Template.EntityDialog"))
                return "EntityDialog";

            return templateName;
        }

        protected bool IsCloneMode
        {
            get { return EntityId != null && IdExtensions.IsNegativeId(EntityId.Value); }
        }

        protected bool IsEditMode
        {
            get { return EntityId != null && IdExtensions.IsPositiveId(EntityId.Value); }
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

                return IdExtensions.IsNegativeId(value.Value);
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

    public abstract class EntityDialog<TEntity> : EntityDialog<TEntity, object>
        where TEntity : class, new()
    {
        public EntityDialog()
            : base(null)
        {
        }
    }
}