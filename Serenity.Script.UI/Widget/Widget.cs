using jQueryApi;
using System;

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

            OnInit();
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
        /// Can be overridden to do something after widget constructor (only the base one not derived) is run
        /// </summary>
        protected virtual void OnInit()
        {
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
    }
}