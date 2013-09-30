using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    public abstract class EntityGrid<TEntity, TOptions> : Widget<TOptions>
        where TOptions: GridOptions, new()
        where TEntity: class, new()
    {
        protected Toolbar toolbar;
        protected SlickRemoteView<TEntity> view;
        protected jQueryObject slickContainer;
        protected SlickGrid slickGrid;
        private Lazy<string> entityType;
        private Lazy<string> entityPlural;
        private Lazy<string> entitySingular;
        private Lazy<string> entityIdField;
        private Lazy<string> entityIsActiveField;
        private Lazy<Type> entityDialogType;
        private Lazy<string> localTextPrefix;
        private bool isDisabled;

        public EntityGrid(jQueryObject container, TOptions opt)
            : base(container, opt)
        {
            var self = this;
            entityType = new Lazy<string>(() => self.InferEntityType());
            localTextPrefix = new Lazy<string>(() => self.GetLocalTextPrefix());
            entitySingular = new Lazy<string>(() => self.GetEntitySingular());
            entityPlural = new Lazy<string>(() => self.GetEntityPlural());
            entityIdField = new Lazy<string>(() => self.InferEntityIdField());
            entityIsActiveField = new Lazy<string>(() => self.InferEntityIsActiveField());
            
            entityDialogType = new Lazy<Type>(() => self.GetEntityDialogType());

            this.element.AddClass("s-DataGrid").Html("");
            this.element.AddClass("s-" + this.GetType().Name);
            this.element.Bind("layout", delegate 
            {
                Q.LayoutFillHeight(self.slickContainer);
                self.slickGrid.ResizeCanvas();
            });

            if (options.Height != null)
                this.element.CSS("height", options.Height);

            var title = GetTitle();
            if (title != null)
                CreateTitleBar(title);

            var buttons = GetButtons();
            if (buttons != null)
                CreateToolbar(buttons);

            this.slickContainer = CreateSlickContainer();

            this.view = CreateView();

            this.slickGrid = CreateSlickGrid();

            if (!options.HidePager)
                CreatePager();

            BindToSlickEvents();
            BindToViewEvents();

            if (buttons != null)
            {
                CreateIncludeDeletedButton();
                CreateQuickSearchInput();
            }

            UpdateDisabledState();

            if (options.PopulateWhenVisible)
            {
                LazyLoadHelper.ExecuteEverytimeWhenShown(element, () => self.RefreshIfNeeded(), false);
                if (element.Is(":visible"))
                    view.Populate();
            }
            else
                view.Populate();
        }

        public override void Destroy()
        {
            if (this.toolbar != null)
            {
                this.toolbar.Destroy();
                this.toolbar = null;
            }

            if (this.slickGrid != null)
            {
                this.slickGrid.SetData((new object[0]).As<List<dynamic>>(), false);
                this.slickGrid.Destroy();
                this.slickGrid = null;
            }

            if (this.view != null)
            {
                this.view.OnRowsChanged.Clear();
                this.view.OnRowCountChanged.Clear();
                this.view.OnSubmit = null;
                this.view.SetFilter(null);
                dynamic viewRows = this.view.Rows;
                if (viewRows != null)
                    viewRows.getItemMetadata = null;
                this.view = null;
            }

            base.Destroy();
        }

        protected virtual string GetTitle()
        {
            if (options.Title != null)
                return options.Title;

            return entityPlural.Value;
        }

        protected virtual dynamic GetItemMetadata(TEntity entity, int index)
        {
            var value = Type.GetField(entity, entityIsActiveField.Value).As<Int32?>();
            if (value == null)
                return false;

            if (Type.GetScriptType(value) == "number")
            {
                if (IdExtensions.IsNegativeId(value.As<Int64>()))
                    return new { cssClasses = "deleted" };
                else if (value.As<Int32>() == 0)
                    return new { cssClasses = "inactive" };
            }
            else if (Type.GetScriptType(value) == "boolean")
            {
                if (value.As<Boolean>() == false)
                    return new { cssClasses = "deleted" };
            }

            return new object();
        }

        protected virtual SlickGrid CreateSlickGrid()
        {
            var slickColumns = GetColumns().SetDefaults(localTextPrefix: localTextPrefix.Value);
            var slickOptions = GetSlickOptions();
            var self = this;
            dynamic viewRows = view.Rows;
            viewRows.getItemMetadata = new Func<int, dynamic>(delegate(int index)
            {
                var item = self.view.Rows[index];
                return self.GetItemMetadata(item, index);
            });

            return new SlickGrid(slickContainer, data: viewRows, columns: slickColumns, options: slickOptions);
        }

        protected virtual void CreateIncludeDeletedButton()
        {
            GridUtils.AddIncludeDeletedToggle(toolbar.Element, view);
        }

        protected virtual void CreateQuickSearchInput()
        {
            GridUtils.AddQuickSearchInput(toolbar.Element, view);
        }

        public List<TEntity> Items
        {
            get
            {
                return view.GetItems().As<List<TEntity>>();
            }
            set
            {
                view.SetItems(value, true);
            }
        }

        protected virtual string GetLocalTextPrefix()
        {
            return "Db." + entityType.Value + ".";
        }

        protected virtual string InferEntityType()
        {
            var typeAttributes = this.GetType().GetCustomAttributes(typeof(EntityTypeAttribute), true);
            if (typeAttributes.Length == 1)
                return typeAttributes[0].As<EntityTypeAttribute>().EntityType;

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

            return name;
        }

        protected virtual string GetEntityPlural()
        {
            return Q.TryGetText(localTextPrefix.Value + "EntityPlural") ?? entityType.Value;
        }

        protected virtual string GetEntitySingular()
        {
            return Q.TryGetText(localTextPrefix.Value + "EntitySingular") ?? entityType.Value;
        }

        protected virtual string InferEntityNameField()
        {
            var attributes = this.GetType().GetCustomAttributes(typeof(NamePropertyAttribute), true);
            if (attributes.Length == 1)
                return attributes[0].As<NamePropertyAttribute>().NameProperty;

            return "Name";
        }

        protected virtual string InferEntityIdField()
        {
            var attributes = this.GetType().GetCustomAttributes(typeof(IdPropertyAttribute), true);
            if (attributes.Length == 1)
                return attributes[0].As<IdPropertyAttribute>().IdProperty;

            return "ID";
        }

        protected void CreatePager()
        {
            var pagerDiv = jQuery.FromHtml("<div></div>")
                .AppendTo(this.element);

            ((dynamic)pagerDiv).slickPager(new { 
                view = this.view.As<object>(), 
                rowsPerPage = 20, 
                rowsPerPageOptions = new int[] { 20, 100, 500, 2500 } 
            });
        }

        protected void BindToSlickEvents()
        {
            var self = this;
            this.slickGrid.OnSort.Subscribe((e, p) => {
                self.view.PopulateLock();
                try
                {
                    self.view.SortOrder = p.sortAsc ? "" : "desc";
                    self.view.SortBy = (p.sortCol ?? new SlickColumn()).field;
                }
                finally
                {
                    self.view.PopulateUnlock();
                }
                self.view.Populate();
            });

            this.slickGrid.OnClick.Subscribe((e, p) =>
            {
                self.OnClick(e, p.row, p.cell);
            });
        }

        protected virtual string GetAddButtonCaption()
        {
            return "Yeni " + entitySingular.Value;
        }

        protected virtual List<ToolButton> GetButtons()
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

        protected virtual void EditItem(object entityOrId)
        {
            dynamic dialog = CreateEntityDialog();
            var scriptType = Type.GetScriptType(entityOrId);
            if (scriptType == "string" || scriptType == "number")
                dialog.loadByIdAndOpenDialog(entityOrId.As<long>());
            else
            {
                var entity = entityOrId.As<TEntity>() ?? new TEntity();
                dialog.loadEntityAndOpenDialog(entity);
            }
        }

        protected virtual void OnClick(jQueryEvent e, int row, int cell)
        {
            var target = jQuery.FromElement(e.Target);
            if (target.HasClass(SlickFormatting.ItemLinkClass(GetItemClass())))
            {
                e.PreventDefault();
                EditItem(SlickFormatting.GetItemId(target));
            }
        }

        private void ViewRowCountChanged(jQueryEvent e, dynamic d)
        {
            this.slickGrid.UpdateRowCount();
            this.slickGrid.Render();
        }

        private void ViewRowsChanged(jQueryEvent e, dynamic rows)
        {
            if (rows == null)
            {
                this.slickGrid.Invalidate();
                MarkupReady();
            }
            else
            {
                this.slickGrid.Invalidate();
                this.slickGrid.Render();
            }
        }

        protected void BindToViewEvents()
        {
            var self = this;
            this.view.OnRowCountChanged.Subscribe((e, d) => self.ViewRowCountChanged(e, d));
            this.view.OnRowsChanged.Subscribe((e, d) => self.ViewRowsChanged(e, d));
            this.view.OnSubmit = (view) => self.OnViewSubmit();
            this.view.SetFilter((item, view) => self.OnViewFilter(item));
            this.view.OnProcessData = ((response, view) => self.OnViewProcessData(response));
        }

        protected virtual ListResponse<TEntity> OnViewProcessData(ListResponse<TEntity> response)
        {
            return response;
        }

        protected virtual bool OnViewFilter(TEntity item)
        {
            return true;
        }

        protected virtual void GetIncludeColumns(JsDictionary<string, bool> include)
        {
            foreach (var column in slickGrid.GetColumns())
                if (column.Field != null)
                    include[column.Field] = true;
        }

        protected virtual void SetIncludeColumnsParameter()
        {
            var include = new JsDictionary<string, bool>();
            if (!Script.IsNullOrUndefined(view.Params.IncludeColumns))
                foreach (var key in (string[])view.Params.IncludeColumns)
                    include[key] = true;

            GetIncludeColumns(include);

            List<string> array = null;
            if (include.Count > 0)
            {
                array = new List<string>();
                foreach (var key in include.Keys)
                    array.Add(key);
                
            }

            view.Params.IncludeColumns = array;
        }

        protected virtual bool OnViewSubmit()
        {
            if (IsDisabled || !GetGridCanLoad())
            {
                view.SetItems(new List<TEntity>(), true);
                return false;
            }

            SetIncludeColumnsParameter();

            return true;
        }

        protected virtual void MarkupReady()
        {
        }

        protected jQueryObject CreateSlickContainer()
        {
            return jQuery.FromHtml("<div class=\"grid-container\"></div>")
                .AppendTo(this.element);
        }

        protected SlickRemoteView<TEntity> CreateView()
        {
            var opt = GetViewOptions();
            return new SlickRemoteView<TEntity>(opt);
        }

        protected virtual string GetDefaultSortBy()
        {
            return "ID";
        }

        protected virtual SlickRemoteViewOptions GetViewOptions()
        {
            var opt = new SlickRemoteViewOptions();
            opt.IdField = entityIdField.Value;
            opt.Url = Q.ResolveUrl("~/Services/" + entityType.Value.Replace('.', '/') + "/List");
            opt.SortBy = GetDefaultSortBy();
            
            if (options.HidePager)
                opt.RowsPerPage = 0;
            else
                opt.RowsPerPage = 20;

            return opt;
        }

        protected void CreateToolbar(List<ToolButton> buttons)
        {
            var toolbarDiv = jQuery.FromHtml("<div class=\"grid-toolbar\"></div>")
                .AppendTo(this.Element);
            toolbar = new Toolbar(toolbarDiv, new ToolbarOptions
            {
                Buttons = buttons
            });
        }

        protected void CreateTitleBar(string title)
        {
            var titleDiv = jQuery.FromHtml("<div class=\"grid-title\"><div class=\"title-text\"></div></div>")
                .AppendTo(this.Element);
            titleDiv.Children().Text(title);
        }

        protected virtual string GetItemClass()
        {
            return entityType.Value.Replace('.', '-');
        }

        protected SlickFormatter ItemLink(string itemClass = null, string idField = null,
            Func<SlickFormatterContext, string> text = null)
        {
            itemClass = itemClass ?? entityType.Value.Replace('.', '-');
            idField = idField ?? entityIdField.Value;

            return text == null ?
                SlickFormatting.ItemLink(itemClass, idField) :
                SlickFormatting.ItemLink(itemClass, idField, text);
        }

        protected virtual List<SlickColumn> GetColumns()
        {
            return new List<SlickColumn>();
        }

        protected virtual SlickGridOptions GetSlickOptions()
        {
            SlickGridOptions opt = new SlickGridOptions();
            opt.MultiSelect = false;
            opt.EnableCellNavigation = false;
            return opt;
        }

        public void PopulateLock()
        {
            view.PopulateLock();
        }

        public void PopulateUnlock()
        {
            view.PopulateUnlock();
        }

        protected virtual bool GetGridCanLoad()
        {
            return true;
        }

        public void Refresh()
        {
            if (!options.PopulateWhenVisible)
            {
                InternalRefresh();
                return;
            }

            if (slickContainer.Is(":visible"))
            {
                slickContainer.Data("needsRefresh", false);
                InternalRefresh();
                return;
            }

            slickContainer.Data("needsRefresh", true);
        }

        private void RefreshIfNeeded()
        {
            if (Q.IsTrue(slickContainer.GetDataValue("needsRefresh")))
            {
                slickContainer.Data("needsRefresh", false);
                InternalRefresh();
            }
        }

        protected virtual void InternalRefresh()
        {
            view.Populate();
        }

        public bool IsDisabled
        {
            get { return isDisabled; }
            set
            {
                if (isDisabled != value)
                {
                    isDisabled = value;
                    if (isDisabled)
                        view.SetItems(new List<TEntity>(), true);
                    UpdateDisabledState();
                }
            }
        }

        protected virtual void UpdateDisabledState()
        {
            slickContainer.ToggleClass("ui-state-disabled", Q.IsTrue(IsDisabled));
        }

        public void ResizeCanvas()
        {
            slickGrid.ResizeCanvas();
        }

        protected virtual void SubDialogDataChange()
        {
            this.Refresh();
        }

        protected virtual void InitEntityDialog(Widget dialog)
        {
            var self = this;
            dialog.BindToDataChange(this, () => self.SubDialogDataChange());
        }

        protected virtual Widget CreateEntityDialog()
        {
            var dialogClass = entityDialogType.Value;
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
            var entityClass = entityType.Value;
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

            return dialogType;
        }

        protected virtual string InferEntityIsActiveField()
        {
            var attributes = this.GetType().GetCustomAttributes(typeof(IsActivePropertyAttribute), true);
            if (attributes.Length == 1)
                return attributes[0].As<IsActivePropertyAttribute>().IsActiveProperty;

            return "IsActive";
        }

        public SlickRemoteView<TEntity> View { get { return view; } }
    }

    [Imported, Serializable]
    public class GridOptions
    {
        public string Title { get; set; }
        public string Height { get; set; }
        public bool HidePager { get; set; }
        public bool PopulateWhenVisible { get; set; }
    }
}