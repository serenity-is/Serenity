using System;

namespace Serenity
{
    public class ElementAttribute : Attribute
    {
        private string html;

        public ElementAttribute(string html)
        {
            this.html = html;
        }

        public string Html
        {
            get { return this.html; }
        }
    }
}