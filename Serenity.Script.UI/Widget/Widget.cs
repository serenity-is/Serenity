using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Base class for classes that add behaviour to HTML elements. Similar to jQuery UI widget factory.
    /// </summary>
    [Imported]
    public abstract class Widget : ScriptContext
    {
        protected Promise asyncPromise;
        protected string widgetName;
        protected string uniqueName;
        protected jQueryObject element;
        protected object options;

        /// <summary>
        /// Creates a widget on given element. Widget gets a unique name like MyNamespace_MyWidget1234.
        /// </summary>
        /// <remarks>
        /// * A data value is added to element with this unique name as key, and the widget object as value.
        /// * Only one widget instance of same class can be created on a element.
        /// * All events attached by this widget class will be unbound when element is removed from document.
        /// * Elements gets a css class like "s-MyWidget" (it has no namespace by default, but this can be customized)
        /// </remarks>
        protected Widget(jQueryObject element, object options = null)
        {
        }

        [ScriptName("init")]
        private Widget InitializeAndReturnThis(Action<Widget> action)
        {
            return this;
        }

        public Promise Initialize()
        {
            return asyncPromise;
        }

        public virtual bool IsAsyncWidget()
        {
            return this is IAsyncInit;
        }

        protected virtual Promise InitializeAsync()
        {
            return Promise.Void;
        }

        /// <summary>
        /// Destroys the widget. Called automatically when the widget element is removed from document
        /// </summary>
        public virtual void Destroy()
        {
        }

        /// <summary>
        /// Override this to change CSS class that is added to element.
        /// </summary>
        protected virtual void AddCssClass()
        {
        }

        protected virtual string GetCssClass()
        {
            return null;
        }

        /// <summary>
        /// The element widget is atteched to
        /// </summary>
        public jQueryObject Element 
        { 
            [InlineCode("{this}.element")]
            get { return element; } 
        }

        /// <summary>
        /// Unique name for this widget instance. Something like MyNamespace_MyWidget1234
        /// </summary>
        public string UniqueName
        {
            [InlineCode("{this}.uniqueName")]
            get { return uniqueName; }
        }

        /// <summary>
        /// Widget name for this widget class. Element has a data value with this widget name as key,
        /// and widget instance as value. It can be used to access widget instance given the
        /// element.
        /// </summary>
        public string WidgetName
        {
            [InlineCode("{this}.widgetName")]
            get { return widgetName; }
        }

        public static jQueryObject ElementFor<TEditor>()
        {
            return null;
        }

        public static jQueryObject ElementFor(Type editorType)
        {
            return null;
        }

        [IncludeGenericArguments(false)]
        [InlineCode("Serenity.Widget.create({{ type: {widgetType}, element: {element}, options: {options}, init: {init} }})")]
        public static Widget CreateOfType(Type widgetType, Action<jQueryObject> element = null,
            object options = null, Action<Widget> init = null)
        {
            return null;
        }

        [IncludeGenericArguments(false)]
        [InlineCode("Serenity.Widget.create({{ type: {TWidget}, element: {element}, options: {options}, init: {init} }})")]
        public static TWidget Create<TWidget>(Action<jQueryObject> element = null, object options = null,
                Action<TWidget> init = null)
            where TWidget : Widget
        {
            return null;
        }

        [IncludeGenericArguments(false)]
        [InlineCode("Serenity.Widget.create({{ type: {TWidget}, container: {container}, options: {options}, init: {init} }})")]
        public static TWidget CreateInside<TWidget>(jQueryObject container, object options = null,
                Action<TWidget> init = null)
            where TWidget : Widget
        {
            return null;
        }
    }
}