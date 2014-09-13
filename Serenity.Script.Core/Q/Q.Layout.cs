using System;
using System.Html;
using jQueryApi;

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
                if (gridDiv.Parent().HasClass("page-content"))
                    gridDiv.CSS("height", "1px")
                        .CSS("overflow", "hidden");

                Q.LayoutFillHeight(gridDiv);
                gridDiv.TriggerHandler("layout");
            };

            if (J("body").HasClass("has-layout-event"))
            {
                J("body").Bind("layout", layout);
            }
            else if (Window.Instance.As<dynamic>().Metronic != null)
                Window.Instance.As<dynamic>().Metronic.addResizeHandler(layout);
            else
            {
                jQuery.Window.Resize(layout);
            }

            layout(null);
        }

        public static void AddFullHeightResizeHandler(Action<int> handler)
        {
            J("body")
                .AddClass("full-height-page");

            jQueryEventHandler layout = delegate
            {
                int avail;
                try
                {
                    avail = int.Parse(J(".page-content").GetCSS("min-height") ?? "0")
                                - int.Parse(J(".page-content").GetCSS("padding-top") ?? "0")
                                - int.Parse(J(".page-content").GetCSS("padding-bottom") ?? "0");
                }
                catch
                {
                    avail = 100;
                }

                handler(avail);
            };

            if (Window.Instance.As<dynamic>().Metronic != null)
                Window.Instance.As<dynamic>().Metronic.addResizeHandler(layout);
            else
            {
                jQuery.Window.Resize(layout);
            }

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
