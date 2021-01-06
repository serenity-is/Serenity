using Serenity.Web;
using System.Web.Mvc;
using Xunit;

namespace Serenity.Navigation.Test
{
    public class NavigationItemTest
    {
        [Fact]
        public void GetUrlFromControllerForDefaultActionWorksProperly()
        {
            var urlIndex = NavigationItemAttribute.GetUrlFromController(typeof(DummyController1), "Index");
            Assert.Equal("~/Dummy", urlIndex);
        }

        [Fact]
        public void GetUrlFromControllerForAnotherActionWorksProperly()
        {
            var urlIndex = NavigationItemAttribute.GetUrlFromController(typeof(DummyController1), "Another");
            Assert.Equal("~/Dummy/Another", urlIndex);
        }

        [Fact]
        public void GetUrlFromControllerForOverriddenActionWorksProperly()
        {
            var urlOverriden = NavigationItemAttribute.GetUrlFromController(typeof(DummyController1), "Overridden");
            Assert.Equal("~/Dummy/Some", urlOverriden);
        }

        [Fact]
        public void GetUrlFromControllerForRoutedActionWorksProperly()
        {
            var urlRooted = NavigationItemAttribute.GetUrlFromController(typeof(DummyController1), "Rooted");
            Assert.Equal("~/Dummy2", urlRooted);
        }

        [Fact]
        public void GetPermissionFromControllerForTypeWorksProperly()
        {
            var permission1 = NavigationItemAttribute.GetPermissionFromController(typeof(DummyController1), "Index");
            Assert.Equal("Admin", permission1);

            var permission2 = NavigationItemAttribute.GetPermissionFromController(typeof(DummyController1), "Another");
            Assert.Equal("Admin", permission2);
        }

        [Fact]
        public void GetPermissionFromControllerForOverridenWorksProperly()
        {
            var permission = NavigationItemAttribute.GetPermissionFromController(typeof(DummyController1), "Overridden");
            Assert.Equal("More", permission);
        }

        [RoutePrefix("Dummy"), Route("{action=index}"), PageAuthorize("Admin")]
        public class DummyController1
        {
            public ActionResult Index()
            {
                return null;
            }

            public ActionResult Another()
            {
                return null;
            }

            [Route("Some"), PageAuthorize("More")]
            public ActionResult Overridden()
            {
                return null;
            }

            [Route("~/Dummy2")]
            public ActionResult Rooted()
            {
                return null;
            }
        }
    }
}