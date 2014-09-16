using jQueryApi;
using jQueryApi.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable]
    public class DialogButton
    {
        public string Text { get; set; }
        public Action Click { get; set; }
    }

    public abstract partial class PropertyDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        private TEntity entity;
        private Int64? entityId;

        protected PropertyDialog()
            : this(Q.NewBodyDiv(), null)
        {
        }

        protected PropertyDialog(TOptions opt)
            : this(Q.NewBodyDiv(), opt)
        {
        }

        protected PropertyDialog(jQueryObject div, TOptions opt)
            : base(div, opt)
        {
            InitPropertyGrid();
            LoadInitialEntity();
        }

        protected virtual void LoadInitialEntity()
        {
            if (propertyGrid != null)
                propertyGrid.Load(new object());
        }

        protected override DialogOptions GetDialogOptions()
        {
            var opt = base.GetDialogOptions();
            opt.Buttons = GetDialogButtons().As<object[]>();
            opt.Width = 400;
            opt.Title = GetDialogTitle();
            return opt;
        }

        protected virtual string GetDialogTitle()
        {
            return "";
        }

        protected virtual void OkClick()
        {
            if (!ValidateBeforeSave())
                return;

            OkClickValidated();
        }

        protected virtual void OkClickValidated()
        {
            this.DialogClose();
        }

        protected virtual void CancelClick()
        {
            this.DialogClose();
        }

        protected virtual List<DialogButton> GetDialogButtons()
        {
            return new List<DialogButton>
            {
                new DialogButton {
                    Text = "Tamam",
                    Click = OkClick,
                },
                new DialogButton {
                    Text = "İptal",
                    Click = CancelClick
                }
            };
        }

        public override void Destroy()
        {
            if (propertyGrid != null)
            {
                propertyGrid.Destroy();
                propertyGrid = null;
            }

            if (validator != null)
            {
                this.ById("Form").Remove();
                validator = null;
            }

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

        protected virtual void UpdateTitle()
        {
        }

        protected override void OnDialogOpen()
        {
            base.OnDialogOpen();
        }

        protected override string GetTemplateName()
        {
            var templateName = base.GetTemplateName();
            
            if (!Q.CanLoadScriptData("Template." + templateName) &&
                Q.CanLoadScriptData("Template.PropertyDialog"))
                return "PropertyDialog";

            return templateName;
        }

        protected PropertyGrid propertyGrid;

        private void InitPropertyGrid()
        {
            var pgDiv = this.ById("PropertyGrid");
            if (pgDiv.Length <= 0)
                return;

            var pgOptions = GetPropertyGridOptions();

            propertyGrid = new PropertyGrid(pgDiv, pgOptions);
        }

        protected virtual string GetFormKey()
        {
            var name = this.GetType().FullName;
            var px = name.IndexOf(".");
            if (px >= 0)
                name = name.Substring(px + 1);

            if (name.EndsWith("Dialog"))
                name = name.Substr(0, name.Length - 6);

            return name;
        }

        protected virtual List<PropertyItem> GetPropertyItems()
        {
            var formKey = GetFormKey();
            return Q.GetForm(formKey);
        }

        protected virtual PropertyGridOptions GetPropertyGridOptions()
        {
            return new PropertyGridOptions
            {
                IdPrefix = this.idPrefix,
                Items = GetPropertyItems(),
                Mode = PropertyGridMode.Insert,
                UseCategories = false,
                LocalTextPrefix = "Forms." + GetFormKey() + "."
            };
        }

        protected virtual bool ValidateBeforeSave()
        {
            return this.validator.ValidateForm();
        }

        protected virtual TEntity GetSaveEntity()
        {
            var entity = new TEntity();

            if (this.propertyGrid != null)
                this.propertyGrid.Save(entity);

            return entity;
        }
    }

    public abstract partial class PropertyDialog<TEntity> : PropertyDialog<TEntity, object>
        where TEntity : class, new()
    {
        public PropertyDialog()
            : base(null)
        {
        }
    }
}