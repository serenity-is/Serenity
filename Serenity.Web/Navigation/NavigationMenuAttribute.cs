using System;
using System.IO;
using System.Web;

namespace Serenity.Navigation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class NavigationMenuAttribute : NavigationItemAttribute
    {
        public NavigationMenuAttribute(int order, string title, string icon = null)
            : base(order, title, null, null, icon)
        {
        }
    }
}