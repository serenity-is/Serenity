using jQueryApi;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public abstract class EntityGrid<TEntity, TOptions> : DataGrid<TEntity, TOptions>
        where TOptions: GridOptions, new()
        where TEntity: class, new()
    {
        private string entityType;
        private string entityPlural;
        private string entitySingular;
        private Type entityDialogType;

        public EntityGrid(jQueryObject container, TOptions opt)
            : base(container, opt)
        {
        }

        protected override void CreateToolbarExtensions()
        {
            CreateIncludeDeletedButton();
            CreateQuickSearchInput();
        }

        protected override string GetTitle()
        {
            if (options.Title != null)
                return options.Title;

            return GetEntityPlural();
        }

        protected override string GetLocalTextPrefix()
        {
            localTextPrefix = localTextPrefix ?? ("Db." + GetEntityType() + ".");
            return localTextPrefix;
        }

        protected virtual string GetEntityType()
        {
            if (entityType == null)
            {
                var typeAttributes = this.GetType().GetCustomAttributes(typeof(EntityTypeAttribute), true);
                if (typeAttributes.Length == 1)
                {
                    entityType = typeAttributes[0].As<EntityTypeAttribute>().EntityType;
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
            entityPlural = entityPlural ?? (Q.TryGetText(GetLocalTextPrefix() + "EntityPlural") ?? GetEntityType());
            return entityPlural;
        }

        protected virtual string GetEntitySingular()
        {
            entitySingular = entitySingular ?? (Q.TryGetText(GetLocalTextPrefix() + "EntitySingular") ?? GetEntityType());
            return entitySingular;
        }

        protected override string GetAddButtonCaption()
        {
            return "Yeni " + GetEntitySingular();
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

            buttons.Add(new ToolButton
            {
                Title = "Yenile",
                CssClass = "refresh-button",
                OnClick = delegate {
                    self.Refresh();
                }
            });

            return buttons;
        }

        protected virtual void AddButtonClick()
        {
            EditItem(new TEntity());
        }

        protected override void EditItem(object entityOrId)
        {
            dynamic dialog = CreateEntityDialog();
            var scriptType = Script.TypeOf(entityOrId);
            if (scriptType == "string" || scriptType == "number")
                dialog.loadByIdAndOpenDialog(entityOrId.As<long>());
            else
            {
                var entity = entityOrId.As<TEntity>() ?? new TEntity();
                dialog.loadEntityAndOpenDialog(entity);
            }
        }

        protected override SlickRemoteViewOptions GetViewOptions()
        {
            var opt = base.GetViewOptions();
            opt.Url = Q.ResolveUrl("~/Services/" + GetEntityType().Replace('.', '/') + "/List");
            return opt;
        }

        protected override string GetItemType()
        {
            return GetEntityType().Replace('.', '-');
        }

        protected virtual void InitEntityDialog(Widget dialog)
        {
            var self = this;
            dialog.BindToDataChange(this, () => self.SubDialogDataChange());
        }

        protected virtual Widget CreateEntityDialog()
        {
            var dialogClass = GetEntityDialogType();
            Widget dialog = Activator.CreateInstance(dialogClass, GetEntityDialogOptions());
            InitEntityDialog(dialog);
            return dialog;
        }

        protected virtual dynamic GetEntityDialogOptions()
        {
            return new { };
        }

        protected virtual Type GetEntityDialogType()
        {
            if (entityDialogType == null)
            {
                var entityClass = GetEntityType();
                string typeName = entityClass + "Dialog";

                Type dialogType = null;
                foreach (var ns in Q.Config.RootNamespaces)
                {
                    dialogType = Type.GetType(ns + "." + typeName);
                    if (dialogType != null)
                        break;
                }

                if (dialogType == null)
                    throw (typeName + " dialog sınıfı bulunamadı!").AsException();

                entityDialogType = dialogType;
            }

            return entityDialogType;
        }
    }
}