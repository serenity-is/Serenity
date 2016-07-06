﻿using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Serenity
{
    /// <summary>
    /// Base class for classes that add behaviour to HTML elements. Similar to jQuery UI widget factory.
    /// </summary>
    [Imported]
    public abstract class Widget : ScriptContext
    {
        /// <summary>
        /// Each widget instance created gets a unique name. This static auto incrementing number is used to generate such names.
        /// </summary>
        private static int NextWidgetNumber = 0;

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
            this.element = element;
            this.options = options ?? new object();
            this.widgetName = WidgetExtensions.GetWidgetName(this.GetType());
            this.uniqueName = widgetName + (NextWidgetNumber++).ToString();

            if (element.GetDataValue(widgetName) != null)
                throw new Exception(String.Format("The element already has widget '{0}'!", widgetName));

            var self = this;
            element.Bind("remove." + widgetName, (e) => {
                if (e.Bubbles ||
                    e.Cancelable)
                    return;

                self.Destroy();
            }).Data(widgetName, this);

            AddCssClass();

            if (IsAsyncWidget())
            {
                Window.SetTimeout(delegate()
                {
                    if (element != null && asyncPromise == null)
                        asyncPromise = this.InitializeAsync();
                }, 0);
            }
        }

        [ScriptName("init")]
        private Widget InitializeAndReturnThis(Action<Widget> action)
        {
            var promise = this.Initialize();

            if (action != null)
            {
                promise.Then(() =>
                {
                    action(this);
                });
            }

            return this;
        }

        public Promise Initialize()
        {
            if (!IsAsyncWidget())
                return Promise.Void;

            if (asyncPromise == null)
                asyncPromise = this.InitializeAsync();

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
            this.element.RemoveClass("s-" + this.GetType().Name);

            element.Unbind("." + widgetName)
                .RemoveData(widgetName);

            element = null;
            asyncPromise = null;
        }

        /// <summary>
        /// Override this to change CSS class that is added to element.
        /// </summary>
        protected virtual void AddCssClass()
        {
            this.element.AddClass(GetCssClass());
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
            return Widget.CreateOfType(typeof(TWidget), element, options,
                init == null ? (Action<Widget>)null : (w => init(w.As<TWidget>()))).As<TWidget>();
        }

        [IncludeGenericArguments(false)]
        [InlineCode("Serenity.Widget.create({{ type: {TWidget}, container: {container}, options: {options}, init: {init} }})")]
        public static TWidget CreateInside<TWidget>(jQueryObject container, object options = null,
                Action<TWidget> init = null)
            where TWidget : Widget
        {
            return Create<TWidget>(e => container.Append(e), options, init);
        }
    }
}