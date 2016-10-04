
namespace Serenity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GridRowSelectionMixin : ScriptContext
    {
        private string idField;
        private IDataGrid grid;
        private JsDictionary<string, bool> include = new JsDictionary<string, bool>();

        public GridRowSelectionMixin(IDataGrid grid)
        {
            this.grid = grid;
            this.idField = grid.GetView().IdField;

            grid.GetGrid().OnClick.Subscribe((e, p) => {
                if (J(e.Target).HasClass("select-item"))
                {
                    e.PreventDefault();

                    var item = grid.GetView().GetItem(p.row);
                    var id = item[idField].toString();

                    if (include.ContainsKey(id))
                        include.Remove(id);
                    else
                        include[id] = true;

                    for (var i = 0; i < grid.GetView().GetLength(); i++)
                        grid.GetGrid().UpdateRow(i);

                    UpdateSelectAll();
                }
            });

            grid.GetGrid().OnHeaderClick.Subscribe((e, u) =>
            {
                if (e.IsDefaultPrevented())
                    return;

                if (J(e.Target).HasClass("select-all-items"))
                {
                    e.PreventDefault();
                    var view = grid.GetView();

                    if (include.Count > 0)
                        include.Clear();
                    else
                    {
                        foreach (var x in grid.GetView().GetItems())
                        {
                            var id = x[idField];
                            include[id.toString()] = true;
                        }
                    }
                    UpdateSelectAll();
                    grid.GetView().SetItems(grid.GetView().GetItems(), true);
                }
            });

            grid.GetView().OnRowsChanged.Subscribe((e, u) => UpdateSelectAll());
        }

        public void Clear()
        {
            include.Clear();
            UpdateSelectAll();
        }

        public void ResetCheckedAndRefresh()
        {
            include = new JsDictionary<string, bool>();
            UpdateSelectAll();
            grid.GetView().Populate();
        }

        private void UpdateSelectAll()
        {
            var selectAllButton = grid.GetElement().Find(".select-all-header .slick-column-name .select-all-items");
            if (selectAllButton != null)
                selectAllButton.ToggleClass("checked", include.Count > 0 && (grid.GetView().GetItems().Count == include.Count));
        }

        public List<string> GetSelectedKeys()
        {
            return include.Keys.ToList();
        }

        public List<Int32> GetSelectedAsInt32()
        {
            return include.Keys.Select(x => Int32.Parse(x)).ToList();
        }

        public List<Int64> GetSelectedAsInt64()
        {
            return include.Keys.Select(x => Int64.Parse(x)).ToList();
        }

        public static SlickColumn CreateSelectColumn(Func<GridRowSelectionMixin> getMixin)
        {
            return new SlickColumn
            {
                Title = "<span class=\"select-all-items check-box no-float " + ("") + "\"></span>",
                ToolTip = " ",
                Field = "__select__",
                Width = 26,
                MinWidth = 26,
                HeaderCssClass = "select-all-header",
                Sortable = false,
                Format = ctx =>
                {
                    var item = ctx.Item;
                    var mixin = getMixin();
                    if (mixin == null)
                        return "";
                    bool isChecked = mixin.include.ContainsKey(ctx.Item[mixin.idField].toString());
                    return "<span class=\"select-item check-box no-float " + (isChecked ? " checked" : "") + "\"></span>";
                }
            };
        }
    }
}