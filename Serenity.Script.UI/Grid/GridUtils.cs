using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

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
    }
}