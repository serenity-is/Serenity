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

        [RoutePrefix("Dummy"), Route("{action=index}")]
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

            [Route("Some")]
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