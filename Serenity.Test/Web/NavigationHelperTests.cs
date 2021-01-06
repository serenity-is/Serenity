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
            Assert.StrictEqual(1, actual.Count);
            var a = actual[0];
            Assert.Equal("A", a.Title);
            Assert.Equal("A", a.FullPath);
            Assert.NotNull(a.Children);
            Assert.StrictEqual(1, a.Children.Count);
            var b = a.Children[0];
            Assert.Equal("B", b.Title);
            Assert.Equal("A/B", b.FullPath);
            Assert.StrictEqual(0, b.Children.Count);
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
            Assert.StrictEqual(1, actual.Count);
            var a = actual[0];
            Assert.Equal("A", a.Title);
            Assert.Equal("A", a.FullPath);
            Assert.StrictEqual(1, a.Children.Count);
            var b = a.Children[0];
            Assert.Equal("B", b.Title);
            Assert.Equal("A/B", b.FullPath);
            Assert.StrictEqual(1, b.Children.Count);
            var c = b.Children[0];
            Assert.Equal("C", c.Title);
            Assert.Equal("A/B/C", c.FullPath);
            Assert.StrictEqual(0, c.Children.Count);
        }

        [Fact]
        public void ConvertToNavigationItems_AutoCreatesSecondLevelMenusWithMultiple()
        {
            var source = NavigationHelper.ByCategory(new[]
            {
                new NavigationLinkAttribute(1, "A/B/C", "http://dummy.com", permission: null),
                new NavigationLinkAttribute(1, "A/B/D", "http://dummy.com", permission: null),
                new NavigationLinkAttribute(1, "A/B/E", "http://dummy.com", permission: null)
            });

            var actual = NavigationHelper.ConvertToNavigationItems(source, null);

            Assert.NotNull(actual);
            Assert.StrictEqual(1, actual.Count);

            var a = actual[0];
            Assert.Equal("A", a.Title);
            Assert.Equal("A", a.FullPath);
            Assert.StrictEqual(1, a.Children.Count);

            var b = a.Children[0];
            Assert.Equal("B", b.Title);
            Assert.Equal("A/B", b.FullPath);
            Assert.StrictEqual(3, b.Children.Count);

            var c = b.Children[0];
            Assert.Equal("C", c.Title);
            Assert.Equal("A/B/C", c.FullPath);
            Assert.StrictEqual(0, c.Children.Count);

            var d = b.Children[1];
            Assert.Equal("D", d.Title);
            Assert.Equal("A/B/D", d.FullPath);
            Assert.StrictEqual(0, d.Children.Count);

            var e = b.Children[2];
            Assert.Equal("E", e.Title);
            Assert.Equal("A/B/E", e.FullPath);
            Assert.StrictEqual(0, e.Children.Count);
        }

        [Fact]
        public void ConvertToNavigationItems_DoesntTrimItems()
        {
            var source = NavigationHelper.ByCategory(new[]
            {
                new NavigationLinkAttribute(1, "A / B / C", "http://dummy.com", permission: null)
            });

            var actual = NavigationHelper.ConvertToNavigationItems(source, null);

            Assert.NotNull(actual);
            Assert.StrictEqual(1, actual.Count);

            var a1 = actual[0];
            Assert.Equal("A ", a1.Title);
            Assert.Equal("A ", a1.FullPath);
            Assert.StrictEqual(1, a1.Children.Count);

            var a1b = a1.Children[0];
            Assert.Equal(" B ", a1b.Title);
            Assert.Equal("A / B ", a1b.FullPath);
            Assert.StrictEqual(1, a1b.Children.Count);

            var a1bc = a1b.Children[0];
            Assert.Equal(" C", a1bc.Title);
            Assert.Equal("A / B / C", a1bc.FullPath);
            Assert.StrictEqual(0, a1bc.Children.Count);
        }

        [Fact]
        public void ConvertToNavigationItems_HandlesDoubleSlashAsSingleSlashWithOneLevel()
        {
            var source = NavigationHelper.ByCategory(new[]
            {
                new NavigationLinkAttribute(1, "X//Y", "http://dummy.com", permission: null)
            });

            var actual = NavigationHelper.ConvertToNavigationItems(source, null);

            Assert.NotNull(actual);
            Assert.StrictEqual(1, actual.Count);

            var xy = actual[0];
            Assert.Equal("X/Y", xy.Title);
            Assert.Equal("X//Y", xy.FullPath);
            Assert.StrictEqual(0, xy.Children.Count);
        }

        [Fact]
        public void ConvertToNavigationItems_HandlesMultipleDoubleSlashAsSingleSlashWithOneLevel()
        {
            var source = NavigationHelper.ByCategory(new[]
            {
                new NavigationLinkAttribute(1, "X////Y", "http://dummy.com", permission: null)
            });

            var actual = NavigationHelper.ConvertToNavigationItems(source, null);

            Assert.NotNull(actual);
            Assert.StrictEqual(1, actual.Count);

            var xy = actual[0];
            Assert.Equal("X//Y", xy.Title);
            Assert.Equal("X////Y", xy.FullPath);
            Assert.StrictEqual(0, xy.Children.Count);
        }

        [Fact]
        public void ConvertToNavigationItems_HandlesDoubleSlashAsSingleSlashWithTwoLevel()
        {
            var source = NavigationHelper.ByCategory(new[]
            {
                new NavigationLinkAttribute(1, "X//Y/U//W", "http://dummy.com", permission: null)
            });

            var actual = NavigationHelper.ConvertToNavigationItems(source, null);

            Assert.NotNull(actual);
            Assert.StrictEqual(1, actual.Count);

            var xy = actual[0];
            Assert.Equal("X/Y", xy.Title);
            Assert.Equal("X//Y", xy.FullPath);
            Assert.StrictEqual(1, xy.Children.Count);

            var uw = xy.Children[0];
            Assert.Equal("U/W", uw.Title);
            Assert.Equal("X//Y/U//W", uw.FullPath);
            Assert.StrictEqual(0, uw.Children.Count);
        }

        [Fact]
        public void ConvertToNavigationItems_HandlesMultipleDoubleSlashAsSingleSlashWithTwoLevel()
        {
            var source = NavigationHelper.ByCategory(new[]
            {
                new NavigationLinkAttribute(1, "X////Y/U//W", "http://dummy.com", permission: null)
            });

            var actual = NavigationHelper.ConvertToNavigationItems(source, null);

            Assert.NotNull(actual);
            Assert.StrictEqual(1, actual.Count);

            var xy = actual[0];
            Assert.Equal("X//Y", xy.Title);
            Assert.Equal("X////Y", xy.FullPath);
            Assert.StrictEqual(1, xy.Children.Count);

            var uw = xy.Children[0];
            Assert.Equal("U/W", uw.Title);
            Assert.Equal("X////Y/U//W", uw.FullPath);
            Assert.StrictEqual(0, uw.Children.Count);
        }
    }
}