using jQueryApi;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    public static partial class Q
    {
        public static int LayoutFillHeightValue(jQueryObject element)
        {
            var h = 0; 
            element.Parent()
                .Children()
                .Not(element)
                    .Each((i, e) =>
                    {
                        var q = J(e);
                        if (q.Is(":visible"))
                            h += q.GetOuterHeight(true);
                    });

            h = element.Parent().GetHeight() - h;
            h = h - (element.GetOuterHeight(true) - element.GetHeight());
            return h;
        }

        public static void LayoutFillHeight(jQueryObject element)
        {
            var h = LayoutFillHeightValue(element);
            string n = h + "px";
            if (element.GetCSS("height") != n)
                element.CSS("height", n);
        }

        public static void InitFullHeightGridPage(jQueryObject gridDiv)
        {
            J("body")
                .AddClass("full-height-page");

            jQueryEventHandler layout = delegate 
            {
                Q.LayoutFillHeight(gridDiv);
                gridDiv.TriggerHandler("layout");
            };

            jQuery.Window.Resize(layout);
            layout(null);
        }

        public static void TriggerLayoutOnShow(jQueryObject element)
        {
            LazyLoadHelper.ExecuteEverytimeWhenShown(element, () =>
            {
                element.TriggerHandler("layout");
            }, true);
        }

        public static void AutoFullHeight(this jQueryObject element)
        {
            element.CSS("height", "100%");
            TriggerLayoutOnShow(element);
        }
    }
}
