namespace Serenity.Navigation
{
    public interface INavigationItemSource
    {
        List<NavigationItemAttribute> GetItems();
    }
}