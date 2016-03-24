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
    [IncludeGenericArguments(false), ScriptName("CheckTreeEditor")]
    public abstract class CheckTreeEditor<TItem, TOptions> : DataGrid<TItem, TOptions>, IGetEditValue, ISetEditValue
        where TOptions: class, new()
        where TItem: CheckTreeItem, new()
    {
        private JsDictionary<string, TItem> byId;

        public CheckTreeEditor(jQueryObject div, TOptions opt)
            : base(div, opt)
        {
            div.AddClass("s-CheckTreeEditor");

            UpdateItems();
        }

        protected virtual List<TItem> GetItems()
        {
            return new List<TItem>();
        }

        protected virtual void UpdateItems()
        {
            var items = GetItems();
            var itemById = new JsDictionary<string, TItem>();
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

            view.AddData(new ListResponse<TItem>
            {
                Entities = items,
                Skip = 0,
                Take = 0,
                TotalCount = items.Count
            });

            UpdateSelectAll();
            UpdateFlags();
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
            string selectAllText = GetSelectAllText();
            if (selectAllText.IsEmptyOrNull())
                return null;

            var self = this;

            return new List<ToolButton>
            {
                GridSelectAllButtonHelper.Define<TItem>
                (
                    getGrid: () => self,
                    getId: x => x.Id,
                    getSelected: x => x.IsSelected,
                    setSelected: (x, v) => {
                        if (x.IsSelected != v)
                        {
                            x.IsSelected = v;
                            ItemSelectedChanged(x);
                        }

                    },
                    onClick: () => UpdateFlags()
                )
            };
        }

        protected virtual void ItemSelectedChanged(TItem item)
        {
        }

        protected virtual string GetSelectAllText()
        {
            return "Tümünü Seç";
        }

        protected virtual bool IsThreeStateHierarchy()
        {
            return false;
        }

        protected override SlickGrid CreateSlickGrid()
        {
            this.element.AddClass("slick-no-cell-border").AddClass("slick-no-odd-even");
            var result = base.CreateSlickGrid();
            this.element.AddClass("slick-hide-header");
            result.ResizeCanvas();
            return result;
        }

        protected override bool OnViewFilter(TItem item)
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
                    self.byId = new JsDictionary<string, TItem>();
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

        protected virtual bool GetInitialCollapse()
        {
            return false;
        }

        protected override ListResponse<TItem> OnViewProcessData(ListResponse<TItem> response)
        {
            response = base.OnViewProcessData(response);
            byId = null;
            SlickTreeHelper.SetIndents(response.Entities, getId: x => x.Id, getParentId: x => x.ParentId, setCollapsed: GetInitialCollapse());
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
                var item = Rows[row];
                var anyChanged = item.IsSelected != (!checkedOrPartial);
                view.BeginUpdate();
                try
                {
                    if (item.IsSelected != !checkedOrPartial)
                    {
                        item.IsSelected = !checkedOrPartial;
                        view.UpdateItem(item.Id, item);
                        ItemSelectedChanged(item);
                    }
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
            GridSelectAllButtonHelper.Update<TItem>(this, x => x.IsSelected);
        }

        protected  virtual void UpdateFlags()
        {
            var view = this.view;
            var items = view.GetItems();
            bool threeState = IsThreeStateHierarchy();

            if (!threeState)
                return;

            view.BeginUpdate();
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item.Children == null || item.Children.Count == 0)
                    {
                        var allsel = GetDescendantsSelected(item);
                        if (allsel != item.IsAllDescendantsSelected)
                        {
                            item.IsAllDescendantsSelected = allsel;
                            view.UpdateItem(item.Id, item);
                        }
                        continue;
                    }

                    bool allSelected = AllDescendantsSelected(item);
                    bool selected = allSelected || AnyDescendantsSelected(item);
                    
                    if (allSelected != item.IsAllDescendantsSelected ||
                        selected != item.IsSelected)
                    {
                        bool selectedChange = item.IsSelected != selected;
                        item.IsAllDescendantsSelected = allSelected;
                        item.IsSelected = selected;
                        view.UpdateItem(item.Id, item);
                        if (selectedChange)
                            ItemSelectedChanged(item);
                    }
                }
            }
            finally
            {
                view.EndUpdate();
            }
        }

        protected virtual bool GetDescendantsSelected(TItem item)
        {
            return true;
        }

        private bool SetAllSubTreeSelected(TItem item, bool selected)
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
                    ItemSelectedChanged(sub.As<TItem>());
                }
                if (sub.Children.Count > 0)
                    result = SetAllSubTreeSelected(sub.As<TItem>(), selected) | result;
            }
            return result;
        }

        private bool AllItemsSelected()
        {
            for (var i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];
                if (!row.IsSelected)
                    return false;
            }

            return Rows.Count > 0;
        }

        protected bool AllDescendantsSelected(TItem item)
        {
            if (item.Children.Count > 0)
                for (int i = 0; i < item.Children.Count; i++)
                {
                    var sub = item.Children[i];
                    if (!sub.IsSelected)
                        return false;
                    if (!AllDescendantsSelected(sub.As<TItem>()))
                        return false;
                }

            return true;
        }

        protected bool AnyDescendantsSelected(TItem item)
        {
            if (item.Children.Count > 0)
                for (int i = 0; i < item.Children.Count; i++)
                {
                    var sub = item.Children[i];
                    if (sub.IsSelected)
                        return true;
                    if (AnyDescendantsSelected(sub.As<TItem>()))
                        return true;
                }

            return false;
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
                        var item = ((object)ctx.Item).As<TItem>();

                        if (item.HideCheckBox)
                            return GetItemText(ctx);

                        bool threeState = IsThreeStateHierarchy();
                        if (item.IsSelected)
                        {
                            if (threeState && !item.IsAllDescendantsSelected)
                                cls += " partial";
                            else
                                cls += " checked";
                        }

                        return "<span class=\"" + cls + "\"></span>" + 
                            GetItemText(ctx);
                    })
                }
            };
        }

        protected virtual string GetItemText(SlickFormatterContext ctx)
        {
            return Q.HtmlEncode(ctx.Value);
        }

        protected override SlickGridOptions GetSlickOptions()
        {
            var opt = base.GetSlickOptions();
            opt.ForceFitColumns = true;
            return opt;
        }

        protected virtual void SortItems()
        {
            if (!MoveSelectedUp())
                return;

            var oldIndexes = new JsDictionary<string, int>();
            var list = this.view.GetItems();
            int i = 0;
            foreach (var x in list)
                oldIndexes[x.Id] = i++;

            list.Sort((x, y) =>
            {
                if (x.IsSelected && !y.IsSelected)
                    return -1;

                if (y.IsSelected && !x.IsSelected)
                    return 1;

                var c = Q.Externals.TurkishLocaleCompare(x.Text, y.Text);
                if (c != 0)
                    return c;

                return oldIndexes[x.Id].CompareTo(oldIndexes[y.Id]);
            });

            view.SetItems(list, true);
        }

        protected virtual bool MoveSelectedUp()
        {
            return false;
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
                    SortItems();
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
        [IncludeGenericArguments(false)]
        public static void Update<TItem>(IDataGrid grid,
            Func<TItem, bool> getSelected)
            where TItem: class, new()
        {
            var grd = grid.As<DataGrid<TItem, object>>();
            var toolbar = grd.Element.Children(".s-Toolbar");
            if (toolbar.Length == 0)
                return;

            var btn = toolbar.GetWidget<Toolbar>()
                .FindButton("select-all-button");

            var items = grd.View.GetItems();
            btn.ToggleClass("checked", items.Count > 0 && !items.Some(x => !getSelected(x)));
        }

        [IncludeGenericArguments(false)]
        public static ToolButton Define<TItem>(
            Func<IDataGrid> getGrid,
            Func<TItem, object> getId,
            Func<TItem, bool> getSelected,
            Action<TItem, bool> setSelected,
            string text = null,
            Action onClick = null)
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
                        .FindButton("select-all-button");
                    var makeSelected = !btn.HasClass("checked");
                    view.BeginUpdate();
                    try
                    {
                        foreach (var item in view.GetItems())
                        {
                            setSelected(item, makeSelected);
                            view.UpdateItem(getId(item), item);
                        }

                        if (onClick != null)
                            onClick();
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

    [Imported, IncludeGenericArguments(false), ScriptName("CheckTreeEditor")]
    public abstract class CheckTreeEditor<TOptions> : CheckTreeEditor<CheckTreeItem, TOptions>, IGetEditValue, ISetEditValue
        where TOptions : class, new()
    {
        public CheckTreeEditor(jQueryObject div, TOptions opt)
            : base(div, opt)
        {
        }
    }

    [Imported, Serializable]
    public class CheckTreeItem
    {
        public bool IsSelected { get; set; }
        public bool HideCheckBox { get; set; }
        public bool IsAllDescendantsSelected { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }
        public string ParentId { get; set; }
        public List<CheckTreeItem> Children { get; set; }
        public object Source { get; set; }
    }

    [Imported, Serializable]
    public class CheckTreeItem<TSource> : CheckTreeItem
    {
        public new TSource Source { get; set; }
        public new List<CheckTreeItem<TSource>> Children { get; set; }
    }
}