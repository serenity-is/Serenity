namespace Serenity.Navigation
{
    public interface INavigationModel
    {
        NavigationItem ActiveItem { get; }
        IEnumerable<NavigationItem> ActivePath { get; }
        IEnumerable<NavigationItem> Items { get; }
    }
}