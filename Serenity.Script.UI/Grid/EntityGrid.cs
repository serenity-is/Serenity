using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Serenity
{
    public abstract class EntityGrid<TEntity, TOptions> : DataGrid<TEntity, TOptions>
        where TEntity: class, new()
        where TOptions: class, new()
    {
        private string entityType;
        private string entityPlural;
        private string entitySingular;
        private string service;
        private Type entityDialogType;

        public EntityGrid(jQueryObject container, TOptions opt = null)
            : base(container, opt)
        {
        }

        protected override bool UsePager()
        {
            return true;
        }

        protected override void CreateToolbarExtensions()
        {
            CreateIncludeDeletedButton();
            CreateQuickSearchInput();
        }

        protected override string GetInitialTitle()
        {
            return GetEntityPlural();
        }

        protected override string GetLocalTextPrefix()
        {
            if (localTextPrefix == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(LocalTextPrefixAttribute), true);
                if (attributes.Length >= 1)
                    localTextPrefix = attributes[0].As<LocalTextPrefixAttribute>().Value;
                else
                    localTextPrefix = GetEntityType();

                localTextPrefix = ("Db." + localTextPrefix + ".");
            }

            return localTextPrefix;
        }

        protected virtual string GetEntityType()
        {
            if (entityType == null)
            {
                var typeAttributes = this.GetType().GetCustomAttributes(typeof(EntityTypeAttribute), true);
                if (typeAttributes.Length == 1)
                {
                    entityType = typeAttributes[0].As<EntityTypeAttribute>().Value;
                    return entityType;
                }

                // typeof(TEntity).Name'i kullanamayız, TEntity genelde Serializable ve Imported olduğundan dolayı tipi Object e karşılık geliyor!

                // remove global namespace
                var name = this.GetType().FullName;
                var px = name.IndexOf(".");
                if (px >= 0)
                    name = name.Substring(px + 1);

                if (name.EndsWith("Grid"))
                    name = name.Substr(0, name.Length - 4);
                else if (name.EndsWith("SubGrid"))
                    name = name.Substr(0, name.Length - 7);

                entityType = name;
            }

            return entityType;
        }

        protected virtual string GetEntityPlural()
        {
            if (entityPlural == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), true);
                if (attributes.Length >= 1)
                {
                    entityPlural = attributes[0].As<DisplayNameAttribute>().DisplayName;
                    entityPlural = LocalText.GetDefault(entityPlural, entityPlural);
                }
                else
                    entityPlural = (Q.TryGetText(GetLocalTextPrefix() + "EntityPlural") ?? GetEntityType());
            }

            return entityPlural;
        }

        protected virtual string GetEntitySingular()
        {
            if (entitySingular == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(ItemNameAttribute), true);
                if (attributes.Length >= 1)
                {
                    entitySingular = attributes[0].As<ItemNameAttribute>().Value;
                    entitySingular = LocalText.GetDefault(entitySingular, entitySingular);
                }
                else
                    entitySingular = (Q.TryGetText(GetLocalTextPrefix() + "EntitySingular") ?? GetEntityType());
            }

            return entitySingular;
        }

        protected override string GetAddButtonCaption()
        {
            return String.Format(Texts.Controls.EntityGrid.NewButton, GetEntitySingular());
        }

        protected override List<ToolButton> GetButtons()
        {
            var self = this;
            var buttons = new List<ToolButton>();

            buttons.Add(new ToolButton
            {
                Title = GetAddButtonCaption(),
                CssClass = "add-button",
                OnClick = delegate {
                    self.AddButtonClick();
                }
            });

            buttons.Add(NewRefreshButton());

            return buttons;
        }

        protected ToolButton NewRefreshButton(bool noText = false)
        {
            var self = this;

            return new ToolButton
            {
                Title = noText ? null : Texts.Controls.EntityGrid.RefreshButton.Get(),
                Hint = noText ? Texts.Controls.EntityGrid.RefreshButton.Get() : null,
                CssClass = "refresh-button",
                OnClick = delegate
                {
                    self.Refresh();
                }
            };
        }

        protected virtual void AddButtonClick()
        {
            EditItem(new TEntity());
        }

        protected override void EditItem(object entityOrId)
        {
            CreateEntityDialog(GetItemType(), dlg =>
            {
                var dialog = dlg as IEditDialog;
                if (dialog != null)
                {
                    dialog.Load(entityOrId, () => dialog.DialogOpen(), null);
                    return;
                }

                throw new InvalidOperationException(String.Format(
                    "{0} doesn't implement IEditDialog!", dlg.GetType().FullName));
            });
        }

        protected override void EditItem(string itemType, object entityOrId)
        {
            if (itemType == GetItemType())
            {
                EditItem(entityOrId);
                return;
            }

            CreateEntityDialog(itemType, dlg =>
            {
                var dialog = dlg as IEditDialog;
                if (dialog != null)
                {
                    dialog.Load(entityOrId, () => dialog.DialogOpen(), null);
                    return;
                }

                throw new InvalidOperationException(String.Format(
                    "{0} doesn't implement IEditDialog!", dlg.GetType().FullName));
            });
        }

        protected virtual string GetService()
        {
            if (service == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(ServiceAttribute), true);
                if (attributes.Length >= 1)
                    service = attributes[0].As<ServiceAttribute>().Value;
                else
                    service = this.GetEntityType().Replace('.', '/');
            }

            return service;
        }

        protected override SlickRemoteViewOptions GetViewOptions()
        {
            var opt = base.GetViewOptions();
            opt.Url = Q.ResolveUrl("~/Services/" + GetService() + "/List");
            return opt;
        }

        protected override string GetItemType()
        {
            return GetEntityType();
        }

        protected virtual void InitEntityDialog(Widget dialog)
        {
            var self = this;
            dialog.BindToDataChange(this, (e, dci) => self.SubDialogDataChange());
        }

        protected virtual void InitEntityDialog(string itemType, Widget dialog)
        {
            if (itemType == GetItemType())
            {
                InitEntityDialog(dialog);
                return;
            }

            var self = this;
            dialog.BindToDataChange(this, (e, dci) => self.SubDialogDataChange());
        }

        protected virtual Widget CreateEntityDialog(string itemType, Action<Widget> callback)
        {
            var dialogClass = GetEntityDialogType(itemType);

            var dialog = Serenity.Widget.CreateOfType(
                widgetType: dialogClass,
                options: (object)GetEntityDialogOptions(itemType),
                init: (d) =>
                {
                    InitEntityDialog(itemType, d);
                    if (callback != null)
                        callback(d);
                });

            return dialog;
        }

        protected virtual dynamic GetEntityDialogOptions()
        {
            return new { };
        }

        protected virtual dynamic GetEntityDialogOptions(string itemType)
        {
            if (itemType == GetItemType())
                return GetEntityDialogOptions();

            return new { };
        }

        protected virtual Type GetEntityDialogType(string itemType)
        {
            if (itemType == GetItemType())
                return GetEntityDialogType();

            return DialogTypeRegistry.Get(itemType);
        }

        protected virtual Type GetEntityDialogType()
        {
            if (entityDialogType == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(DialogTypeAttribute), true);
                if (attributes.Length >= 1)
                    entityDialogType = attributes[0].As<DialogTypeAttribute>().Value;
                else
                {
                    entityDialogType = DialogTypeRegistry.Get(GetEntityType());
                }
            }

            return entityDialogType;
        }
    }

    public abstract class EntityGrid<TEntity> : EntityGrid<TEntity, object>
        where TEntity : class, new()
    {
        public EntityGrid(jQueryObject container)
            : base(container)
        {
        }
    }
}