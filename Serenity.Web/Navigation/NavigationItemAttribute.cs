using System;
using System.IO;
using System.Web;

namespace Serenity.Navigation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public abstract class NavigationItemAttribute : Attribute
    {
        protected NavigationItemAttribute(int order, string path, string url, string permission, string icon)
        {
            var idx = (path ?? "").IndexOf("/");
            if (idx >= 0)
            {
                this.Category = path.Substring(0, idx);
                this.Title = path.Substring(idx + 1);
            }
            else
                this.Title = path;

            this.Order = order;
            this.Permission = permission;
            this.IconClass = icon;
            this.Url = url;
        }

        public int Order { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string IconClass { get; set; }
        public string Permission { get; set; }
    }
}