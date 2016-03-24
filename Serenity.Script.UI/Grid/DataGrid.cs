using System;
using System.Collections;
using System.Collections.Generic;
using jQueryApi;
using System.Linq;
using Serenity.Data;
using System.Html;
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
        protected string idFieldName;
        protected string isActiveFieldName;
        protected string localTextPrefix;
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

            UpdateDisabledState();

            if (!IsAsyncWidget())
                InitialPopulate();
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

        protected virtual void CreateIncludeDeletedButton()
        {
            if (!GetIsActiveFieldName().IsEmptyOrNull())
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
            var activeFieldName = GetIsActiveFieldName();
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
            columns.SetDefaults(localTextPrefix: GetLocalTextPrefix());
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
                    columns = PostProcessColumns(columns);

                    var self = this;
                    if (this.filterBar != null)
                    {
                        filterBar.Store = new FilterStore(this.GetPropertyItems().Where(x => x.NotFilterable != true));
                        filterBar.Store.Changed += (s, e) => self.Refresh();
                    }

                    if (this.slickGrid != null)
                        this.slickGrid.SetColumns(columns);

                    SetInitialSortOrder();
                    InitialPopulate();
                });
        }

        protected virtual SlickGrid CreateSlickGrid()
        {
            List<SlickColumn> slickColumns;

            if (this.IsAsyncWidget())
            {
                slickColumns = new List<SlickColumn>();
            }
            else
            {
                slickColumns = PostProcessColumns(GetColumns());
            }

            var slickOptions = GetSlickOptions();
            var grid = new SlickGrid(slickContainer, data: view.As<List<dynamic>>(), columns: slickColumns, options: slickOptions);

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
            get
            {
                return view.GetItems().As<List<TItem>>();
            }
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
            return "Yeni";
        }

        protected virtual List<ToolButton> GetButtons()
        {
            return new List<ToolButton>();
        }

        protected virtual void EditItem(object entityOrId)
        {
            throw new NotImplementedException();
        }

        protected virtual void EditItem(string itemType, object entityOrId)
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

                EditItem(SlickFormatting.GetItemType(target), SlickFormatting.GetItemId(target));
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
                filterBar.Store = new FilterStore(this.GetPropertyItems().Where(x => x.NotFilterable != true));
                filterBar.Store.Changed += (s, e) => self.Refresh();
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
            opt.IdField = GetIdFieldName();
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
            get
            {
                if (titleDiv == null)
                    return null;

                return titleDiv.Children().GetText();
            }
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

                            titleDiv.Children().Text(value);
                        }
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
            idField = idField ?? GetIdFieldName();

            return SlickFormatting.ItemLink(itemType, idField, text, cssClass, encode);
        }

        protected virtual Promise<List<PropertyItem>> GetPropertyItemsAsync()
        {
            return Promise.Void.ThenAwait(() => 
            {
                var attr = this.GetType().GetCustomAttributes(typeof(ColumnsKeyAttribute), true);

                if (attr != null && attr.Length > 0)
                    return Q.GetColumnsAsync(attr[0].As<ColumnsKeyAttribute>().Value);

                return Promise.FromValue(new List<PropertyItem>());
            });
        }

        protected virtual List<PropertyItem> GetPropertyItems()
        {
            var attr = this.GetType().GetCustomAttributes(typeof(ColumnsKeyAttribute), true);

            if (attr != null && attr.Length > 0)
                return Q.GetColumns(attr[0].As<ColumnsKeyAttribute>().Value);

            return new List<PropertyItem>();
        }

        protected virtual List<SlickColumn> GetColumns()
        {
            var columnItems = GetPropertyItems().Where(x => x.FilterOnly != true && x.Visible != false).ToList();
            return PropertyItemsToSlickColumns(columnItems);
        }

        protected virtual List<SlickColumn> PropertyItemsToSlickColumns(List<PropertyItem> propertyItems)
        {
            var columns = PropertyItemSlickConverter.ToSlickColumns(propertyItems);

            if (propertyItems != null)
            {
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
            }

            return columns;
        }

        protected virtual Promise<List<SlickColumn>> GetColumnsAsync()
        {
            return GetPropertyItemsAsync().ThenSelect(propertyItems =>
            {
                return PropertyItemsToSlickColumns(propertyItems.Where(x => x.FilterOnly != true && x.Visible != false).ToList());
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
            get { return isDisabled; }
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

        protected virtual string GetLocalTextPrefix()
        {
            if (localTextPrefix == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(LocalTextPrefixAttribute), true);
                if (attributes.Length >= 1)
                    localTextPrefix = attributes[0].As<LocalTextPrefixAttribute>().Value;
                else
                    localTextPrefix = "";
            }

            return localTextPrefix;
        }

        protected virtual string GetIdFieldName()
        {
            if (idFieldName == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(IdPropertyAttribute), true);
                if (attributes.Length == 1)
                    idFieldName = attributes[0].As<IdPropertyAttribute>().Value;
                else
                    idFieldName = "ID";
            }

            return idFieldName;
        }

        protected virtual string GetIsActiveFieldName()
        {
            if (isActiveFieldName == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(IsActivePropertyAttribute), true);
                if (attributes.Length == 1)
                {
                    isActiveFieldName = attributes[0].As<IsActivePropertyAttribute>().Value;
                }
                else
                {
                    isActiveFieldName = String.Empty;
                }
            }

            return isActiveFieldName;
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

        protected string DetermineText(string text, Func<string, string> getKey)
        {
            if (text != null &&
                !text.StartsWith("`"))
            {
                var local = Q.TryGetText(text);
                if (local != null)
                    return local;
            }

            if (text != null && text.StartsWith("`"))
                text = text.Substr(1);

            var localTextPrefix = GetLocalTextPrefix();

            if (!localTextPrefix.IsEmptyOrNull())
            {
                var local = Q.TryGetText(getKey(localTextPrefix));
                if (local != null)
                    return local;
            }

            return text;
        }

        public TWidget AddEqualityFilter<TWidget>(string field, string title = null, object options = null, Action<QuickFilterArgs<TWidget>> handler = null,
            Action<jQueryObject> element = null, Action<TWidget> init = null)
            where TWidget: Widget
        {
            if (quickFiltersDiv == null)
            {
                J("<div/>").AddClass("clear").AppendTo(toolbar.Element);
                quickFiltersDiv = J("<div/>").AddClass("quick-filters-bar").AppendTo(toolbar.Element);
            }
           
            var quickFilter = J("<div class='quick-filter-item'><span class='quick-filter-label'></span></div>")
                .AppendTo(quickFiltersDiv)
                .Children().Text(DetermineText(title ?? field, pre => pre + field) ?? "")
                .Parent();

            var widget = Widget.Create<TWidget>(
                element: e =>
                {
                    if (!field.IsEmptyOrNull())
                        e.Attribute("id", this.UniqueName + "_QuickFilter_" + field);
                    e.Attribute("placeholder", " ");
                    e.AppendTo(quickFilter);
                    if (element != null)
                        element(e);
                },
                options: options,
                init: init);

            Action submitHandler = () =>
            {
                if (quickFilter.HasClass("ignore"))
                    return;

                var request = (ListRequest)view.Params;
                request.EqualityFilter = request.EqualityFilter ?? new JsDictionary<string, object>();

                var value = EditorUtils.GetValue(widget);
                bool active = Script.IsValue(value) && !string.IsNullOrEmpty(value.ToString());

                if (handler != null)
                {
                    var args = new QuickFilterArgs<TWidget>
                    {
                        Field = field,
                        Request = request,
                        EqualityFilter = request.EqualityFilter,
                        Value = value,
                        Active = active,
                        Widget = widget,
                        Handled = true
                    };

                    handler(args);

                    quickFilter.ToggleClass("quick-filter-active", args.Active);

                    if (!args.Handled)
                        request.EqualityFilter[field] = value;
                }
                else
                {
                    request.EqualityFilter[field] = value;
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

        public DateEditor AddDateRangeFilter(string field, string title = null)
        {
            DateEditor end = null;

            return AddEqualityFilter<DateEditor>(field,
                element: e1 =>
                {
                    end = Widget.Create<DateEditor>(element: e2 => e2.InsertAfter(e1));
                    end.Element.Change(x => e1.TriggerHandler("change"));
                    J("<span/>").AddClass("range-separator").Text("-").InsertAfter(e1);
                },
                handler: args =>
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
                });
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

        public static int DefaultRowHeight { get; set; }
        public static int DefaultHeaderHeight { get; set; }

        public SlickRemoteView<TItem> View { get { return view; } }
        public SlickGrid SlickGrid { get { return slickGrid; } }

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
}