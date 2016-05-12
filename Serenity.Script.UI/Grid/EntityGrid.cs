using jQueryApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [IncludeGenericArguments(false), ScriptName("EntityGrid")]
    public abstract class EntityGrid<TEntity, TOptions> : DataGrid<TEntity, TOptions>
        where TEntity: class, new()
        where TOptions: class, new()
    {
        private string entityType;
        private string displayName;
        private string itemName;
        private string service;
        private Type dialogType;

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
            return GetDisplayName();
        }

        protected override string GetLocalTextPrefix()
        {
            var result = base.GetLocalTextPrefix();

            if (result.IsEmptyOrNull())
                return GetEntityType();

            return result;
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

        protected virtual string GetDisplayName()
        {
            if (displayName == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), true);
                if (attributes.Length >= 1)
                {
                    displayName = attributes[0].As<DisplayNameAttribute>().DisplayName;
                    displayName = LocalText.GetDefault(displayName, displayName);
                }
                else
                    displayName = (Q.TryGetText(GetLocalTextDbPrefix() + "EntityPlural") ?? GetEntityType());
            }

            return displayName;
        }

        protected virtual string GetItemName()
        {
            if (itemName == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(ItemNameAttribute), true);
                if (attributes.Length >= 1)
                {
                    itemName = attributes[0].As<ItemNameAttribute>().Value;
                    itemName = LocalText.GetDefault(itemName, itemName);
                }
                else
                    itemName = (Q.TryGetText(GetLocalTextDbPrefix() + "EntitySingular") ?? GetEntityType());
            }

            return itemName;
        }

        protected override string GetAddButtonCaption()
        {
            return String.Format(Q.Text("Controls.EntityGrid.NewButton"), GetItemName());
        }

        protected override List<ToolButton> GetButtons()
        {
            var self = this;
            var buttons = new List<ToolButton>();

            buttons.Add(new ToolButton
            {
                Title = GetAddButtonCaption(),
                CssClass = "add-button",
                Hotkey = "alt+n",
                OnClick = delegate {
                    self.AddButtonClick();
                }
            });

            buttons.Add(NewRefreshButton(noText: true));

            return buttons;
        }

        protected ToolButton NewRefreshButton(bool noText = false)
        {
            var self = this;

            return new ToolButton
            {
                Title = noText ? null : Q.Text("Controls.EntityGrid.RefreshButton"),
                Hint = noText ? Q.Text("Controls.EntityGrid.RefreshButton") : null,
                CssClass = "refresh-button",
                OnClick = delegate
                {
                    self.Refresh();
                }
            };
        }

        protected virtual void AddButtonClick()
        {
            EditItem(new object().As<TEntity>());
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

        protected override void EditItemOfType(string itemType, object entityOrId)
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

        [Obsolete("Use InitDialog"), InlineCode("InitDialog({dialog})")]
        protected void InitEntityDialog(Widget dialog)
        {
            InitDialog(dialog);
        }

        protected virtual void InitDialog(Widget dialog)
        {
            var self = this;
            dialog.BindToDataChange(this, (e, dci) => self.SubDialogDataChange());
        }

        protected virtual void InitEntityDialog(string itemType, Widget dialog)
        {
            if (itemType == GetItemType())
            {
                InitDialog(dialog);
                return;
            }

            var self = this;
            dialog.BindToDataChange(this, (e, dci) => self.SubDialogDataChange());
        }

        protected virtual Widget CreateEntityDialog(string itemType, Action<Widget> callback)
        {
            var dialogClass = GetDialogTypeFor(itemType);

            var dialog = Serenity.Widget.CreateOfType(
                widgetType: dialogClass,
                options: (object)GetDialogOptionsFor(itemType),
                init: (d) =>
                {
                    InitEntityDialog(itemType, d);
                    if (callback != null)
                        callback(d);
                });

            return dialog;
        }

        protected virtual dynamic GetDialogOptions()
        {
            return new { };
        }

        protected virtual dynamic GetDialogOptionsFor(string itemType)
        {
            if (itemType == GetItemType())
                return GetDialogOptions();

            return new { };
        }

        protected virtual Type GetDialogTypeFor(string itemType)
        {
            if (itemType == GetItemType())
                return GetDialogType();

            return DialogTypeRegistry.Get(itemType);
        }

        protected virtual Type GetDialogType()
        {
            if (dialogType == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(DialogTypeAttribute), true);
                if (attributes.Length >= 1)
                    dialogType = attributes[0].As<DialogTypeAttribute>().Value;
                else
                {
                    dialogType = DialogTypeRegistry.Get(GetEntityType());
                }
            }

            return dialogType;
        }
    }

    [Imported, IncludeGenericArguments(false), ScriptName("EntityGrid")]
    public abstract class EntityGrid<TEntity> : EntityGrid<TEntity, object>
        where TEntity : class, new()
    {
        public EntityGrid(jQueryObject container)
            : base(container)
        {
        }
    }
}