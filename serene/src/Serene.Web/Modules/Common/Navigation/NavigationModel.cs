namespace Serenity.Navigation;

public class NavigationModel : INavigationModel
{
    private NavigationItem activeItem;
    private List<NavigationItem> activePath;

    public NavigationItem ActiveItem
    {
        get => activeItem;
        set
        {
            activeItem = value;
            activePath = null;
        }
    }

    public List<NavigationItem> Items { get; set; }
    IEnumerable<NavigationItem> INavigationModel.Items => Items;

    IEnumerable<NavigationItem> INavigationModel.ActivePath
    {
        get
        {
            if (activePath == null)
            {
                var p = new List<NavigationItem>();
                var current = activeItem;
                while (current != null)
                {
                    p.Add(current);
                    current = current.Parent;
                }
                activePath = p;
            }

            return activePath;
        }
    }

}