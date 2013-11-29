using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    public interface IEntityGrid
    {
        SlickGrid SlickGrid { get; }
    }

    public abstract class EntityGrid<TEntity, TOptions> : Widget<TOptions>, IEntityGrid
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
            this.element.AddClass("require-layout").Bind("layout", delegate 
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
                CreateToolbarExtensions();

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

        protected virtual void CreateToolbarExtensions()
        {
            CreateIncludeDeletedButton();
            CreateQuickSearchInput();
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
                if (this.slickGridOnSort != null)
                {
                    this.slickGrid.OnSort.Unsubscribe(this.slickGridOnSort);
                    this.slickGridOnSort = null;
                }

                if (this.slickGridOnClick != null)
                {
                    this.slickGrid.OnSort.Unsubscribe(this.slickGridOnClick);
                    this.slickGridOnClick = null;
                }

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
                return new object();

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

            var grid = new SlickGrid(slickContainer, data: viewRows, columns: slickColumns, options: slickOptions);

            grid.RegisterPlugin(new SlickAutoTooltips(new SlickAutoTooltipsOptions
            {
                EnableForHeaderCells = true
            }));

            grid.SetSortColumns(GetDefaultSortBy().Map<SlickColumnSort>(s => {
                var x = new SlickColumnSort();
                if (s != null && s.ToLower().EndsWith(" DESC"))
                {
                    x.ColumnId = s.Substr(0, s.Length - 5);
                    x.SortAsc = false;
                }
                else
                {
                    x.ColumnId = s;
                    x.SortAsc = true;
                }
                return x;
            }));

            return grid;
        }

        protected virtual void CreateIncludeDeletedButton()
        {
            GridUtils.AddIncludeDeletedToggle(toolbar.Element, view);
        }

        protected virtual List<QuickSearchField> GetQuickSearchFields()
        {
            return null;
        }

        protected virtual void CreateQuickSearchInput()
        {
            GridUtils.AddQuickSearchInput(toolbar.Element, view, GetQuickSearchFields());
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
            var pagerDiv = J("<div></div>")
                .AppendTo(this.element);

            ((dynamic)pagerDiv).slickPager(new { 
                view = this.view.As<object>(), 
                rowsPerPage = 20, 
                rowsPerPageOptions = new int[] { 20, 100, 500, 2500 } 
            });
        }

        private Action<jQueryEvent, dynamic> slickGridOnSort;
        private Action<jQueryEvent, dynamic> slickGridOnClick;

        protected void BindToSlickEvents()
        {
            var self = this;
            slickGridOnSort = (e, p) =>
            {
                self.view.PopulateLock();
                try
                {
                    List<string> sortBy = new List<string>();
                    if (p.multiColumnSort)
                    {
                        for (var i = 0; i < p.sortCols.length; i++)
                        {
                            var x = p.sortCols[i];
                            var col = x.sortCol ?? new SlickColumn();
                            sortBy.Add(col.field + (x.sortAsc ? "" : " DESC"));
                        }
                    }
                    else
                    {
                        var col = p.sortCol ?? new SlickColumn();
                        sortBy.Add(col.field + (p.sortAsc ? "" : " DESC"));
                    }

                    self.view.SortBy = sortBy.As<string[]>();
                }
                finally
                {
                    self.view.PopulateUnlock();
                }
                self.view.Populate();
            };

            this.slickGrid.OnSort.Subscribe(slickGridOnSort);

            slickGridOnClick = (e, p) =>
            {
                self.OnClick(e, p.row, p.cell);
            };

            this.slickGrid.OnClick.Subscribe(slickGridOnClick);
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
            var target = J(e.Target);
            if (target.HasClass("s-" + GetItemType() + "Link"))
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
            return J("<div class=\"grid-container\"></div>")
                .AppendTo(this.element);
        }

        protected SlickRemoteView<TEntity> CreateView()
        {
            var opt = GetViewOptions();
            return new SlickRemoteView<TEntity>(opt);
        }

        protected virtual List<string> GetDefaultSortBy()
        {
            return new List<string> { entityIdField.Value };
        }

        protected virtual SlickRemoteViewOptions GetViewOptions()
        {
            var opt = new SlickRemoteViewOptions();
            opt.IdField = entityIdField.Value;
            opt.Url = Q.ResolveUrl("~/Services/" + entityType.Value.Replace('.', '/') + "/List");
            opt.SortBy = GetDefaultSortBy().As<string[]>();
            
            if (options.HidePager)
                opt.RowsPerPage = 0;
            else
                opt.RowsPerPage = 100;

            return opt;
        }

        protected void CreateToolbar(List<ToolButton> buttons)
        {
            var toolbarDiv = J("<div class=\"grid-toolbar\"></div>")
                .AppendTo(this.Element);
            toolbar = new Toolbar(toolbarDiv, new ToolbarOptions
            {
                Buttons = buttons
            });
        }

        protected void CreateTitleBar(string title)
        {
            var titleDiv = J("<div class=\"grid-title\"><div class=\"title-text\"></div></div>")
                .AppendTo(this.Element);
            titleDiv.Children().Text(title);
        }

        protected virtual string GetItemType()
        {
            return entityType.Value.Replace('.', '-');
        }

        protected SlickFormatter ItemLink(string itemType = null, string idField = null,
            Func<SlickFormatterContext, string> text = null, Func<SlickFormatterContext, string> cssClass = null)
        {
            itemType = itemType ?? GetItemType();
            idField = idField ?? entityIdField.Value;

            return SlickFormatting.ItemLink(itemType, idField, text, cssClass);
        }

        protected virtual List<SlickColumn> GetColumns()
        {
            return new List<SlickColumn>();
        }

        protected virtual SlickGridOptions GetSlickOptions()
        {
            SlickGridOptions opt = new SlickGridOptions();
            opt.MultiSelect = false;
            opt.MultiColumnSort = true;
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
        public SlickGrid SlickGrid { get { return slickGrid; } }
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