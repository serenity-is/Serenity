using Serenity.ComponentModel;
using Serenity.PropertyGrid;
using System.Collections.Generic;
using Xunit;

namespace Serenity.Test.PropertyGrid
{
    [Collection("AvoidParallel")]
    public partial class GetPropertyItemsForTests
    {
        private class ClassWithNoProperties
        {
        }

        [Fact]
        public void Returns_Empty_Array_For_ClassWithNoProperties()
        {
            var actual = PropertyItemHelper.GetPropertyItemsFor(typeof(ClassWithNoProperties));
            Assert.Equal(new List<PropertyItem>(), actual);
        }

        private class ClassWithField
        {
            public string Field;

            public ClassWithField()
            {
                Field = "";
            }
        }

        [Fact]
        public void IgnoresFieldDeclarations()
        {
            var actual = PropertyItemHelper.GetPropertyItemsFor(typeof(ClassWithField));
            var expected = new PropertyItem[0];

            Assert.Equal(JSON.StringifyIndented(expected), JSON.StringifyIndented(actual));
        }

        private class ClassWithOneSimpleProperty
        {
            public string Property { get; set; }
        }

        [Fact]
        public void Returns_One_Element_List_For_ClassWithOneSimpleProperty()
        {
            var actual = PropertyItemHelper.GetPropertyItemsFor(typeof(ClassWithOneSimpleProperty));
            var expected = new PropertyItem[]
            {
                new PropertyItem { Name = "Property", Title = "Property", Width = 80 }
            };

            Assert.Equal(JSON.StringifyIndented(expected), JSON.StringifyIndented(actual));
        }

        private class ClassWithUnorderedProperties
        {
            public string Property3 { get; set; }
            public string Property1 { get; set; }
            public string Property2 { get; set; }
        }

        [Fact]
        public void PreservesDeclarationOrderingFor_ClassWithUnorderedProperties()
        {
            var actual = PropertyItemHelper.GetPropertyItemsFor(typeof(ClassWithUnorderedProperties));

            var expected = new PropertyItem[]
            {
                new PropertyItem { Name = "Property3", Title = "Property3", Width = 80 },
                new PropertyItem { Name = "Property1", Title = "Property1", Width = 80 },
                new PropertyItem { Name = "Property2", Title = "Property2", Width = 80 }
            };

            Assert.Equal(JSON.StringifyIndented(expected), JSON.StringifyIndented(actual));
        }

    }
}