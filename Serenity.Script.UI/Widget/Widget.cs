using jQueryApi;
using System;

namespace Serenity
{
    public abstract class Widget : ScriptContext
    {
        private static int NextWidgetNumber = 0;
        protected string widgetName;
        protected string uniqueName;
        protected jQueryObject element;

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

            if (this is ICustomValidate)
                this.element.AddClass("customValidate");

            OnInit();
        }

        protected virtual void AddCssClass()
        {
            this.element.AddClass("s-" + this.GetType().Name);
        }

        protected virtual void OnInit()
        {
        }

        public virtual void Destroy()
        {
            this.element.RemoveClass("s-" + this.GetType().Name);

            element.Unbind("." + widgetName)
                .RemoveData(widgetName);

            element = null;
        }

        public jQueryObject Element { get { return element; } }

        public string UniqueName
        {
            get { return uniqueName; }
        }

        public string WidgetName
        {
            get { return widgetName; }
        }
    }
}