using System.Collections.Generic;

namespace Serenity.Plugins
{
    public interface INavigationItems
    {
        IEnumerable<NavigationEntry> GetNavigationEntries();
    }
}