using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static class GridUtils
    {
        public static void AddToggleButton(jQueryObject toolDiv, string cssClass, Action<bool> callback,
            string hint, bool initial = false)
        {
            var div = jQuery.FromHtml("<div><a href=\"#\"></a></div>")
                .AddClass("s-ToggleButton")
                .AddClass(cssClass)
                .PrependTo(toolDiv);
            
            div.Children("a")
                .Click((e) => {
                    e.PreventDefault();
                    div.ToggleClass("pressed");
                    bool pressed = div.HasClass("pressed");
                    if (callback != null)
                        callback(pressed);
                }).Attribute("title", hint ?? "");

            if (initial)
                div.AddClass("pressed");
        }

        [IncludeGenericArguments(false)]
        public static void AddIncludeDeletedToggle<TEntity>(jQueryObject toolDiv, 
            SlickRemoteView<TEntity> view, string hint = null, bool initial = false)
        {
            bool includeDeleted = false;

            var oldSubmit = view.OnSubmit;
            view.OnSubmit = (v) =>
            {
                v.Params.IncludeDeleted = includeDeleted;
                if (oldSubmit != null)
                    return oldSubmit(v);

                return true;
            };

            AddToggleButton(toolDiv,
                cssClass: "s-IncludeDeletedToggle",
                initial: initial,
                hint: hint ?? Texts.Controls.EntityGrid.IncludeDeletedToggle,
                callback: (pressed) =>
                {
                    includeDeleted = pressed;
                    view.SeekToPage = 1;
                    view.Populate();
                });

            toolDiv.Bind("remove", delegate
            {
                view.OnSubmit = null;
                oldSubmit = null;
            });
        }

        public static void AddQuickSearchInput<TEntity>(jQueryObject toolDiv,
            SlickRemoteView<TEntity> view, List<QuickSearchField> fields = null)
        {
            var oldSubmit = view.OnSubmit;
            var searchText = "";
            var searchField = "";
            
            view.OnSubmit = (v) =>
            {
                if (searchText != null && searchText.Length > 0)
                    v.Params.ContainsText = searchText;
                else
                    Script.Delete(v.Params, "ContainsText");

                if (searchField != null && searchField.Length > 0)
                    v.Params.ContainsField = searchField;
                else
                    Script.Delete(v.Params, "ContainsField");

                if (oldSubmit != null)
                    return oldSubmit(v);

                return true;
            };

            AddQuickSearchInputCustom(toolDiv, delegate(string field, string query)
            {
                searchText = query;
                searchField = field;
                view.SeekToPage = 1;
                view.Populate();
            }, fields);
        }

        public static void AddQuickSearchInputCustom(jQueryObject container, Action<string, string> onSearch,
            List<QuickSearchField> fields = null)
        {
            var input = jQuery.FromHtml("<div><input type=\"text\"/></div>")
                .AddClass("s-QuickSearchBar")
                .PrependTo(container);

            if (fields != null && fields.Count > 0)
                input.AddClass("has-quick-search-fields");

            input = input.Children();

            new QuickSearchInput(input, new QuickSearchInputOptions
            {
                Fields = fields,
                OnSearch = onSearch
            });
        }

        public static void MakeOrderable(SlickGrid grid, Action<int[], int> handleMove)
        {
            var moveRowsPlugin = new SlickRowMoveManager(new SlickRowMoveManagerOptions
            {
                CancelEditOnDrag = true
            });

            moveRowsPlugin.OnBeforeMoveRows.Subscribe((e, data) =>
            {
                for (var i = 0; i < data.rows.length; i++)
                {
                    if (data.rows[i] == data.insertBefore || data.rows[i] == data.insertBefore - 1)
                    {
                        e.StopPropagation();
                        return false;
                    }
                }
                return true;
            });

            moveRowsPlugin.OnMoveRows.Subscribe((e, data) =>
            {
                int[] rows = data.rows;
                int insertBefore = data.insertBefore;
                handleMove(rows, insertBefore);
                try { grid.SetSelectedRows(new int[0]); }
                catch { }
            });

            grid.RegisterPlugin(moveRowsPlugin);
        }

        public static void MakeOrderableWithUpdateRequest<TItem, TOptions>(DataGrid<TItem, TOptions> grid,
            Func<TItem, Int64> getId, Func<TItem, int?> getDisplayOrder,
            string service, Func<long, int, SaveRequest<TItem>> getUpdateRequest)
            where TItem : class, new()
            where TOptions : class, new()
        {
            MakeOrderable(grid.SlickGrid, (rows, insertBefore) =>
            {
                if (rows.Length == 0)
                    return;

                int order;
                var index = insertBefore;
                if (index < 0)
                    order = 1;
                else
                {
                    if (insertBefore >= grid.View.Rows.Count)
                    {
                        order = getDisplayOrder((TItem)grid.View.Rows[grid.View.Rows.Count - 1]) ?? 0;
                        if (order == 0)
                            order = insertBefore + 1;
                        else
                            order = order + 1;
                    }
                    else
                    {
                        order = getDisplayOrder((TItem)grid.View.Rows[insertBefore]) ?? 0;
                        if (order == 0)
                            order = insertBefore + 1;
                    }
                }

                int i = 0;

                Action next = null;

                next = delegate
                {
                    Q.ServiceCall(new ServiceCallOptions
                    {
                        Service = service,
                        Request = getUpdateRequest(getId(grid.View.Rows[rows[i]]), order++),
                        OnSuccess = delegate(ServiceResponse response)
                        {
                            i++;
                            if (i < rows.Length)
                                next();
                            else
                                grid.View.Populate();
                        }
                    });
                };

                next();
            });
        }

        public static List<SlickColumn> PropertyItemsToSlickColumns(List<PropertyItem> items)
        {
            var result = new List<SlickColumn>();

            if (items == null)
                return result;

            for (var i = 0; i < items.Count; i++)
                result.Add(PropertyItemToSlickColumn(items[i]));

            return result;
        }

        public static SlickColumn PropertyItemToSlickColumn(PropertyItem item)
        {
            var result = new SlickColumn();

            result.Field = item.Name;
            result.Title = Q.TryGetText(item.Title) ?? item.Title;
            result.CssClass = item.CssClass;
            result.Width = Script.IsValue(item.Width) ? item.Width : 80;
            result.MinWidth = (!Script.IsValue(item.MinWidth) || item.MinWidth == 0) ? 30 : item.MinWidth;
            result.MaxWidth = (!Script.IsValue(item.MaxWidth) || item.MaxWidth == 0) ? null : (int?)item.MaxWidth;
            result.Resizable = !Script.IsValue(item.Resizable) || item.Resizable;

            return result;
        }

        public static Type FindDialogTypeFor(string itemType)
        {
            string typeName = itemType + "Dialog";

            Type dialogType = null;
            foreach (var ns in Q.Config.RootNamespaces)
            {
                dialogType = Type.GetType(ns + "." + typeName);
                if (dialogType != null)
                    break;
            }

            if (dialogType == null)
                throw new Exception(typeName + " dialog class is not found!");

            return dialogType;
        }
    }
}