using jQueryApi;
using jQueryApi.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Serenity
{
    [Imported, Serializable]
    public class DialogButton
    {
        public string Text { get; set; }
        public Action Click { get; set; }
    }

    [IncludeGenericArguments(false), ScriptName("PropertyDialog")]
    public abstract partial class PropertyDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        private TEntity entity;
        private Int64? entityId;

        protected PropertyDialog(TOptions opt = null)
            : base(opt)
        {
            if (!IsAsyncWidget())
            {
                #pragma warning disable 618
                InitPropertyGrid();
                LoadInitialEntity();
                #pragma warning restore 618
            }
        }

        protected override Promise InitializeAsync()
        {
            return base.InitializeAsync()
                .ThenAwait(InitPropertyGridAsync)
                .Then(() =>
                {
                    LoadInitialEntity();
                });
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
            return opt;
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
                    Text = Q.Text("Dialogs.OkButton"),
                    Click = OkClick,
                },
                new DialogButton {
                    Text = Q.Text("Dialogs.CancelButton"),
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
            set { entity = value ?? new object().As<TEntity>(); }
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

        protected PropertyGrid propertyGrid;

        [Obsolete("Prefer async version")]
        private void InitPropertyGrid()
        {
            var pgDiv = this.ById("PropertyGrid");
            if (pgDiv.Length <= 0)
                return;

            #pragma warning disable 618
            var pgOptions = GetPropertyGridOptions();
            #pragma warning restore 618

            propertyGrid = new PropertyGrid(pgDiv, pgOptions).Init();
            if (this.element.Closest(".ui-dialog").HasClass("s-Flexify"))
                propertyGrid.Element.Children(".categories").FlexHeightOnly();
        }

        private Promise InitPropertyGridAsync()
        {
            return Promise.Void.ThenAwait(() =>
            {
                var pgDiv = this.ById("PropertyGrid");
                if (pgDiv.Length <= 0)
                    return Promise.Void;

                return GetPropertyGridOptionsAsync()
                    .ThenAwait(pgOptions =>
                    {
                        propertyGrid = new PropertyGrid(pgDiv, pgOptions);
                        if (this.element.Closest(".ui-dialog").HasClass("s-Flexify"))
                            propertyGrid.Element.Children(".categories").FlexHeightOnly();
                        return propertyGrid.Initialize();
                    });
            });
        }

        protected virtual string GetFormKey()
        {
            var attributes = this.GetType().GetCustomAttributes(typeof(FormKeyAttribute), true);
            if (attributes.Length >= 1)
                return attributes[0].As<FormKeyAttribute>().Value;
            else
            {
                var name = this.GetType().FullName;
                var px = name.IndexOf(".");
                if (px >= 0)
                    name = name.Substring(px + 1);

                if (name.EndsWith("Dialog"))
                    name = name.Substr(0, name.Length - 6);
                else if (name.EndsWith("Panel"))
                    name = name.Substr(0, name.Length - 5);

                return name;
            }
        }

        protected virtual List<PropertyItem> GetPropertyItems()
        {
            #pragma warning disable 618
            var formKey = GetFormKey();
            return Q.GetForm(formKey);
            #pragma warning restore 618
        }

        protected virtual Promise<List<PropertyItem>> GetPropertyItemsAsync()
        {
            return Promise.Void.ThenAwait(() => 
            {
                var formKey = GetFormKey();
                return Q.GetFormAsync(formKey);
            });
        }

        [Obsolete("Prefer async version")]
        protected virtual PropertyGridOptions GetPropertyGridOptions()
        {
            #pragma warning disable 618
            return new PropertyGridOptions
            {
                IdPrefix = this.idPrefix,
                Items = GetPropertyItems(),
                Mode = PropertyGridMode.Insert,
                UseCategories = false,
                LocalTextPrefix = "Forms." + GetFormKey() + "."
            };
            #pragma warning restore 618
        }

        protected virtual Promise<PropertyGridOptions> GetPropertyGridOptionsAsync()
        {
            return GetPropertyItemsAsync().ThenSelect(propertyItems =>
            {
                return new PropertyGridOptions
                {
                    IdPrefix = this.idPrefix,
                    Items = propertyItems,
                    Mode = PropertyGridMode.Insert,
                    UseCategories = false,
                    LocalTextPrefix = "Forms." + GetFormKey() + ".",
                };
            });
        }

        protected virtual bool ValidateBeforeSave()
        {
            return this.validator.ValidateForm();
        }

        protected virtual TEntity GetSaveEntity()
        {
            var entity = new object().As<TEntity>();

            if (this.propertyGrid != null)
                this.propertyGrid.Save(entity);

            return entity;
        }
    }

    [Imported, IncludeGenericArguments(false), ScriptName("PropertyDialog")]
    public abstract partial class PropertyDialog<TEntity> : PropertyDialog<TEntity, object>
        where TEntity : class, new()
    {
        public PropertyDialog()
            : base(null)
        {
        }
    }
}