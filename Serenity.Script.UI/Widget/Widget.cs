using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;
using System.Reflection;
using System.Threading.Tasks;

namespace Serenity
{
    /// <summary>
    /// Base class for classes that add behaviour to HTML elements. Similar to jQuery UI widget factory.
    /// </summary>
    public abstract class Widget : ScriptContext
    {
        /// <summary>
        /// Each widget instance created gets a unique name. This static auto incrementing number is used to generate such names.
        /// </summary>
        private static int NextWidgetNumber = 0;

        protected bool? initialized;
        protected string widgetName;
        protected string uniqueName;
        protected jQueryObject element;

        /// <summary>
        /// Creates a widget on given element. Widget gets a unique name like MyNamespace_MyWidget1234.
        /// </summary>
        /// <remarks>
        /// * A data value is added to element with this unique name as key, and the widget object as value.
        /// * Only one widget instance of same class can be created on a element.
        /// * All events attached by this widget class will be unbound when element is removed from document.
        /// * Elements gets a css class like "s-MyWidget" (it has no namespace by default, but this can be customized)
        /// </remarks>
        protected Widget(jQueryObject element)
        {
            this.element = element;
            this.widgetName = WidgetExtensions.GetWidgetName(this.GetType());
            this.uniqueName = widgetName + (NextWidgetNumber++).ToString();

            if (element.GetDataValue(widgetName) != null)
                throw new Exception(String.Format("The element already has widget '{0}'!", widgetName));

            var self = this;
            element.Bind("remove." + widgetName, (e) => self.Destroy())
                .Data(widgetName, this);

            AddCssClass();
        }

        protected internal Widget Init(Action<Widget> callback = null)
        {
            if (initialized == true || !IsAsyncWidget())
            {
                initialized = true;

                if (callback != null)
                    callback(this);

                return this;
            }

            if (initialized != null)
                throw new InvalidOperationException("Widget already initializing!");

            initialized = false;

            InitializeAsync(delegate {
                initialized = true;
                if (callback != null)
                    callback(this);
            });

            return this;
        }

        protected virtual bool IsAsyncWidget()
        {
            return this is IAsyncWidget;
        }

        protected virtual void InitializeAsync(Action callback)
        {
            callback();
        }

        /// <summary>
        /// Destroys the widget. Called automatically when the widget element is removed from document
        /// </summary>
        public virtual void Destroy()
        {
            this.element.RemoveClass("s-" + this.GetType().Name);

            element.Unbind("." + widgetName)
                .RemoveData(widgetName);

            element = null;
        }

        /// <summary>
        /// Override this to change CSS class that is added to element.
        /// </summary>
        protected virtual void AddCssClass()
        {
            this.element.AddClass("s-" + this.GetType().Name);
        }

        /// <summary>
        /// The element widget is atteched to
        /// </summary>
        public jQueryObject Element 
        { 
            get { return element; } 
        }

        /// <summary>
        /// Unique name for this widget instance. Something like MyNamespace_MyWidget1234
        /// </summary>
        public string UniqueName
        {
            get { return uniqueName; }
        }

        /// <summary>
        /// Widget name for this widget class. Element has a data value with this widget name as key,
        /// and widget instance as value. It can be used to access widget instance given the
        /// element.
        /// </summary>
        public string WidgetName
        {
            get { return widgetName; }
        }

        public static jQueryObject ElementFor<TEditor>()
        {
            return ElementFor(typeof(TEditor));
        }

        public static jQueryObject ElementFor(Type editorType)
        {
            var elementAttr = editorType.GetCustomAttributes(typeof(ElementAttribute), true);
            string elementHtml = (elementAttr.Length > 0) ? elementAttr[0].As<ElementAttribute>().Html : "<input/>";
            return jQuery.FromHtml(elementHtml);
        }

        public static Widget CreateOfType(Type widgetType, Action<jQueryObject> element = null,
            object options = null, Action<Widget> init = null)
        {
            Widget widget;

            if (typeof(IDialog).IsAssignableFrom(widgetType))
            {
                widget = (Widget)Activator.CreateInstance(widgetType, options);

                if (element != null)
                    element(widget.element);
            }
            else
            {
                var e = ElementFor(widgetType);

                if (element != null)
                    element(e);

                widget = (Widget)Activator.CreateInstance(widgetType, e, options);
            }

            widget.Init(delegate
            {
                if (init != null)
                    init(widget);
            });

            return widget;
        }

        public static TWidget Create<TWidget>(Action<jQueryObject> element = null, object options = null,
                Action<TWidget> init = null) 
            where TWidget : Widget
        {
            return Widget.CreateOfType(typeof(TWidget), element, options,
                init == null ? (Action<Widget>)null : (w => init(w.As<TWidget>()))).As<TWidget>();
        }

        public static TWidget CreateInside<TWidget>(jQueryObject container, object options = null,
                Action<TWidget> init = null)
            where TWidget : Widget
        {
            return Create<TWidget>(e => container.Append(e), options, init);
        }
    }
}