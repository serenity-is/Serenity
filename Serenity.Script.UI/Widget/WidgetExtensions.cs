using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public delegate string CustomValidationRule(jQueryObject element);

    [ScriptName("WX")]
    public static class WidgetExtensions
    {
        public static TWidget GetWidget<TWidget>(this jQueryObject element) where TWidget : Widget
        {
            var widget = TryGetWidget<TWidget>(element);
            if (widget == null)
                throw new Exception(String.Format("Element has no widget of type '{0}'!", GetWidgetName(typeof(TWidget))));

            return widget;
        }

        public static object GetWidget(this jQueryObject element, Type widgetType)
        {
            var widget = TryGetWidget(element, widgetType);
            if (widget == null)
                throw new Exception(String.Format("Element has no widget of type '{0}'!", GetWidgetName(widgetType)));

            return widget;
        }

        public static TWidget TryGetWidget<TWidget>(this jQueryObject element) where TWidget : Widget
        {
            if (element == null)
                throw new Exception("Argument 'element' is null!");

            var widgetName = WidgetExtensions.GetWidgetName(typeof(TWidget));

            return element.GetDataValue(widgetName) as TWidget;
        }

        public static object TryGetWidget(this jQueryObject element, Type widgetType)
        {
            if (widgetType == null)
                throw new Exception("Argument 'widgetType' is null!");

            if (element == null)
                throw new Exception("Argument 'element' is null!");

            var widgetName = WidgetExtensions.GetWidgetName(widgetType);
            var widget = element.GetDataValue(widgetName);

            if (widget != null && !widgetType.IsAssignableFrom(widget.GetType()))
                return null;

            return widget;
        }

        public static string GetWidgetName(Type type)
        {
            return type.FullName.Replace(".", "_");
        }

        public static bool HasOriginalEvent(this jQueryEvent e)
        {
            return !Script.IsUndefined(((dynamic)e).originalEvent);
        }

        public static bool ValidateElement(this jQueryValidator validator, Widget widget)
        {
            return validator.ValidateElement(widget.Element[0]);
        }

        [IncludeGenericArguments(false)] // saltarelle bug ı var, değiştirme
        public static void Change<TWidget>(this TWidget widget, jQueryEventHandler handler)
            where TWidget: Widget
        {
            widget.Element.Bind("change." + widget.UniqueName, handler);
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

        public static jQueryObject AddValidationRule(this Widget widget, string eventClass,
            Func<jQueryObject, string> rule)
        {
            return AddValidationRule(widget.Element, eventClass, rule);
        }

        public static jQueryObject AddValidationRule(this jQueryObject element, string eventClass,
            Func<jQueryObject, string> rule)
        {
            if (element.Length == 0)
                return element;

            if (rule == null)
                throw new Exception("rule is null!");

            element.AddClass("customValidate")
                .Bind("customValidate." + eventClass, rule.As<jQueryEventHandler>());

            return element;
        }

        public static jQueryObject RemoveValidationRule(this jQueryObject element, string eventClass)
        {
            element.Unbind("customValidate." + eventClass);
            return element;
        }

        public static jQueryObject CreateElementFor<TEditor>()
        {
            return CreateElementFor(typeof(TEditor));
        }

        public static jQueryObject CreateElementFor(this Type editorType)
        {
            var elementAttr = editorType.GetCustomAttributes(typeof(ElementAttribute), true);
            string elementHtml = (elementAttr.Length > 0) ? elementAttr[0].As<ElementAttribute>().Html : "<input/>";
            return jQuery.FromHtml(elementHtml);
        }

        public static TWidget Create<TWidget>(Action<jQueryObject> initElement, object options = null)
            where TWidget : Widget
        {
            var element = CreateElementFor(typeof(TWidget));
            
            if (initElement != null)
                initElement(element);

            return (TWidget)Activator.CreateInstance(typeof(TWidget), element, options);
        }
    }
}
