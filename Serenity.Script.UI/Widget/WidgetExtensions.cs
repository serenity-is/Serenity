using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [ScriptName("WX"), Imported]
    public static class WidgetExtensions
    {
        static WidgetExtensions()
        {
            typeof(Widget).Prototype["changeSelect2"] =
                new Action<jQueryEventHandler>((handler) =>
                {
                    var widget = Script.This.As<Widget>();
                    widget.Element.Bind2("change." + widget.UniqueName, (e, x) =>
                    {
                        if (e.HasOriginalEvent() || Q.IsFalse(x))
                        {
                            handler(e);
                        }
                    });
                }
            );

            typeof(Widget).Prototype["change"] =
                new Action<jQueryEventHandler>((handler) =>
                {
                    var widget = Script.This.As<Widget>();
                    widget.Element.Bind("change." + widget.UniqueName, handler);
                }
            );

            typeof(Widget).Prototype["getGridField"] =
                new Func<jQueryObject>(() =>
                {
                    return Script.This.As<Widget>().Element.Closest(".field");
                }
            );

            jQuery.Instance.fn.tryGetWidget = new Func<Type, object>((widgetType) =>
            {
                var element = Script.This.As<jQueryObject>();

                object widget;
                if (typeof(Widget).IsAssignableFrom(widgetType))
                {
                    var widgetName = WidgetExtensions.GetWidgetName(widgetType);

                    widget = element.GetDataValue(widgetName);
                    if (widget != null && !widgetType.IsAssignableFrom(widget.GetType()))
                        widget = null;

                    if (widget != null)
                        return widget;
                }

                var data = element.GetData();
                if (data == null)
                    return null;

                foreach (string key in data.Keys)
                {
                    widget = data[key];
                    if (widget != null && widgetType.IsAssignableFrom(widget.GetType()))
                        return widget;
                }

                return null;
            });


            jQuery.Instance.fn.getWidget = new Func<Type, object>((widgetType) =>
            {
                var element = Script.This.As<jQueryObject>();

                if (element == null)
                    throw new ArgumentNullException("element");

                if (element.Length == 0)
                    throw new Exception(String.Format("Searching for widget of type '{0}' on a non-existent element! ({1})",
                        widgetType.FullName, element.Selector));

                var widget = TryGetWidget(element, widgetType);
                if (widget == null)
                {
                    var message = String.Format("Element has no widget of type '{0}'! If you have recently changed editor type of a property in " +
                        "a form class, or changed data type in row (which also changes editor type) your script side Form definition might be out of date. " +
                        "Make sure your project builds successfully and transform T4 templates", widgetType.FullName);

                    Q.NotifyError(message);
                    throw new Exception(message);

                }

                return widget;
            });
        }

        [InlineCode("Serenity.WX.getWidget({element}, {TWidget})")]
        public static TWidget GetWidget<TWidget>(this jQueryObject element) where TWidget : class
        {
            return null;   
        }

        [ScriptName("getWidget")]
        public static object GetWidget(this jQueryObject element, Type widgetType)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (element.Length == 0)
                throw new Exception(String.Format("Searching for widget of type '{0}' on a non-existent element! ({1})",
                    widgetType.FullName, element.Selector));

            var widget = element.As<dynamic>().tryGetWidget(widgetType);
            if (widget == null)
            {
                var message = String.Format("Element has no widget of type '{0}'! If you have recently changed editor type of a property in " +
                    "a form class, or changed data type in row (which also changes editor type) your script side Form definition might be out of date. " +
                    "Make sure your project builds successfully and transform T4 templates", widgetType.FullName);

                Q.NotifyError(message);
                throw new Exception(message);
            }

            return widget;
        }

        [InlineCode("{element}.tryGetWidget({TWidget})")]
        public static TWidget TryGetWidget<TWidget>(this jQueryObject element) where TWidget : class
        {
            return null;
        }

        [InlineCode("{element}.tryGetWidget({widgetType})")]
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
        
        [Obsolete("Use Widget.Create"), InlineCode("Serenity.Widget.create({{ type: {TWidget}, element: {initElement}, options: {options} }})")]
        public static TWidget Create<TWidget>(Action<jQueryObject> initElement, object options = null)
            where TWidget : Widget
        {
            return Widget.Create<TWidget>(initElement, options);
        }
    }
}
