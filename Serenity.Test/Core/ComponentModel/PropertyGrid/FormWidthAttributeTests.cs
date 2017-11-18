using Serenity.ComponentModel;
using Xunit;

namespace Serenity.Test
{
    [Collection("AvoidParallel")]
    public class FormWidthAttributeTests
    {
        [Fact]
        public void CreateWithDefaultConstructor()
        {
            var attr = new FormWidthAttribute();
            Assert.Null(attr.Value);
            Assert.Equal(0, attr.XSmall);
            Assert.Equal(0, attr.Small);
            Assert.Equal(0, attr.Medium);
            Assert.Equal(0, attr.Large);
            Assert.False(attr.UntilNext);
        }

        [Fact]
        public void CreateWithNullValue()
        {
            var attr = new FormWidthAttribute(null);
            Assert.Null(attr.Value);
            Assert.Equal(0, attr.XSmall);
            Assert.Equal(0, attr.Small);
            Assert.Equal(0, attr.Medium);
            Assert.Equal(0, attr.Large);
            Assert.False(attr.UntilNext);
        }

        [Fact]
        public void CreateWithEmptyValue()
        {
            var attr = new FormWidthAttribute(null);
            Assert.Null(attr.Value);
            Assert.Equal(0, attr.XSmall);
            Assert.Equal(0, attr.Small);
            Assert.Equal(0, attr.Medium);
            Assert.Equal(0, attr.Large);
            Assert.False(attr.UntilNext);
        }

        [Fact]
        public void CreateWithDummyValue()
        {
            var attr = new FormWidthAttribute("some-dummy-value");
            Assert.Equal("some-dummy-value", attr.Value);
            Assert.Equal(0, attr.XSmall);
            Assert.Equal(0, attr.Small);
            Assert.Equal(0, attr.Medium);
            Assert.Equal(0, attr.Large);
        }

        [Fact]
        public void CreateWithColMd6()
        {
            var attr = new FormWidthAttribute("col-md-6");
            Assert.Equal("col-md-6", attr.Value);
            Assert.Equal(0, attr.XSmall);
            Assert.Equal(0, attr.Small);
            Assert.Equal(6, attr.Medium);
            Assert.Equal(0, attr.Large);
        }

        [Fact]
        public void CreateWithColMd4Sm6()
        {
            var attr = new FormWidthAttribute("col-md-4 col-sm-6");
            Assert.Equal("col-md-4 col-sm-6", attr.Value);
            Assert.Equal(0, attr.XSmall);
            Assert.Equal(6, attr.Small);
            Assert.Equal(4, attr.Medium);
            Assert.Equal(0, attr.Large);
        }

        [Fact]
        public void CreateWithColMd4ReplaceWith6()
        {
            var attr = new FormWidthAttribute("col-md-4");
            attr.Medium = 6;
            Assert.Equal("col-md-6", attr.Value);
            Assert.Equal(0, attr.XSmall);
            Assert.Equal(0, attr.Small);
            Assert.Equal(6, attr.Medium);
            Assert.Equal(0, attr.Large);
        }

        [Fact]
        public void CreateWithColMd4AndRemove()
        {
            var attr = new FormWidthAttribute("col-md-4");
            attr.Medium = 0;
            Assert.Equal("", attr.Value);
            Assert.Equal(0, attr.XSmall);
            Assert.Equal(0, attr.Small);
            Assert.Equal(0, attr.Medium);
            Assert.Equal(0, attr.Large);
        }

        [Fact]
        public void CreateWithColMd4Sm6ReplaceWith23()
        {
            var attr = new FormWidthAttribute("col-md-4 col-sm-6");
            attr.Medium = 2;
            attr.Small = 3;
            Assert.Equal("col-md-2 col-sm-3", attr.Value);
            Assert.Equal(0, attr.XSmall);
            Assert.Equal(3, attr.Small);
            Assert.Equal(2, attr.Medium);
            Assert.Equal(0, attr.Large);
        }

        [Fact]
        public void CreateWithColMd4Sm6AndRemoveMd4()
        {
            var attr = new FormWidthAttribute("col-md-4 col-sm-6");
            attr.Medium = 0;
            Assert.Equal("col-sm-6", attr.Value);
            Assert.Equal(0, attr.XSmall);
            Assert.Equal(6, attr.Small);
            Assert.Equal(0, attr.Medium);
            Assert.Equal(0, attr.Large);
        }

        [Fact]
        public void CreateWithColMd4Sm6AndRemoveSm6()
        {
            var attr = new FormWidthAttribute("col-md-4 col-sm-6");
            attr.Small = 0;
            Assert.Equal("col-md-4", attr.Value);
            Assert.Equal(0, attr.XSmall);
            Assert.Equal(0, attr.Small);
            Assert.Equal(4, attr.Medium);
            Assert.Equal(0, attr.Large);
        }

        [Fact]
        public void CreateWithDummyValuesAndSetLg6()
        {
            var attr = new FormWidthAttribute("col-md-x col-xs-4 some-dummy-value");
            attr.Large = 6;
            Assert.Equal("col-md-x col-xs-4 some-dummy-value col-lg-6", attr.Value);
            Assert.Equal(4, attr.XSmall);
            Assert.Equal(0, attr.Small);
            Assert.Equal(0, attr.Medium);
            Assert.Equal(6, attr.Large);
        }

        [Fact]
        public void CanSetUntilNext()
        {
            var attr = new FormWidthAttribute();
            attr.UntilNext = true;
            Assert.True(attr.UntilNext);
        }
    }
}