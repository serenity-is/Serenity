using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    public interface IDataGrid
    {
    }

    public abstract class DataGrid<TItem, TOptions> : Widget<TOptions>, IDataGrid
        where TOptions: GridOptions, new()
        where TItem: class, new()
    {
        protected Toolbar toolbar;
        protected SlickRemoteView<TItem> view;
        protected jQueryObject slickContainer;
        protected SlickGrid slickGrid;
        protected string idFieldName;
        protected string isActiveFieldName;
        protected string localTextPrefix;
        private bool isDisabled;

        public DataGrid(jQueryObject container, TOptions opt)
            : base(container, opt)
        {
            var self = this;

            this.element.AddClass("s-DataGrid").Html("");
            this.element.AddClass("s-" + this.GetType().Name);
            this.element.AddClass("require-layout").Bind("layout", delegate 
            {
                if (!this.element.Is(":visible"))
                    return;

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

            return null;
        }

        protected virtual dynamic GetItemMetadata(TItem entity, int index)
        {
            var activeFieldName = GetIsActiveFieldName();
            if (activeFieldName.IsEmptyOrNull())
                return new object();

            var value = entity.As<JsDictionary<string, object>>()[activeFieldName].As<Int32?>();
            if (value == null)
                return new object();

            if (Script.TypeOf(value) == "number")
            {
                if (IdExtensions.IsNegativeId(value.As<Int64>()))
                    return new { cssClasses = "deleted" };
                else if (value.As<Int32>() == 0)
                    return new { cssClasses = "inactive" };
            }
            else if (Script.TypeOf(value) == "boolean")
            {
                if (value.As<Boolean>() == false)
                    return new { cssClasses = "deleted" };
            }

            return new object();
        }

        protected virtual SlickGrid CreateSlickGrid()
        {
            var slickColumns = GetColumns().SetDefaults(localTextPrefix: GetLocalTextPrefix());
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
            return "Yeni";
        }

        protected virtual List<ToolButton> GetButtons()
        {
            return new List<ToolButton>();
        }

        protected virtual void EditItem(object entityOrId)
        {
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
                view.SetItems(new List<TItem>(), true);
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

        protected SlickRemoteView<TItem> CreateView()
        {
            var opt = GetViewOptions();
            return new SlickRemoteView<TItem>(opt);
        }

        protected virtual List<string> GetDefaultSortBy()
        {
            return new List<string> { GetIdFieldName() };
        }

        protected virtual SlickRemoteViewOptions GetViewOptions()
        {
            var opt = new SlickRemoteViewOptions();
            opt.IdField = GetIdFieldName();
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
            return "Item";
        }

        protected SlickFormatter ItemLink(string itemType = null, string idField = null,
            Func<SlickFormatterContext, string> text = null, Func<SlickFormatterContext, string> cssClass = null)
        {
            itemType = itemType ?? GetItemType();
            idField = idField ?? GetIdFieldName();

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
            opt.HeaderRowHeight = 23;
            opt.RowHeight = 23;
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
                        view.SetItems(new List<TItem>(), true);
                    UpdateDisabledState();
                }
            }
        }

        protected virtual string GetLocalTextPrefix()
        {
            localTextPrefix = localTextPrefix ?? "";
            return localTextPrefix;
        }

        protected virtual string GetIdFieldName()
        {
            if (idFieldName == null)
            {
                var attributes = this.GetType().GetCustomAttributes(typeof(IdPropertyAttribute), true);
                if (attributes.Length == 1)
                    idFieldName = attributes[0].As<IdPropertyAttribute>().IdProperty;
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
                    isActiveFieldName = attributes[0].As<IsActivePropertyAttribute>().IsActiveProperty;
                else
                    isActiveFieldName = "IsActive";
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

        public SlickRemoteView<TItem> View { get { return view; } }
        public SlickGrid SlickGrid { get { return slickGrid; } }
    }

    [Imported, Serializable]
    public class GridOptions
    {
        [Hidden]
        public string Title { get; set; }
        [Hidden]
        public string Height { get; set; }
        [Hidden]
        public bool HidePager { get; set; }
        [Hidden]
        public bool PopulateWhenVisible { get; set; }
    }
}