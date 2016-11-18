using System;
using System.Collections.Generic;

namespace Serenity.Navigation
{    
    public interface INavigationItemSource
    {
        List<NavigationItemAttribute> GetItems();
    }
}