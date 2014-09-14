
namespace BasicApplication.Navigation
{
    using BasicApplication.Administration.Entities;
    using Serenity;
    using Serenity.Navigation;
    using System;
    using System.Collections.Generic;

    public partial class NavigationModel
    {
        public List<NavigationItem> Items { get; private set; }

        public NavigationModel()
        {
            Items = TwoLevelCache.GetLocalStoreOnly("LeftNavigationModel:NavigationItems:" + (Authorization.UserId ?? -1L), TimeSpan.Zero,
                UserRow.Fields.GenerationKey,
                () => NavigationHelper.GetNavigationItems(System.Web.VirtualPathUtility.ToAbsolute));
        }
    }
}