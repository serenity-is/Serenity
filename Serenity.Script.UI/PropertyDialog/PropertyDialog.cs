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

        protected PropertyDialog(TOptions opt)
            : base(Q.NewBodyDiv(), opt)
        {
            InitInferences();
            InitPropertyGrid();
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
            return this.entityType.Value;
        }

        protected virtual void OkClick()
        {
            if (!this.validator.ValidateForm())
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
        }

        protected override string GetTemplateName()
        {
            var templateName = base.GetTemplateName();
            
            if (!Q.CanLoadScriptData("Template." + templateName) &&
                Q.CanLoadScriptData("Template.PropertyDialog"))
                return "PropertyDialog";

            return templateName;
        }
    }
}