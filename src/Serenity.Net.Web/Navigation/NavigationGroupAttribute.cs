namespace Serenity.Navigation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class NavigationGroupAttribute : NavigationItemAttribute
    {
        public NavigationGroupAttribute(int order, string title, string icon = null)
            : base(order, title, (string)null, null, icon)
        {
        }

        public NavigationGroupAttribute(string title, string icon = null)
            : this(int.MaxValue, title, icon)
        {
        }

        /// <summary>
        /// This is a list used to move items that are not normally
        /// under this item based on path (to create groups).
        /// For example, if this item is named A,
        /// and want to move all menus under B/.. or C/.. to A,
        /// the list should be ["B/", "C/"]. To move B and C themselves under A,
        /// list should be ["B", "C"].
        /// </summary>
        public string[] Include { get; set; }

        /// <summary>
        /// This group automatically includes siblings that does not match any other groups
        /// </summary>
        public bool Default { get; set; }
    }
}