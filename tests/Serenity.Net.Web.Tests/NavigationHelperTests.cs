using Microsoft.Extensions.DependencyInjection;
using Serenity.Navigation;
using Xunit;

namespace Serenity.Tests.Web
{
    public class NavigationHelperTests
    {

        [Fact]
        public void CreateGroup_WithRest_Case1()
        {
            var typeSource = new MockTypeSource(
                new NavigationMenuAttribute("A"),
                new NavigationLinkAttribute("A/B", "#", "*"),
                new NavigationLinkAttribute("C", "#", "*"),
                new NavigationMenuAttribute("D") { Include = new[] { "...rest" } });

            var permissions = new MockPermissions();
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var actual = NavigationHelper.GetNavigationItems(permissions, typeSource, serviceProvider);

            Assert.Collection(actual,
                x =>
                {
                    Assert.Equal("D", x.FullPath);
                    Assert.Collection(x.Children,
                        y => 
                        {
                            Assert.Equal("A", y.FullPath);
                            Assert.Collection(y.Children,
                                z => Assert.Equal("A/B", z.FullPath));
                        },
                        y => Assert.Equal("C", y.FullPath));
                });
        }

        [Fact]
        public void CreateGroup_WithRest_Case2()
        {
            var typeSource = new MockTypeSource(
                new NavigationMenuAttribute("A"),
                new NavigationLinkAttribute("A/B", "#", "*"),
                new NavigationLinkAttribute("C", "#", "*"),
                new NavigationMenuAttribute("D") { Include = new[] { "C", "...rest" } });

            var permissions = new MockPermissions();
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var actual = NavigationHelper.GetNavigationItems(permissions, typeSource, serviceProvider);

            Assert.Collection(actual,
                x => Assert.Equal("C", x.FullPath),
                x =>
                {
                    Assert.Equal("D", x.FullPath);
                    Assert.Collection(x.Children,
                        y =>
                        {
                            Assert.Equal("A", y.FullPath);
                            Assert.Collection(y.Children,
                                z => Assert.Equal("A/B", z.FullPath));
                        });
                });
        }

        [Fact]
        public void CreateGroup_WithInclude_Case1()
        {
            var typeSource = new MockTypeSource(
                new NavigationMenuAttribute("D") { Include = new[] { "A", "C2" } },
                new NavigationMenuAttribute("A"),
                new NavigationLinkAttribute("A/B", "#", "*"),
                new NavigationLinkAttribute("C", "#", "*"),
                new NavigationLinkAttribute("C2", "#", "*"));

            var permissions = new MockPermissions();
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var actual = NavigationHelper.GetNavigationItems(permissions, typeSource, serviceProvider);

            Assert.Collection(actual,
                x =>
                {
                    Assert.Equal("D", x.FullPath);
                    Assert.Collection(x.Children,
                        y =>
                        {
                            Assert.Equal("A", y.FullPath);
                            Assert.Collection(y.Children,
                                z => Assert.Equal("A/B", z.FullPath));
                        },
                        y => Assert.Equal("C2", y.FullPath));
                },
                x => Assert.Equal("C", x.FullPath));
        }

        [Fact]
        public void CreateGroup_WithInclude_Case2()
        {
            var typeSource = new MockTypeSource(
                new NavigationMenuAttribute("D") { Include = new[] { "A/", "C2" } },
                new NavigationMenuAttribute("A"),
                new NavigationLinkAttribute("A/B", "#", "*"),
                new NavigationLinkAttribute("C", "#", "*"),
                new NavigationLinkAttribute("C2", "#", "*"));

            var permissions = new MockPermissions();
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var actual = NavigationHelper.GetNavigationItems(permissions, typeSource, serviceProvider);

            Assert.Collection(actual,
                x =>
                {
                    Assert.Equal("D", x.FullPath);
                    Assert.Collection(x.Children,
                        y => Assert.Equal("A/B", y.FullPath),
                        y => Assert.Equal("C2", y.FullPath));
                },
                x => Assert.Equal("C", x.FullPath));
        }

        [Fact]
        public void Before_Case1()
        {
            var typeSource = new MockTypeSource(
                new NavigationMenuAttribute("A"),
                new NavigationLinkAttribute("A/X", "#", "*"),
                new NavigationLinkAttribute("A/Y", "#", "*") { Before = new[] { "X" } },
                new NavigationLinkAttribute("C", "#", "*") { Before = new[] { "A" } });

            var permissions = new MockPermissions();
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var actual = NavigationHelper.GetNavigationItems(permissions, typeSource, serviceProvider);

            Assert.Collection(actual,
                x => Assert.Equal("C", x.FullPath),
                x =>
                {
                    Assert.Equal("A", x.FullPath);
                    Assert.Collection(x.Children,
                        y => Assert.Equal("A/Y", y.FullPath),
                        y => Assert.Equal("A/X", y.FullPath));
                });
        }

        [Fact]
        public void After_Case1()
        {
            var typeSource = new MockTypeSource(
                new NavigationMenuAttribute("A") { After = new[] { "C" } },
                new NavigationLinkAttribute("A/X", "#", "*") { After = new[] { "Y" } },
                new NavigationLinkAttribute("A/Y", "#", "*"),
                new NavigationLinkAttribute("C", "#", "*"));

            var permissions = new MockPermissions();
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var actual = NavigationHelper.GetNavigationItems(permissions, typeSource, serviceProvider);

            Assert.Collection(actual,
                x => Assert.Equal("C", x.FullPath),
                x =>
                {
                    Assert.Equal("A", x.FullPath);
                    Assert.Collection(x.Children,
                        y => Assert.Equal("A/Y", y.FullPath),
                        y => Assert.Equal("A/X", y.FullPath));
                });
        }
    }
}