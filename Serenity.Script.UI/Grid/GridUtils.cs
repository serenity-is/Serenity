using jQueryApi;
using System;
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
                hint: hint ?? "silinmiş kayıtları da göster",
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
            SlickRemoteView<TEntity> view)
        {
            var oldSubmit = view.OnSubmit;
            var searchText = "";
            
            view.OnSubmit = (v) =>
            {
                if (searchText != null && searchText.Length > 0)
                    v.Params.ContainsText = searchText;
                else
                    Type.DeleteField(v.Params, "ContainsText");

                if (oldSubmit != null)
                    return oldSubmit(v);

                return true;
            };

            AddQuickSearchInputCustom(toolDiv, delegate(string query)
            {
                searchText = query;
                view.SeekToPage = 1;
                view.Populate();
            });
        }

        private static void AddQuickSearchInputCustom(jQueryObject container, Action<string> onSearch)
        {
            var input = jQuery.FromHtml("<div><input type=\"text\"/></div>")
                .AddClass("s-QuickSearchBar")
                .PrependTo(container)
                .Children();

            new QuickSearchInput(input, new QuickSearchInputOptions
            {
                OnSearch = onSearch
            });
        }
    }
}