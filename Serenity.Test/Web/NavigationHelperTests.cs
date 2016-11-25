using Serenity.Web;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace Serenity.Navigation.Test
{
    public class NavigationHelperTests
    {
        [Fact]
        public void ConvertToNavigationItems_AutoCreatesFirstLevelMenus()
        {
            var source = NavigationHelper.ByCategory(new[]
            {
                new NavigationLinkAttribute(1, "A/B", "http://dummy.com", permission: null)
            });

            var actual = NavigationHelper.ConvertToNavigationItems(source, null);

            Assert.NotNull(actual);
            Assert.Equal(1, actual.Count);
            var a = actual[0];
            Assert.Equal("A", a.Title);
            Assert.Equal("A", a.FullPath);
            Assert.NotNull(a.Children);
            Assert.Equal(1, a.Children.Count);
            var b = a.Children[0];
            Assert.Equal("B", b.Title);
            Assert.Equal("A/B", b.FullPath);
            Assert.Equal(0, b.Children.Count);
        }

        [Fact]
        public void ConvertToNavigationItems_AutoCreatesSecondLevelMenus()
        {
            var source = NavigationHelper.ByCategory(new[]
            {
                new NavigationLinkAttribute(1, "A/B/C", "http://dummy.com", permission: null)
            });

            var actual = NavigationHelper.ConvertToNavigationItems(source, null);

            Assert.NotNull(actual);
            Assert.Equal(1, actual.Count);
            var a = actual[0];
            Assert.Equal("A", a.Title);
            Assert.Equal("A", a.FullPath);
            Assert.Equal(1, a.Children.Count);
            var b = a.Children[0];
            Assert.Equal("B", b.Title);
            Assert.Equal("A/B", b.FullPath);
            Assert.Equal(1, b.Children.Count);
            var c = b.Children[0];
            Assert.Equal("C", c.Title);
            Assert.Equal("A/B/C", c.FullPath);
            Assert.Equal(0, c.Children.Count);
        }
    }
}