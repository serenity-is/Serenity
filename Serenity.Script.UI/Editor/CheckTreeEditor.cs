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
        }

        void ISetEditValue.SetEditValue(dynamic source, PropertyItem property)
        {
        }

        protected override List<ToolButton> GetButtons()
        {
            string selectAllText = options.As<CheckTreeEditorOptions>().SelectAllOptionText;
            if (selectAllText.IsEmptyOrNull())
                return null;

            var self = this;

            return new List<ToolButton>
            {
                new ToolButton 
                {
                    Title = selectAllText,
                    CssClass = "select-all-button",
                    OnClick = delegate 
                    {
                        var view = self.view;
                        var btn = self.toolbar.FindButton("select-all-button").Find(".button-inner");
                        var makeSelected = !btn.HasClass("checked");
                        view.BeginUpdate();
                        try
                        {
                            foreach (var item in view.GetItems())
                            {
                                item.IsSelected = makeSelected;
                                view.UpdateItem(item.Id, item);
                            }
                        }
                        finally
                        {
                            view.EndUpdate();
                        }
                        btn.ToggleClass("checked");
                    }
                }
            };
        }

        protected override TOptions GetDefaults()
        {
            var opt = base.GetDefaults();
            opt.HidePager = true;
            opt.SelectAllOptionText = "Tümünü Seç";
            opt.Items = new List<CheckTreeItem>
            {
                new CheckTreeItem { Id = "1", ParentId = null, Text = "Dummy 1" },
                new CheckTreeItem { Id = "2", ParentId = "1", Text = "Dummy 1.2" },
                new CheckTreeItem { Id = "3", ParentId = "1", Text = "Dummy 1.3" },
                new CheckTreeItem { Id = "4", ParentId = null, Text = "Dummy 4" },
            };
            return opt;
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
                view.BeginUpdate();
                try
                {
                    item.IsSelected = !checkedOrPartial;
                    view.UpdateItem(item.Id, item);
                    SetAllSubTreeSelected(item, item.IsSelected);
                    UpdateSelectAll();
                    UpdateFlags();
                }
                finally
                {
                    view.EndUpdate();
                }
            }

        }

        protected void UpdateSelectAll()
        {
            if (toolbar == null)
                return;

            toolbar.FindButton("select-all-button").Find(".button-inner").ToggleClass("checked", AllItemsSelected());
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

        private void SetAllSubTreeSelected(CheckTreeItem item, bool selected)
        {
            for (int i = 0; i < item.Children.Count; i++)
            {
                var sub = item.Children[i];
                if (sub.IsSelected != selected)
                {
                    sub.IsSelected = selected;
                    view.UpdateItem(sub.Id, sub);
                }
                if (sub.Children.Count > 0)
                    SetAllSubTreeSelected(sub, selected);
            }
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

    [Imported, Serializable, Reflectable]
    public class CheckTreeEditorOptions : GridOptions
    {
        [Hidden]
        public List<CheckTreeItem> Items { get; set; }
        [DisplayName("Tümünü Seç Metni")]
        public string SelectAllOptionText { get; set; }
        [DisplayName("Üç Durumlu Hiyerarşi")]
        public bool ThreeStateHierarchy { get; set; }
    }
}