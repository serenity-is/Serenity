using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Element("<div/>")]
    [IdProperty("id")]
    public abstract class CheckTreeEditor<TOptions> : DataGrid<CheckTreeItem, TOptions>, IGetEditValue, ISetEditValue
        where TOptions: CheckTreeEditorOptions, new()
    {
        private JsDictionary<string, CheckTreeItem> byId;

        public CheckTreeEditor(jQueryObject div, TOptions opt)
            : base(div, opt)
        {
            div.AddClass("s-CheckTreeEditor");

            UpdateItems();
        }

        protected virtual List<CheckTreeItem> GetItems()
        {
            return options.Items ?? new List<CheckTreeItem>();
        }

        protected virtual void UpdateItems()
        {
            var items = GetItems();
            var itemById = new JsDictionary<string, CheckTreeItem>();
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                item.Children = new List<CheckTreeItem>();
                if (!item.Id.IsEmptyOrNull())
                    itemById[item.Id] = item;

                if (!item.ParentId.IsEmptyOrNull())
                {
                    var parent = itemById[item.ParentId];
                    if (parent != null)
                        parent.Children.Add(item);
                }
            }

            view.AddData(new ListResponse<CheckTreeItem>
            {
                Entities = items,
                Skip = 0,
                Take = 0,
                TotalCount = items.Count
            });

            UpdateSelectAll();
        }

        void IGetEditValue.GetEditValue(PropertyItem property, dynamic target)
        {
            target[property.Name] = this.Value;
        }

        void ISetEditValue.SetEditValue(dynamic source, PropertyItem property)
        {
            var list = source[property.Name] as List<string>;
            if (list != null)
                this.Value = list;
        }

        protected override List<ToolButton> GetButtons()
        {
            string selectAllText = options.As<CheckTreeEditorOptions>().SelectAllOptionText;
            if (selectAllText.IsEmptyOrNull())
                return null;

            var self = this;

            return new List<ToolButton>
            {
                GridSelectAllButtonHelper.Define<CheckTreeItem>(
                    () => self,
                    x => x.Id,
                    x => x.IsSelected,
                    (x, v) => x.IsSelected = v
                )
            };
        }

        protected override SlickGrid CreateSlickGrid()
        {
            this.element.AddClass("slick-no-cell-border").AddClass("slick-no-odd-even");
            var result = base.CreateSlickGrid();
            this.element.AddClass("slick-hide-header");
            result.ResizeCanvas();
            return result;
        }

        protected override bool OnViewFilter(CheckTreeItem item)
        {
            if (!base.OnViewFilter(item))
                return false;

            var items = view.GetItems();
            var self = this;

            return SlickTreeHelper.FilterCustom(item, x =>
            {
                if (x.ParentId == null)
                    return null;

                if (self.byId == null)
                {
                    self.byId = new JsDictionary<string, CheckTreeItem>();
                    for (var i = 0; i < items.Count; i++)
                    {
                        var o = items[i];
                        if (o.Id != null)
                            self.byId[o.Id] = o;
                    }
                }

                return self.byId[x.ParentId];
            });
        }

        protected override ListResponse<CheckTreeItem> OnViewProcessData(ListResponse<CheckTreeItem> response)
        {
            response = base.OnViewProcessData(response);
            byId = null;
            SlickTreeHelper.SetIndents(response.Entities, getId: x => x.Id, getParentId: x => x.ParentId, setCollapsed: false);
            return response;
        }

        protected override void OnClick(jQueryEvent e, int row, int cell)
        {
            base.OnClick(e, row, cell);

            if (!e.IsDefaultPrevented())
                SlickTreeHelper.ToggleClick(e, row, cell, view, x => x.Id);

            if (e.IsDefaultPrevented())
                return;

            var target = J(e.Target);
            if (target.HasClass("check-box"))
            {
                bool checkedOrPartial = target.HasClass("checked") || target.HasClass("partial");
                var item = view.Rows[row];
                var anyChanged = item.IsSelected != (!checkedOrPartial);
                view.BeginUpdate();
                try
                {
                    item.IsSelected = !checkedOrPartial;
                    view.UpdateItem(item.Id, item);
                    anyChanged = SetAllSubTreeSelected(item, item.IsSelected) | anyChanged;
                    UpdateSelectAll();
                    UpdateFlags();
                }
                finally
                {
                    view.EndUpdate();
                }

                if (anyChanged)
                    this.element.TriggerHandler("change");
            }
        }

        protected void UpdateSelectAll()
        {
            GridSelectAllButtonHelper.Update<CheckTreeItem>(this, x => x.IsSelected);
        }

        private void UpdateFlags()
        {
            var view = this.view;
            var items = view.GetItems();
            bool threeState = options.As<CheckTreeEditorOptions>().ThreeStateHierarchy;

            if (!threeState)
                return;

            view.BeginUpdate();
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];

                    if (threeState)
                    {
                        if (item.Children.Count > 0)
                        {
                            bool treeSelected = AllSubTreeSelected(item);
                            if (treeSelected != item.IsDescendantsSelected)
                            {
                                item.IsDescendantsSelected = treeSelected;
                                view.UpdateItem(item.Id, item);
                            }
                        }
                    }
                }
            }
            finally
            {
                view.EndUpdate();
            }
        }

        private bool SetAllSubTreeSelected(CheckTreeItem item, bool selected)
        {
            var result = false;
            for (int i = 0; i < item.Children.Count; i++)
            {
                var sub = item.Children[i];
                if (sub.IsSelected != selected)
                {
                    result = true;
                    sub.IsSelected = selected;
                    view.UpdateItem(sub.Id, sub);
                }
                if (sub.Children.Count > 0)
                    result = SetAllSubTreeSelected(sub, selected) | result;
            }
            return result;
        }

        private bool AllChildSelected(CheckTreeItem item)
        {
            for (int i = 0; i < item.Children.Count; i++)
            {
                if (!item.Children[i].IsSelected)
                    return false;
            }

            return true;
        }

        private bool AllItemsSelected()
        {
            foreach (var row in view.Rows)
                if (!row.IsSelected)
                    return false;

            return view.Rows.Count > 0;
        }

        private bool AllSubTreeSelected(CheckTreeItem item)
        {
            if (item.Children.Count > 0)
                for (int i = 0; i < item.Children.Count; i++)
                {
                    var sub = item.Children[i];
                    if (!sub.IsSelected)
                        return false;
                    if (!AllSubTreeSelected(sub))
                        return false;
                }

            return true;
        }

        protected override List<SlickColumn> GetColumns()
        {
            var self = this;

            return new List<SlickColumn>
            {
                new SlickColumn { Field = "text", Title = "Kayıt", Width = 80, Format = 
                    SlickFormatting.TreeToggle(() => self.view, x => x.Id, ctx => 
                    {
                        var cls = "check-box";
                        var item = (CheckTreeItem)ctx.Item;

                        bool threeState = options.As<CheckTreeEditorOptions>().ThreeStateHierarchy;
                        if (item.IsSelected)
                        {
                            if (threeState && !item.IsDescendantsSelected)
                                cls += " partial";
                            else
                                cls += " checked";
                        }

                        return "<span class=\"" + cls + "\"></span>" + 
                            Q.HtmlEncode(ctx.Value);
                    })
                }
            };
        }

        protected override SlickGridOptions GetSlickOptions()
        {
            var opt = base.GetSlickOptions();
            opt.ForceFitColumns = true;
            return opt;
        }

        public List<string> Value
        {
            get
            {
                List<string> list = new List<string>();
                var items = this.View.GetItems();
                for (var i = 0; i < items.Count; i++)
                    if (items[i].IsSelected)
                        list.Add(items[i].Id);
                return list;
            }
            set
            {
                JsDictionary<string, bool> selected = new JsDictionary<string,bool>();
                if (value != null)
                    for (var i = 0; i < value.Count; i++)
                        selected[value[i]] = true;

                view.BeginUpdate();
                try
                {
                    var items = this.View.GetItems();
                    for (var i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        bool select = selected[item.Id];
                        if (select != item.IsSelected)
                        {
                            item.IsSelected = select;
                            view.UpdateItem(item.Id, item);
                        }
                    }

                    UpdateSelectAll();
                    UpdateFlags();
                }
                finally
                {
                    view.EndUpdate();
                }
            }
        }
    }

    public static class GridSelectAllButtonHelper
    {
        public static void Update<TItem>(IDataGrid grid,
            Func<TItem, bool> getSelected)
            where TItem: class, new()
        {
            var grd = grid.As<DataGrid<TItem, object>>();
            var toolbar = grd.Element.Children(".s-Toolbar");
            if (toolbar.Length == 0)
                return;

            var btn = toolbar.GetWidget<Toolbar>()
                .FindButton("select-all-button").Find(".button-inner");

            var items = grd.View.GetItems();
            btn.ToggleClass("checked", items.Count > 0 && !items.Some(x => !getSelected(x)));
        }

        public static ToolButton Define<TItem>(
            Func<IDataGrid> getGrid,
            Func<TItem, object> getId,
            Func<TItem, bool> getSelected,
            Action<TItem, bool> setSelected,
            string text = null)
            where TItem : class, new()
        {
            return new ToolButton
            {
                Title = text ?? "Tümünü Seç",
                CssClass = "select-all-button",
                OnClick = delegate
                {
                    var grid = getGrid().As<DataGrid<TItem, object>>();
                    var view = grid.View;
                    var btn = grid.Element.Children(".s-Toolbar").GetWidget<Toolbar>()
                        .FindButton("select-all-button").Find(".button-inner");
                    var makeSelected = !btn.HasClass("checked");
                    view.BeginUpdate();
                    try
                    {
                        foreach (var item in view.GetItems())
                        {
                            setSelected(item, makeSelected);
                            view.UpdateItem(getId(item), item);
                        }
                    }
                    finally
                    {
                        view.EndUpdate();
                    }
                    btn.ToggleClass("checked", makeSelected);
                }
            };
        }
    }

    [Imported, Serializable]
    public class CheckTreeItem
    {
        public bool IsSelected { get; set; }

        public bool IsDescendantsSelected { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }
        public string ParentId { get; set; }
        public List<CheckTreeItem> Children { get; set; }
    }

    [Serializable, Reflectable]
    public class CheckTreeEditorOptions
    {
        public CheckTreeEditorOptions()
        {
            SelectAllOptionText = "Tümünü Seç";
        }

        [Hidden]
        public List<CheckTreeItem> Items { get; set; }
        [DisplayName("Tümünü Seç Metni")]
        public string SelectAllOptionText { get; set; }
        [DisplayName("Üç Durumlu Hiyerarşi")]
        public bool ThreeStateHierarchy { get; set; }
    }
}