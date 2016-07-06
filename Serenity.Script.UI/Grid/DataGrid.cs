using jQueryApi;
using Serenity.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.WebStorage;
using System.Html;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [IncludeGenericArguments(false), ScriptName("DataGrid")]
    public abstract class DataGrid<TItem, TOptions> : Widget<TOptions>, IDataGrid
        where TItem : class, new()
        where TOptions : class, new()
    {
        protected jQueryObject titleDiv;
        protected Toolbar toolbar;
        protected FilterDisplayBar filterBar;
        protected jQueryObject quickFiltersDiv;
        protected SlickRemoteView<TItem> view;
        protected jQueryObject slickContainer;
        protected SlickGrid slickGrid;
        protected List<SlickColumn> allColumns;
        protected PersistedGridSettings initialSettings;
        protected int restoringSettings;
        private string idProperty;
        private string isActiveProperty;
        private string localTextDbPrefix;
        private bool isDisabled;
        protected event Action submitHandlers;

        static DataGrid()
        {
            DefaultRowHeight = 27;
            DefaultHeaderHeight = 30;
        }

        public DataGrid(jQueryObject container, TOptions opt = null)
            : base(container, opt)
        {
            var self = this;

            this.element.AddClass("s-DataGrid").Html("");
            this.element.AddClass("s-" + this.GetType().Name);
            this.element.AddClass("require-layout").Bind("layout", delegate
            {
                self.Layout();
            });

            Title = GetInitialTitle();

            var buttons = GetButtons();
            if (buttons != null)
                CreateToolbar(buttons);

            this.slickContainer = CreateSlickContainer();

            this.view = CreateView();

            this.slickGrid = CreateSlickGrid();

            if (EnableFiltering())
                CreateFilterBar();

            if (UsePager())
                CreatePager();

            BindToSlickEvents();
            BindToViewEvents();

            if (buttons != null)
                CreateToolbarExtensions();

            CreateQuickFilters();

            UpdateDisabledState();

            if (!IsAsyncWidget())
            {
                initialSettings = GetCurrentSettings();
                RestoreSettings();
                InitialPopulate();
            }
        }

        protected void Layout()
        {
            if (!this.element.Is(":visible"))
                return;

            if (this.slickContainer == null)
                return;

            Q.LayoutFillHeight(this.slickContainer);

            if (this.element.HasClass("responsive-height"))
            {

                if (this.slickGrid != null && this.slickGrid.GetOptions().AutoHeight)
                {
                    this.slickContainer.Children(".slick-viewport").CSS("height", "auto");
                    this.slickGrid.SetOptions(new SlickGridOptions { AutoHeight = false });
                }

                if (this.slickGrid != null &&
                    (this.slickContainer.GetHeight() < 200 ||
                     (J(Window.Instance).GetWidth() < 768)))
                {
                    this.element.CSS("height", "auto");
                    this.slickContainer.CSS("height", "auto")
                        .Children(".slick-viewport").CSS("height", "auto");
                    this.slickGrid.SetOptions(new SlickGridOptions { AutoHeight = true });
                }
            }

            if (this.slickGrid != null)
                this.slickGrid.ResizeCanvas();
        }

        protected virtual string GetInitialTitle()
        {
            return null;
        }

        protected virtual void CreateToolbarExtensions()
        {
        }

        protected virtual void CreateQuickFilters()
        {
            foreach (var filter in GetQuickFilters())
                AddQuickFilter(filter);
        }

        protected virtual List<QuickFilter<Widget, object>> GetQuickFilters()
        {
            var list = new List<QuickFilter<Widget, object>>();

            foreach (var column in this.allColumns.Where(x => 
                x.SourceItem != null && x.SourceItem.QuickFilter == true))
            {
                var item = column.SourceItem;

                var quick = new QuickFilter<Widget, object>();

                var filteringType = FilteringTypeRegistry.Get(item.FilteringType ?? "String");

                if (filteringType == typeof(DateFiltering) || filteringType == typeof(DateTimeFiltering))
                {
                    quick = DateRangeQuickFilter(item.Name, Q.TryGetText(item.Title) ?? item.Title ?? item.Name)
                        .As<QuickFilter<Widget, object>>();
                }
                else
                {
                    var filtering = (IFiltering)Activator.CreateInstance(filteringType);
                    if (filtering != null && filtering is IQuickFiltering)
                    {
                        ReflectionOptionsSetter.Set(filtering, item.FilteringParams);
                        filtering.Field = item;
                        filtering.Operator = FilterOperators.EQ;

                        ((IQuickFiltering)filtering).InitQuickFilter(quick);
                        quick.Options = Q.DeepExtend(quick.Options, item.QuickFilterParams);
                    }
                    else
                        continue;
                }

                list.Add(quick);
            }

            return list;
        }

        [IncludeGenericArguments(false), ScriptName("findQuickFilter")]
        private Widget FindQuickFilter(Type type, string field)
        {
            return J("#" + this.UniqueName + "_QuickFilter_" + field).GetWidget(type).As<Widget>();
        }

        [InlineCode("{this}.findQuickFilter({TWidget}, {field})")]
        protected TWidget FindQuickFilter<TWidget>(string field)
            where TWidget : Widget
        {
            return null;
        }

        protected virtual void CreateIncludeDeletedButton()
        {
            if (!GetIsActiveProperty().IsEmptyOrNull())
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

        public override void Destroy()
        {
            this.submitHandlers = null;

            if (this.toolbar != null)
            {
                this.toolbar.Destroy();
                this.toolbar = null;
            }

            if (this.slickGrid != null)
            {
                this.slickGrid.OnClick.Clear();
                this.slickGrid.OnSort.Clear();
                this.slickGrid.OnColumnsResized.Clear();
                this.slickGrid.OnColumnsReordered.Clear();
                this.slickGrid.Destroy();
                this.slickGrid = null;
            }

            if (this.view != null)
            {
                this.view.OnDataChanged.Clear();
                this.view.OnSubmit = null;
                this.view.SetFilter(null);
                this.view = null;
            }

            this.titleDiv = null;

            base.Destroy();
        }

        protected virtual string GetItemCssClass(TItem item, int index)
        {
            var activeFieldName = GetIsActiveProperty();
            if (activeFieldName.IsEmptyOrNull())
                return null;

            var value = item.As<JsDictionary>()[activeFieldName].As<Int32?>();
            if (value == null)
                return null;

            if (Script.TypeOf(value) == "number")
            {
                if (value.As<double>() < 0)
                    return "deleted";
                else if (value.As<double>() == 0)
                    return "inactive";
            }
            else if (Script.TypeOf(value) == "boolean")
            {
                if (value.As<Boolean>() == false)
                    return "deleted";
            }

            return null;
        }

        protected virtual dynamic GetItemMetadata(TItem item, int index)
        {
            var itemClass = GetItemCssClass(item, index);
            if (itemClass.IsEmptyOrNull())
                return new object();

            return new { cssClasses = itemClass };
        }

        protected virtual List<SlickColumn> PostProcessColumns(List<SlickColumn> columns)
        {
            columns.SetDefaults(localTextPrefix: GetLocalTextDbPrefix());
            return columns;
        }

        protected void InitialPopulate()
        {
            var self = this;

            if (PopulateWhenVisible())
            {
                LazyLoadHelper.ExecuteEverytimeWhenShown(element, () => self.RefreshIfNeeded(), false);
                if (element.Is(":visible"))
                    view.Populate();
            }
            else
                view.Populate();
        }

        protected override Promise InitializeAsync()
        {
            return base.InitializeAsync()
                .ThenAwait(GetColumnsAsync)
                .Then((columns) => {
                    this.allColumns = columns;
                    PostProcessColumns(this.allColumns);

                    var self = this;
                    if (this.filterBar != null)
                    {
                        filterBar.Store = new FilterStore(this.allColumns.Where(x => 
                            x.SourceItem != null && x.SourceItem.NotFilterable != true).Select(x => x.SourceItem));
                        filterBar.Store.Changed += (s, e) =>
                        {
                            if (restoringSettings <= 0)
                            {
                                self.PersistSettings();
                                self.Refresh();
                            }
                        };
                    }

                    var visibleColumns = this.allColumns.Where(x => x.Visible != false).ToList();

                    if (this.slickGrid != null)
                        this.slickGrid.SetColumns(visibleColumns);

                    SetInitialSortOrder();
                    initialSettings = GetCurrentSettings();
                    RestoreSettings();
                    InitialPopulate();
                });
        }

        protected virtual SlickGrid CreateSlickGrid()
        {
            List<SlickColumn> visibleColumns;

            if (this.IsAsyncWidget())
            {
                visibleColumns = new List<SlickColumn>();
            }
            else
            {
                this.allColumns = GetColumns();
                visibleColumns = PostProcessColumns(this.allColumns).Where(x => x.Visible != false).ToList();
            }

            var slickOptions = GetSlickOptions();
            var grid = new SlickGrid(slickContainer, data: view.As<List<dynamic>>(), 
                columns: visibleColumns, options: slickOptions);

            grid.RegisterPlugin(new SlickAutoTooltips(new SlickAutoTooltipsOptions
            {
                EnableForHeaderCells = true
            }));

            this.slickGrid = grid;
            this.Rows = this.slickGrid.As<GridRows<TItem>>();

            if (!IsAsyncWidget())
                SetInitialSortOrder();

            return grid;
        }

        protected virtual void SetInitialSortOrder()
        {
            var sortBy = GetDefaultSortBy();

            if (view != null)
                view.SortBy = sortBy.ToArray();

            var mapped = sortBy.Map<SlickColumnSort>(s =>
            {
                var x = new SlickColumnSort();
                if (s != null && s.ToLower().EndsWith(" desc"))
                {
                    x.ColumnId = s.Substr(0, s.Length - 5).TrimEnd();
                    x.SortAsc = false;
                }
                else
                {
                    x.ColumnId = s;
                    x.SortAsc = true;
                }
                return x;
            });

            slickGrid.SetSortColumns(mapped);
        }

        public TItem ItemAt(int row)
        {
            return ((object)slickGrid.GetDataItem(row)).As<TItem>();
        }

        public int RowCount()
        {
            return slickGrid.GetDataLength();
        }

        [System.Runtime.CompilerServices.IntrinsicProperty]
        public GridRows<TItem> Rows
        {
            get;
            private set;
        }

        /// <summary>
        /// Be careful with this as it contains all source items.
        /// If grid is grouped, or showing filtered items, 
        /// use Rows to access data item for a given row index
        /// </summary>
        public List<TItem> Items
        {
            [ScriptName("getItems")]
            get
            {
                return view.GetItems().As<List<TItem>>();
            }
            [ScriptName("setItems")]
            set
            {
                view.SetItems(value, true);
            }
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
                PersistSettings();
            };

            this.slickGrid.OnSort.Subscribe(slickGridOnSort);

            slickGridOnClick = (e, p) =>
            {
                self.OnClick(e, p.row, p.cell);
            };

            this.slickGrid.OnClick.Subscribe(slickGridOnClick);
            this.slickGrid.OnColumnsReordered.Subscribe((e, p) => this.PersistSettings());
            this.slickGrid.OnColumnsResized.Subscribe((e, p) => this.PersistSettings());
        }

        protected virtual string GetAddButtonCaption()
        {
            return "New";
        }

        protected virtual List<ToolButton> GetButtons()
        {
            return new List<ToolButton>();
        }

        protected virtual void EditItem(object entityOrId)
        {
            throw new NotImplementedException();
        }

        protected virtual void EditItemOfType(string itemType, object entityOrId)
        {
            if (itemType == GetItemType())
            {
                EditItem(entityOrId);
                return;
            }

            throw new NotImplementedException();
        }

        protected virtual void OnClick(jQueryEvent e, int row, int cell)
        {
            if (e.IsDefaultPrevented())
                return;

            var target = J(e.Target);
            if (!target.HasClass("s-EditLink"))
                target = target.Closest("a");
            if (target.HasClass("s-EditLink"))
            {
                e.PreventDefault();

                EditItemOfType(SlickFormatting.GetItemType(target), SlickFormatting.GetItemId(target));
            }
        }

        private void ViewDataChanged(jQueryEvent e, dynamic rows)
        {
            MarkupReady();
        }

        protected void BindToViewEvents()
        {
            var self = this;
            this.view.OnDataChanged.Subscribe((e, d) => self.ViewDataChanged(e, d));
            this.view.OnSubmit = (view) => self.OnViewSubmit();
            this.view.SetFilter((item, view) => self.OnViewFilter(item));
            this.view.OnProcessData = ((response, view) => self.OnViewProcessData(response));
        }

        protected virtual ListResponse<TItem> OnViewProcessData(ListResponse<TItem> response)
        {
            return response;
        }

        protected virtual bool OnViewFilter(TItem item)
        {
            return true;
        }

        protected virtual void GetIncludeColumns(JsDictionary<string, bool> include)
        {
            foreach (var column in slickGrid.GetColumns())
            {
                if (column.Field != null)
                    include[column.Field] = true;

                if (column.ReferencedFields != null)
                {
                    foreach (var x in column.ReferencedFields)
                        include[x] = true;
                }
            }
        }

        protected virtual void SetCriteriaParameter()
        {
            Script.Delete(view.Params, "Criteria");

            if (filterBar != null)
            {
                var criteria = filterBar.Store.ActiveCriteria;
                if (!criteria.IsEmpty)
                    ((ListRequest)view.Params).Criteria = criteria;
            }
        }

        public void SetEquality(string field, object value)
        {
            ((ListRequest)view.Params).SetEquality(field, value);
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
                return false;

            SetCriteriaParameter();
            SetIncludeColumnsParameter();
            InvokeSubmitHandlers();

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

        protected SlickRemoteView<TItem> CreateView()
        {
            var opt = GetViewOptions();
            return new SlickRemoteView<TItem>(opt);
        }

        protected virtual List<string> GetDefaultSortBy()
        {
            if (slickGrid != null && slickGrid.GetColumns().Count > 0)
            {
                var columns = slickGrid.GetColumns().Where(x => Script.IsValue(x.SortOrder) && x.SortOrder != 0).ToList();
                if (columns.Count > 0)
                {
                    columns.Sort((x, y) => Math.Abs(x.SortOrder).CompareTo(Math.Abs(y.SortOrder)));

                    var list = new List<string>();
                    for (var i = 0; i < columns.Count; i++)
                    {
                        var col = columns[i];
                        list.Add(col.Field + (col.SortOrder < 0 ? " DESC" : ""));
                    }

                    return list;
                }
            }

            return new List<string>();
        }

        protected virtual bool UsePager()
        {
            return false;
        }

        protected virtual bool EnableFiltering()
        {
            var attr = this.GetType().GetCustomAttributes(typeof(FilterableAttribute), true);
            return attr.Length > 0 && attr[0].As<FilterableAttribute>().Value;
        }

        protected virtual bool PopulateWhenVisible()
        {
            return false;
        }

        protected virtual void CreateFilterBar()
        {
            var filterBarDiv = J("<div/>")
                .AppendTo(this.element);

            var self = this;

            filterBar = new FilterDisplayBar(filterBarDiv);

            if (!IsAsyncWidget())
            {
                filterBar.Store = new FilterStore(this.allColumns.Where(x => 
                    x.SourceItem != null && x.SourceItem.NotFilterable != true).Select(x => x.SourceItem));
                filterBar.Store.Changed += (s, e) =>
                {
                    if (restoringSettings <= 0)
                    {
                        self.PersistSettings();
                        self.Refresh();
                    }
                };
            }
        }

        protected void CreatePager()
        {
            var pagerDiv = J("<div></div>")
                .AppendTo(this.element);

            ((dynamic)pagerDiv).slickPager(new
            {
                view = this.view.As<object>(),
                rowsPerPage = 20,
                rowsPerPageOptions = new int[] { 20, 100, 500, 2500 }
            });
        }

        protected virtual SlickRemoteViewOptions GetViewOptions()
        {
            var opt = new SlickRemoteViewOptions();
            opt.IdField = GetIdProperty();
            opt.SortBy = GetDefaultSortBy().As<string[]>();

            if (!UsePager())
                opt.RowsPerPage = 0;
            else if (element.HasClass("responsive-height"))
                opt.RowsPerPage = J(Window.Instance).GetWidth() < 768 ? 20 : 100;
            else
                opt.RowsPerPage = 100;

            opt.GetItemMetadata = (item, index) => this.GetItemMetadata(item.As<TItem>(), index);

            return opt;
        }

        protected void CreateToolbar(List<ToolButton> buttons)
        {
            var toolbarDiv = J("<div class=\"grid-toolbar\"></div>")
                .AppendTo(this.Element);

            toolbar = new Toolbar(toolbarDiv, new ToolbarOptions
            {
                Buttons = buttons,
                HotkeyContext = this.element[0]
            });
        }

        protected string Title
        {
            [ScriptName("getTitle")]
            get
            {
                if (titleDiv == null)
                    return null;

                return titleDiv.Children().GetText();
            }
            [ScriptName("setTitle")]
            set
            {
                if (value != Title)
                {
                    if (value == null)
                    {
                        if (titleDiv != null)
                        {
                            titleDiv.Remove();
                            titleDiv = null;
                        }
                    }
                    else
                    {
                        if (titleDiv == null)
                        {
                            titleDiv = J("<div class=\"grid-title\"><div class=\"title-text\"></div></div>")
                                .PrependTo(this.Element);

                        }
                        titleDiv.Children().Text(value);
                    }

                    Layout();
                }
            }
        }

        protected virtual string GetItemType()
        {
            return "Item";
        }

        protected SlickFormatter ItemLink(string itemType = null, string idField = null,
            Func<SlickFormatterContext, string> text = null, Func<SlickFormatterContext, string> cssClass = null, bool encode = true)
        {
            itemType = itemType ?? GetItemType();
            idField = idField ?? GetIdProperty();

            return SlickFormatting.ItemLink(itemType, idField, text, cssClass, encode);
        }

        protected virtual string GetColumnsKey()
        {
            var attr = this.GetType().GetCustomAttributes(typeof(ColumnsKeyAttribute), true);

            if (attr != null && attr.Length > 0)
                return attr[0].As<ColumnsKeyAttribute>().Value;

            return null;
        }

        protected virtual Promise<List<PropertyItem>> GetPropertyItemsAsync()
        {
            return Promise.Void.ThenAwait(() => 
            {
                var columnsKey = GetColumnsKey();
                if (!string.IsNullOrEmpty(columnsKey))
                    return Q.GetColumnsAsync(columnsKey);

                return Promise.FromValue(new List<PropertyItem>());
            });
        }

        protected virtual List<PropertyItem> GetPropertyItems()
        {
            var attr = this.GetType().GetCustomAttributes(typeof(ColumnsKeyAttribute), true);

            var columnsKey = GetColumnsKey();
            if (!string.IsNullOrEmpty(columnsKey))
                return Q.GetColumns(columnsKey);

            return new List<PropertyItem>();
        }

        protected virtual List<SlickColumn> GetColumns()
        {
            var propertyItems = GetPropertyItems();
            return PropertyItemsToSlickColumns(propertyItems);
        }

        protected virtual List<SlickColumn> PropertyItemsToSlickColumns(List<PropertyItem> propertyItems)
        {
            var columns = PropertyItemSlickConverter.ToSlickColumns(propertyItems);

            for (var i = 0; i < propertyItems.Count; i++)
            {
                var item = propertyItems[i];
                var column = columns[i];

                if (item.EditLink == true)
                {
                    var oldFormat = column.Format;
                    var css = Script.IsValue(item.EditLinkCssClass) ? item.EditLinkCssClass : null;

                    column.Format = this.ItemLink(
                        itemType: Script.IsValue(item.EditLinkItemType) ? item.EditLinkItemType : (string)null,
                        idField: Script.IsValue(item.EditLinkIdField) ? item.EditLinkIdField : (string)null,
                        text: ctx =>
                        {
                            if (oldFormat != null)
                                return oldFormat(ctx);

                            return Q.HtmlEncode(ctx.Value);
                        },
                        cssClass: ctx => css ?? "",
                        encode: false);

                    if (!string.IsNullOrEmpty(item.EditLinkIdField))
                    {
                        column.ReferencedFields = column.ReferencedFields ?? new List<string>();
                        column.ReferencedFields.Add(item.EditLinkIdField);
                    }
                }
            }

            return columns;
        }

        protected virtual Promise<List<SlickColumn>> GetColumnsAsync()
        {
            return GetPropertyItemsAsync().ThenSelect(propertyItems =>
            {
                return PropertyItemsToSlickColumns(propertyItems);
            });
        }

        protected virtual SlickGridOptions GetSlickOptions()
        {
            SlickGridOptions opt = new SlickGridOptions();
            opt.MultiSelect = false;
            opt.MultiColumnSort = true;
            opt.EnableCellNavigation = false;
            opt.HeaderRowHeight = DefaultHeaderHeight;
            opt.RowHeight = DefaultRowHeight;
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
            if (!PopulateWhenVisible())
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
            [InlineCode("{this}.isDisabled")]
            get { return isDisabled; }
            [ScriptName("setIsDisabled")]
            set
            {
                if (isDisabled != value)
                {
                    isDisabled = value;
                    if (isDisabled)
                        view.SetItems(new List<TItem>(), true);
                    UpdateDisabledState();
                }
            }
        }

        protected virtual string GetLocalTextDbPrefix()
        {
            if (localTextDbPrefix == null)
            {
                localTextDbPrefix = GetLocalTextPrefix() ?? "";
                if (localTextDbPrefix.Length > 0 && !localTextDbPrefix.EndsWith("."))
                    localTextDbPrefix = "Db." + localTextDbPrefix + ".";
            }

            return localTextDbPrefix;
        }

        protected virtual string GetLocalTextPrefix()
        {
            var attributes = this.GetType().GetCustomAttributes(typeof(LocalTextPrefixAttribute), true);
            if (attributes.Length >= 1)
                return attributes[0].As<LocalTextPrefixAttribute>().Value;
            else
                return "";
        }

        protected virtual string GetIdProperty()
        {
            if (idProperty == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(IdPropertyAttribute), true);
                if (attributes.Length == 1)
                    idProperty = attributes[0].As<IdPropertyAttribute>().Value;
                else
                    idProperty = "ID";
            }

            return idProperty;
        }

        protected virtual string GetIsActiveProperty()
        {
            if (isActiveProperty == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(IsActivePropertyAttribute), true);
                if (attributes.Length == 1)
                {
                    isActiveProperty = attributes[0].As<IsActivePropertyAttribute>().Value;
                }
                else
                {
                    isActiveProperty = String.Empty;
                }
            }

            return isActiveProperty;
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

        public void AddFilterSeparator()
        {
            if (quickFiltersDiv != null)
                quickFiltersDiv.Append(J("<hr/>"));
        }

        protected string DetermineText(Func<string, string> getKey)
        {
            var localTextPrefix = GetLocalTextDbPrefix();

            if (!localTextPrefix.IsEmptyOrNull())
            {
                var local = Q.TryGetText(getKey(localTextPrefix));
                if (local != null)
                    return local;
            }

            return null;
        }

        [ScriptName("addQuickFilter"), IncludeGenericArguments(false)]
        protected object AddQuickFilter<TWidget, TOpt>(QuickFilter<TWidget, TOpt> opt)
            where TWidget: Widget
        {
            if (opt == null)
                throw new ArgumentNullException("opt");

            if (quickFiltersDiv == null)
            {
                J("<div/>").AddClass("clear").AppendTo(toolbar.Element);
                quickFiltersDiv = J("<div/>").AddClass("quick-filters-bar").AppendTo(toolbar.Element);
            }

            var quickFilter = J("<div class='quick-filter-item'><span class='quick-filter-label'></span></div>")
                .AppendTo(quickFiltersDiv)
                .Children().Text(opt.Title ?? DetermineText(pre => pre + opt.Field) ?? opt.Field)
                .Parent();

            var widget = Widget.CreateOfType(opt.Type, e =>
            {
                if (!opt.Field.IsEmptyOrNull())
                    e.Attribute("id", this.UniqueName + "_QuickFilter_" + opt.Field);
                e.Attribute("placeholder", " ");
                e.AppendTo(quickFilter);
                if (opt.Element != null)
                    opt.Element(e);
            }, opt.Options, opt.Init.As<Action<Widget>>());

            Action submitHandler = () =>
            {
                if (quickFilter.HasClass("ignore"))
                    return;

                var request = (ListRequest)view.Params;
                request.EqualityFilter = request.EqualityFilter ?? new JsDictionary<string, object>();

                var value = EditorUtils.GetValue(widget);
                bool active = Script.IsValue(value) && !string.IsNullOrEmpty(value.ToString());

                if (opt.Handler != null)
                {
                    var args = new QuickFilterArgs<TWidget>
                    {
                        Field = opt.Field,
                        Request = request,
                        EqualityFilter = request.EqualityFilter,
                        Value = value,
                        Active = active,
                        Widget = widget.As<TWidget>(),
                        Handled = true
                    };

                    opt.Handler(args.As<Serenity.QuickFilterArgs<TWidget>>());

                    quickFilter.ToggleClass("quick-filter-active", args.Active);

                    if (!args.Handled)
                    {
                        if (value.As<object[]>().Length > 0)
                            request.Criteria &= new Criteria(opt.Field).In(value.As<object[]>());
                        else
                            request.EqualityFilter[opt.Field] = value;
                    }
                }
                else
                {
                    if (jQuery.IsArray(value))
                    {
                        if (value.As<object[]>().Length > 0)
                            request.Criteria &= new Criteria(opt.Field).In(value.As<object[]>());
                    }
                    else
                        request.EqualityFilter[opt.Field] = value;

                    quickFilter.ToggleClass("quick-filter-active", active);

                }
            };

            widget.Change(e =>
            {
                this.QuickFilterChange(e);
            });

            submitHandlers += submitHandler;
            widget.Element.Bind("remove." + this.uniqueName, x =>
            {
                submitHandlers -= submitHandler;
            });

            return widget;
        }

        [InlineCode("{this}.addQuickFilter({{ field: {field}, type: {TWidget}, title: {title}, options: {options}, handler: {handler}, init: {init}, element: {element} }})")]
        public TWidget AddEqualityFilter<TWidget>(string field, string title = null, object options = null, Action<QuickFilterArgs<TWidget>> handler = null,
            Action<jQueryObject> element = null, Action<TWidget> init = null)
            where TWidget: Widget
        {
            return null;
        }

        public DateEditor AddDateRangeFilter(string field, string title = null)
        {
            return (DateEditor)AddQuickFilter(DateRangeQuickFilter(field, title));
        }

        private QuickFilter<DateEditor, DateTimeEditorOptions> DateRangeQuickFilter(string field, string title)
        {
            DateEditor end = null;

            return new QuickFilter<DateEditor, DateTimeEditorOptions>
            {
                Field = field,
                Type = typeof(DateEditor),
                Title = title,
                Element = e1 =>
                {
                    end = Widget.Create<DateEditor>(element: e2 => e2.InsertAfter(e1));
                    end.Element.Change(x => e1.TriggerHandler("change"));
                    J("<span/>").AddClass("range-separator").Text("-").InsertAfter(e1);
                },
                Handler = args =>
                {
                    args.Active =
                        !string.IsNullOrEmpty(args.Widget.Value) ||
                        !string.IsNullOrEmpty(end.Value);

                    if (!string.IsNullOrEmpty(args.Widget.Value))
                        args.Request.Criteria &= new Criteria(args.Field) >= args.Widget.Value;

                    if (!string.IsNullOrEmpty(end.Value))
                    {
                        var next = new JsDate(end.ValueAsDate.ValueOf());
                        next.SetDate(next.GetDate() + 1);
                        args.Request.Criteria &= new Criteria(args.Field) < Q.FormatDate(next, "yyyy-MM-dd");
                    }
                }
            };
        }

        protected virtual void InvokeSubmitHandlers()
        {
            if (submitHandlers != null)
                submitHandlers();
        }

        protected virtual void QuickFilterChange(jQueryEvent e)
        {
            this.Refresh();
        }

        protected virtual Storage GetPersistanceStorage()
        {
            return DefaultPersistanceStorage;
        }

        protected virtual string GetPersistanceKey()
        {
            var key = "GridSettings:";

            var path = Window.Location.Pathname;
            if (!string.IsNullOrEmpty(path))
                key += string.Join("/", path.Substr(1).Split('/').Take(2).ToArray()) + ":";

            key += this.GetType().FullName;

            return key;
        }

        protected virtual GridPersistanceFlags GridPersistanceFlags()
        {
            return new GridPersistanceFlags();
        }

        protected virtual void RestoreSettings(PersistedGridSettings settings = null, GridPersistanceFlags flags = null)
        {
            if (settings == null)
            {
                var storage = GetPersistanceStorage();
                if (storage == null)
                    return;

                var json = storage.GetItem(GetPersistanceKey()).TrimToNull();
                if (json != null && json.StartsWith("{") && json.EndsWith("}"))
                    settings = System.Serialization.Json.Parse<PersistedGridSettings>(json);
                else
                    return;
            }

            if (slickGrid == null)
                return;

            var columns = slickGrid.GetColumns();
            JsDictionary<string, SlickColumn> colById = null;

            Action<List<SlickColumn>> updateColById = (cl) =>
            {
                colById = new JsDictionary<string, SlickColumn>();
                foreach (var c in cl)
                    colById[c.Identifier] = c;
            };

            view.BeginUpdate();
            restoringSettings++;
            try
            {
                flags = flags ?? GridPersistanceFlags();
                if (settings.Columns != null)
                {
                    if (flags.ColumnVisibility != false)
                    {
                        var visible = new JsDictionary<string, bool>();
                        updateColById(this.allColumns);

                        var newColumns = new List<SlickColumn>();
                        foreach (var x in settings.Columns)
                            if (x.ID != null && x.Visible == true)
                            {
                                var column = colById[x.ID];
                                if (column != null && (column.SourceItem == null || column.SourceItem.FilterOnly != true))
                                {
                                    column.Visible = true;
                                    newColumns.Add(column);
                                    colById.Remove(x.ID);
                                }
                            }

                        foreach (var c in this.allColumns)
                            if (colById.ContainsKey(c.Identifier))
                            {
                                c.Visible = false;
                                newColumns.Add(c);
                            }

                        this.allColumns = newColumns;
                        columns = this.allColumns.Where(x => x.Visible == true).ToList();
                    }

                    if (flags.ColumnWidths != false)
                    {
                        updateColById(columns);
                        foreach (var x in settings.Columns)
                            if (x.ID != null && x.Width != null && x.Width != 0)
                            {
                                var column = colById[x.ID];
                                if (column != null)
                                    column.Width = x.Width.Value;
                            }
                    }

                    if (flags.SortColumns != false)
                    {
                        updateColById(columns);
                        var list = new List<SlickColumnSort>();
                        foreach (var x in settings.Columns
                            .Where(x => x.ID != null && (x.Sort ?? 0) != 0)
                            .OrderBy(z => Math.Abs(z.Sort.Value)))
                        {
                            var column = colById[x.ID];
                            if (column != null)
                            {
                                list.Add(new SlickColumnSort
                                {
                                    ColumnId = x.ID,
                                    SortAsc = x.Sort > 0
                                });
                            }
                        }

                        this.slickGrid.SetSortColumns(list);
                    }

                    this.slickGrid.SetColumns(columns);
                    this.slickGrid.Invalidate();
                }

                if (settings.FilterItems != null &&
                    flags.FilterItems != false &&
                    filterBar != null &&
                    filterBar.Store != null)
                {
                    filterBar.Store.Items.Clear();
                    filterBar.Store.Items.AddRange(settings.FilterItems);
                    filterBar.Store.RaiseChanged();
                }

                if (settings.IncludeDeleted != null &&
                    flags.IncludeDeleted != false)
                {
                    var includeDeletedToggle = this.Element.Find(".s-IncludeDeletedToggle");
                    if (Q.IsTrue(settings.IncludeDeleted) != includeDeletedToggle.HasClass("pressed"))
                        includeDeletedToggle.Children("a").Click();
                }
            }
            finally
            {
                restoringSettings--;
                view.EndUpdate();
            }
        }

        protected virtual void PersistSettings(GridPersistanceFlags flags = null)
        {
            var storage = GetPersistanceStorage();
            if (storage == null)
                return;

            var settings = GetCurrentSettings(flags);
            storage.SetItem(GetPersistanceKey(), Q.ToJSON(settings));
        }

        protected virtual PersistedGridSettings GetCurrentSettings(GridPersistanceFlags flags = null)
        {
            flags = flags ?? GridPersistanceFlags();

            var settings = new PersistedGridSettings();

            if (flags.ColumnVisibility != false ||
                flags.ColumnWidths != false ||
                flags.SortColumns != false)
            {
                settings.Columns = new List<PersistedGridColumn>();
                var sortColumns = slickGrid.GetSortColumns();
                foreach (var column in this.slickGrid.GetColumns())
                {
                    var p = new PersistedGridColumn();
                    p.ID = column.Identifier;

                    if (flags.ColumnVisibility != false)
                        p.Visible = true;

                    if (flags.ColumnWidths != false)
                        p.Width = column.Width;

                    if (flags.SortColumns != false)
                    {
                        var sort = sortColumns.IndexOf(x => x.ColumnId == column.Identifier);
                        p.Sort = sort >= 0 ? (sortColumns[sort].SortAsc != false ? (sort + 1) : (-sort - 1)) : 0;
                    }

                    settings.Columns.Add(p);
                }
            }

            if (flags.IncludeDeleted != false)
                settings.IncludeDeleted = this.Element.Find(".s-IncludeDeletedToggle").HasClass("pressed");

            if (flags.FilterItems != false &&
                this.filterBar != null &&
                this.filterBar.Store != null)
            {
                settings.FilterItems = this.filterBar.Store.Items.ToList();
            }

            return settings;
        }

        [IntrinsicProperty]
        public static Storage DefaultPersistanceStorage { get; set; }
        [IntrinsicProperty]
        public static int DefaultRowHeight { get; set; }
        [IntrinsicProperty]
        public static int DefaultHeaderHeight { get; set; }

        public SlickRemoteView<TItem> View { [InlineCode("{this}.view")] get { return view; } }
        public SlickGrid SlickGrid { [InlineCode("{this}.slickGrid")] get { return slickGrid; } }

        jQueryObject IDataGrid.GetElement()
        {
            return this.element;
        }

        SlickGrid IDataGrid.GetGrid()
        {
            return this.slickGrid;
        }

        SlickRemoteView IDataGrid.GetView()
        {
            return this.view;
        }

        FilterStore IDataGrid.GetFilterStore()
        {
            return this.filterBar == null ? null : this.filterBar.Store;
        }
    }

    [Imported, IncludeGenericArguments(false), ScriptName("DataGrid")]
    public abstract class DataGrid<TItem> : DataGrid<TItem, object>
        where TItem : class, new()
    {
        public DataGrid(jQueryObject container)
            : base(container, null)
        {
        }
    }

    [IncludeGenericArguments(false)]
    public class GridRows<TItem>
    {
        private GridRows()
        {
        }

        public TItem this[int row]
        {
            [InlineCode("{this}.getDataItem({row})")]
            get
            {
                return default(TItem);
            }
        }

        public int Count
        {
            [InlineCode("{this}.getDataLength()")]
            get
            {
                return 0;
            }
        }
    }

    [Imported]
    public class ColumnPickerDialog : TemplatedDialog
    {
        public ColumnPickerDialog()
            : base()
        {
        }

        public static ToolButton CreateToolButton(IDataGrid grid)
        {
            return null;
        }

        [IntrinsicProperty]
        public List<SlickColumn> AllColumns { get; set; }

        [IntrinsicProperty]
        public string[] DefaultColumns { get; set; }

        [IntrinsicProperty]
        public string[] VisibleColumns { get; set; }

        [IntrinsicProperty]
        public Action Done { get; set; }
    }

    [Imported, Serializable]
    public class GridPersistanceFlags
    {
        public bool? ColumnWidths { get; set; }
        public bool? ColumnVisibility { get; set; }
        public bool? SortColumns { get; set; }
        public bool? FilterItems { get; set; }
        public bool? QuickFilters { get; set; }
        public bool? IncludeDeleted { get; set; }
    }

    [Imported, Serializable]
    public class PersistedGridSettings
    {
        public List<PersistedGridColumn> Columns { get; set; }
        public List<FilterLine> FilterItems { get; set; }
        public Dictionary<string, object> QuickFilters { get; set; }
        public bool? IncludeDeleted { get; set; }
    }

    [Imported, Serializable]
    public class PersistedGridColumn
    {
        public string ID { get; set; }
        public int? Width { get; set; }
        public int? Sort { get; set; }
        public bool? Visible { get; set; }
    }
}