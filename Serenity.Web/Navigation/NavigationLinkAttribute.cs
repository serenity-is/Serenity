using System;
using System.IO;
using System.Web;

namespace Serenity.Navigation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class NavigationLinkAttribute : NavigationItemAttribute
    {
        public NavigationLinkAttribute(int order, string path, string url, string permission, string icon = null)
            : base(order, path, url, permission, icon)
        {
        }
    }
}