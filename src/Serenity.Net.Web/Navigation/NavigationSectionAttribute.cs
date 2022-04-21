namespace Serenity.Navigation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class NavigationSectionAttribute : NavigationGroupAttribute
    {
        public NavigationSectionAttribute(int order, string title, string icon = null)
            : base(order, title, icon)
        {
        }

        public NavigationSectionAttribute(string title, string icon = null)
            : this(int.MaxValue, title, icon)
        {
        }
    }
}