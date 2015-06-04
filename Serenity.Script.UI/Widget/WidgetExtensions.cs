using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [ScriptName("WX")]
    public static class WidgetExtensions
    {
        public static TWidget GetWidget<TWidget>(this jQueryObject element) where TWidget : class
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (element.Length == 0)
                throw new Exception(String.Format("Searching for widget of type '{0}' on a non-existent element!", typeof(TWidget).FullName));

            var widget = TryGetWidget<TWidget>(element);
            if (widget == null)
                throw new Exception(String.Format("Element has no widget of type '{0}'!", typeof(TWidget).FullName));

            return widget;
        }

        [InlineCode("Serenity.WX.getWidget({widgetType})(element)")]
        public static object GetWidget(this jQueryObject element, Type widgetType)
        {
            return null;
        }

        public static TWidget TryGetWidget<TWidget>(this jQueryObject element) where TWidget : class
        {
            if (element == null)
                throw new Exception("Argument 'element' is null!");

            TWidget widget;
            if (typeof(TWidget).IsAssignableFrom(typeof(Widget)))
            {
                var widgetName = WidgetExtensions.GetWidgetName(typeof(TWidget));

                widget = element.GetDataValue(widgetName) as TWidget;
                if (widget != null)
                    return widget;
            }

            var data = element.GetData();
            if (data == null)
                return null;

            foreach (string key in data.Keys)
            {
                widget = data[key] as TWidget;
                if (widget != null)
                    return widget;
            }

            return null;
        }

        [InlineCode("Serenity.WX.tryGetWidget({widgetType})(element)")]
        public static object TryGetWidget(this jQueryObject element, Type widgetType)
        {
            return null;
        }

        public static string GetWidgetName(Type type)
        {
            return type.FullName.Replace(".", "_");
        }

        public static bool HasOriginalEvent(this jQueryEvent e)
        {
            return !Script.IsUndefined(((dynamic)e).originalEvent);
        }

        [IncludeGenericArguments(false)] // saltarelle bug ı var, değiştirme
        public static void Change<TWidget>(this TWidget widget, jQueryEventHandler handler)
            where TWidget : Widget
        {
            widget.Element.Bind("change." + widget.UniqueName, handler);
        }

        [IncludeGenericArguments(false)] // saltarelle bug ı var, değiştirme
        public static void ChangeSelect2<TWidget>(this TWidget widget, jQueryEventHandler handler)
            where TWidget : Widget
        {
            widget.Element.Bind2("change." + widget.UniqueName, (e, x) =>
            {
                if (e.HasOriginalEvent() || Q.IsFalse(x))
                {
                    handler(e);
                }
            });
        }

        public static jQueryObject GetGridField(this Widget widget)
        {
            return widget.Element.Closest(".field");
        }

        [InlineCode("{obj}.bind({eventName}, {handler})")]
        public static jQueryObject Bind2(this jQueryObject obj, string eventName, Action<jQueryEvent, dynamic> handler)
        {
            return null;
        }
        
        [Obsolete("Use Widget.Create")]
        public static TWidget Create<TWidget>(Action<jQueryObject> initElement, object options = null)
            where TWidget : Widget
        {
            return Widget.Create<TWidget>(initElement, options);
        }
    }
}
