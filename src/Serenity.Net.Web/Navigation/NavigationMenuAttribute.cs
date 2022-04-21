namespace Serenity.Navigation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class NavigationMenuAttribute : NavigationItemAttribute
    {
        public NavigationMenuAttribute(int order, string title, string icon = null)
            : base(order, title, (string)null, null, icon)
        {
        }

        public NavigationMenuAttribute(string title, string icon = null)
            : base(int.MaxValue, title, (string)null, null, icon)
        {
        }
    }
}