using FakeItEasy;
using Serenity.Abstractions;
using Serenity.Web;
using Xunit;

namespace Serenity.Test
{
    [Collection("AvoidParallel")]
    public partial class LogicOperatorPermissionServiceTests
    {
        [Fact]
        public void ShouldDelegateSimplePermissionsToUnderlyingOne()
        {
            var ps = A.Fake<IPermissionService>();
            var expected = false;
            A.CallTo(() => ps.HasPermission(A<string>.Ignored))
                .ReturnsLazily(s => expected);

            var lops = new LogicOperatorPermissionService(ps);
            Assert.Equal(false, lops.HasPermission(""));
            Assert.Equal(false, lops.HasPermission("A"));
            Assert.Equal(false, lops.HasPermission("B:C"));

            expected = true;
            Assert.Equal(true, lops.HasPermission(""));
            Assert.Equal(true, lops.HasPermission("A"));
            Assert.Equal(true, lops.HasPermission("B:C"));
        }

        private IPermissionService FakeService()
        {
            var ps = A.Fake<IPermissionService>();
            A.CallTo(() => ps.HasPermission("T")).Returns(true);
            A.CallTo(() => ps.HasPermission("F")).Returns(false);
            A.CallTo(() => ps.HasPermission("Y")).Returns(true);
            A.CallTo(() => ps.HasPermission("N")).Returns(false);
            return ps;
        }

        [Fact]
        public void ReturnsFalseForOrWhenAllFalse()
        {
            var lops = new LogicOperatorPermissionService(FakeService());
            Assert.Equal(false, lops.HasPermission("F|N"));
            Assert.Equal(false, lops.HasPermission("N|F"));
            Assert.Equal(false, lops.HasPermission("N|N|N"));
            Assert.Equal(false, lops.HasPermission("N|N|F|F"));
            Assert.Equal(false, lops.HasPermission("N|F|N|F"));
        }

        [Fact]
        public void ReturnsTrueForOrWhenAllTrue()
        {
            var lops = new LogicOperatorPermissionService(FakeService());
            Assert.Equal(true, lops.HasPermission("T|Y"));
            Assert.Equal(true, lops.HasPermission("Y|T"));
            Assert.Equal(true, lops.HasPermission("Y|Y|Y"));
            Assert.Equal(true, lops.HasPermission("Y|Y|T|T"));
            Assert.Equal(true, lops.HasPermission("Y|T|Y|T"));
        }

        [Fact]
        public void ReturnsTrueForOrWhenSomeTrue()
        {
            var lops = new LogicOperatorPermissionService(FakeService());
            Assert.Equal(true, lops.HasPermission("T|F"));
            Assert.Equal(true, lops.HasPermission("Y|N"));
            Assert.Equal(true, lops.HasPermission("N|Y|N"));
            Assert.Equal(true, lops.HasPermission("F|F|T|T"));
            Assert.Equal(true, lops.HasPermission("N|Y|N|F|N|N"));
        }

        [Fact]
        public void ReturnsFalseForAndWhenAllFalse()
        {
            var lops = new LogicOperatorPermissionService(FakeService());
            Assert.Equal(false, lops.HasPermission("F&N"));
            Assert.Equal(false, lops.HasPermission("N&F"));
            Assert.Equal(false, lops.HasPermission("N&N&N"));
            Assert.Equal(false, lops.HasPermission("N&N&F&F"));
            Assert.Equal(false, lops.HasPermission("N&F&N&F"));
        }

        [Fact]
        public void ReturnsTrueForAndWhenAllTrue()
        {
            var lops = new LogicOperatorPermissionService(FakeService());
            Assert.Equal(true, lops.HasPermission("T&Y"));
            Assert.Equal(true, lops.HasPermission("Y&T"));
            Assert.Equal(true, lops.HasPermission("Y&Y&Y"));
            Assert.Equal(true, lops.HasPermission("Y&Y&T&T"));
            Assert.Equal(true, lops.HasPermission("Y&T&Y&T"));
        }

        [Fact]
        public void ReturnsFalseForAndWhenSomeFalse()
        {
            var lops = new LogicOperatorPermissionService(FakeService());
            Assert.Equal(false, lops.HasPermission("T&F"));
            Assert.Equal(false, lops.HasPermission("Y&N"));
            Assert.Equal(false, lops.HasPermission("T&Y&N"));
            Assert.Equal(false, lops.HasPermission("T&T&F&T"));
            Assert.Equal(false, lops.HasPermission("N&Y&N&F&N&N"));
        }

        [Fact]
        public void AndTakesPrecedenceOverOr()
        {
            var lops = new LogicOperatorPermissionService(FakeService());
            Assert.Equal(false, lops.HasPermission("F|T&F"));
            Assert.Equal(false, lops.HasPermission("F|F&T"));
            Assert.Equal(true, lops.HasPermission("T|F&T"));
            Assert.Equal(false, lops.HasPermission("F&T|F&T"));
            Assert.Equal(false, lops.HasPermission("T&F|F|F&T"));
            Assert.Equal(false, lops.HasPermission("T&T&T&F|F&F&F&T"));
            Assert.Equal(true, lops.HasPermission("T&T&T&F|T&T|F&F&F&T"));
        }
    }
}