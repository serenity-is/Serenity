using jQueryApi;
using System;

namespace Serenity
{
    public abstract class Widget
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

            if (element.GetDataValue("widgetName") != null)
                throw new Exception(String.Format("Element already has widget '{0}'!", widgetName));

            var self = this;
            element.Bind("remove." + widgetName, (e) => self.Destroy())
                .Data(widgetName, this);
        }

        public virtual void Destroy()
        {
            element.Unbind("." + widgetName)
                .RemoveData(widgetName);

            element = null;
        }

        public jQueryObject Element { get { return element; } }
        
        public static void Create()
        {
        }

        public string WidgetName
        {
            get { return widgetName; }
        }
    }
}