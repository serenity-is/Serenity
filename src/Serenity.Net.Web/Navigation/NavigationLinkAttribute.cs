namespace Serenity.Navigation
{
    /// <summary>
    /// A navigation item with a link
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class NavigationLinkAttribute : NavigationItemAttribute
    {
        /// <summary>
        /// Creates a new instance of the attribute
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="path">Path</param>
        /// <param name="url">URL</param>
        /// <param name="permission">Permission</param>
        /// <param name="icon">Icon</param>
        public NavigationLinkAttribute(int order, string path, string url, object permission, string icon = null)
            : base(order, path, url, permission, icon)
        {
        }

        /// <summary>
        /// Creates a new instance of the attribute
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="path">Path</param>
        /// <param name="controller">Controller to get URL and action from</param>
        /// <param name="icon">Icon</param>
        /// <param name="action">Action name</param>
        public NavigationLinkAttribute(int order, string path, Type controller, string icon = null, string action = "Index")
            : base(order, path, controller, icon, action)
        {
        }

        /// <summary>
        /// Creates a new instance of the attribute
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="url">URL</param>
        /// <param name="permission">Permission</param>
        /// <param name="icon">Icon</param>
        public NavigationLinkAttribute(string path, string url, object permission, string icon = null)
            : base(int.MaxValue, path, url, permission, icon)
        {
        }

        /// <summary>
        /// Creates a new instance of the attribute
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="controller">Controller to get URL and action from</param>
        /// <param name="icon">Icon</param>
        /// <param name="action">Action name</param>
        public NavigationLinkAttribute(string path, Type controller, string icon = null, string action = "Index")
            : base(int.MaxValue, path, controller, icon, action)
        {
        }
    }
}