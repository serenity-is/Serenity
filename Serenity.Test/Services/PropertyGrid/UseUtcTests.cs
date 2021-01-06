using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Serenity.Test.PropertyGrid
{
    [Collection("AvoidParallel")]
    public partial class PropertyItemHelperUseUtcTests
    {
        [Fact]
        public void Date_Field_With_No_Kind_Attribute_Should_Not_Have_Use_Utc_False()
        {
            var list = PropertyItemHelper.GetPropertyItemsFor(typeof(DateTimeTestForm));
            var item = list.FirstOrDefault(x => x.Name == "Date");
            Assert.NotNull(item);
            object useUtc;
            if (item.EditorParams != null && item.EditorParams.TryGetValue("useUtc", out useUtc))
                Assert.False((bool)useUtc);
        }

        [Fact]
        public void Date_Field_With_Unspecified_Kind_Attribute_Should_Not_Have_Use_Utc_False()
        {
            var list = PropertyItemHelper.GetPropertyItemsFor(typeof(DateTimeTestForm));
            var item = list.FirstOrDefault(x => x.Name == "Unspecified");
            Assert.NotNull(item);
            object useUtc;
            if (item.EditorParams != null && item.EditorParams.TryGetValue("useUtc", out useUtc))
                Assert.False((bool)useUtc);
        }

        [Fact]
        public void Date_Field_With_Local_Kind_Attribute_Should_Have_Use_Utc_True()
        {
            var list = PropertyItemHelper.GetPropertyItemsFor(typeof(DateTimeTestForm));
            var item = list.FirstOrDefault(x => x.Name == "Local");
            Assert.NotNull(item);
            Assert.NotNull(item.EditorParams);
            Assert.Contains("useUtc", item.EditorParams.Keys);
            Assert.True((bool)item.EditorParams["useUtc"]);
        }

        [Fact]
        public void Date_Field_With_Utc_Kind_Attribute_Should_Have_Use_Utc_True()
        {
            var list = PropertyItemHelper.GetPropertyItemsFor(typeof(DateTimeTestForm));
            var item = list.FirstOrDefault(x => x.Name == "Utc");
            Assert.NotNull(item);
            Assert.NotNull(item.EditorParams);
            Assert.Contains("useUtc", item.EditorParams.Keys);
            Assert.True((bool)item.EditorParams["useUtc"]);
        }

        [Fact]
        public void Date_Property_With_No_Kind_Attribute_Should_Not_Have_Use_Utc_False()
        {
            var list = PropertyItemHelper.GetPropertyItemsFor(typeof(DateTimeTestForm));
            var item = list.FirstOrDefault(x => x.Name == "MyDate");
            Assert.NotNull(item);
            object useUtc;
            if (item.EditorParams != null && item.EditorParams.TryGetValue("useUtc", out useUtc))
                Assert.False((bool)useUtc);
        }

        [Fact]
        public void Date_Property_With_Unspecified_Kind_Attribute_Should_Not_Have_Use_Utc_False()
        {
            var list = PropertyItemHelper.GetPropertyItemsFor(typeof(DateTimeTestForm));
            var item = list.FirstOrDefault(x => x.Name == "MyUnspecified");
            Assert.NotNull(item);
            object useUtc;
            if (item.EditorParams != null && item.EditorParams.TryGetValue("useUtc", out useUtc))
                Assert.False((bool)useUtc);
        }

        [Fact]
        public void Date_Property_With_Local_Kind_Attribute_Should_Have_Use_Utc_True()
        {
            var list = PropertyItemHelper.GetPropertyItemsFor(typeof(DateTimeTestForm));
            var item = list.FirstOrDefault(x => x.Name == "MyLocal");
            Assert.NotNull(item);
            Assert.NotNull(item.EditorParams);
            Assert.Contains("useUtc", item.EditorParams.Keys);
            Assert.True((bool)item.EditorParams["useUtc"]);
        }

        [Fact]
        public void Date_Property_With_Utc_Kind_Attribute_Should_Have_Use_Utc_True()
        {
            var list = PropertyItemHelper.GetPropertyItemsFor(typeof(DateTimeTestForm));
            var item = list.FirstOrDefault(x => x.Name == "MyUtc");
            Assert.NotNull(item);
            Assert.NotNull(item.EditorParams);
            Assert.Contains("useUtc", item.EditorParams.Keys);
            Assert.True((bool)item.EditorParams["useUtc"]);
        }

        [Fact]
        public void Date_Property_With_Only_DateTimeEditor_Attribute_Should_Not_Have_Use_Utc_False()
        {
            var list = PropertyItemHelper.GetPropertyItemsFor(typeof(DateTimeTestForm));
            var item = list.FirstOrDefault(x => x.Name == "OnlyDateTimeEditor");
            Assert.NotNull(item);
            object useUtc;
            if (item.EditorParams != null && item.EditorParams.TryGetValue("useUtc", out useUtc))
                Assert.False((bool)useUtc);
        }

        [Fact]
        public void Date_Property_With_DateTimeEditor_Attribute_AndUnspecifiedKind_Should_Not_Have_Use_Utc_False()
        {
            var list = PropertyItemHelper.GetPropertyItemsFor(typeof(DateTimeTestForm));
            var item = list.FirstOrDefault(x => x.Name == "DateTimeWithUnspecifiedKind");
            Assert.NotNull(item);
            object useUtc;
            if (item.EditorParams != null && item.EditorParams.TryGetValue("useUtc", out useUtc))
                Assert.False((bool)useUtc);
        }

        [Fact]
        public void Date_Property_With_DateTimeEditorAttribute_AndLocalKind_Should_Have_Use_Utc_True()
        {
            var list = PropertyItemHelper.GetPropertyItemsFor(typeof(DateTimeTestForm));
            var item = list.FirstOrDefault(x => x.Name == "DateTimeWithLocalKind");
            Assert.NotNull(item);
            Assert.NotNull(item.EditorParams);
            Assert.Contains("useUtc", item.EditorParams.Keys);
            Assert.True((bool)item.EditorParams["useUtc"]);
        }

        [Fact]
        public void Date_Property_With_DateTimeEditorAttribute_AndUtcKind_Should_Have_Use_Utc_True()
        {
            var list = PropertyItemHelper.GetPropertyItemsFor(typeof(DateTimeTestForm));
            var item = list.FirstOrDefault(x => x.Name == "DateTimeWithUtcKind");
            Assert.NotNull(item);
            Assert.NotNull(item.EditorParams);
            Assert.Contains("useUtc", item.EditorParams.Keys);
            Assert.True((bool)item.EditorParams["useUtc"]);
        }

        [BasedOnRow(typeof(DateTimeTestRow))]
        public class DateTimeTestForm
        {
            public DateTime Date { get; set; }
            public DateTime Unspecified { get; set; }
            public DateTime Local { get; set; }
            public DateTime Utc { get; set; }
            public DateTime MyDate { get; set; }
            [DateTimeKind(DateTimeKind.Unspecified)]
            public DateTime MyUnspecified { get; set; }
            [DateTimeKind(DateTimeKind.Local)]
            public DateTime MyLocal { get; set; }
            [DateTimeKind(DateTimeKind.Utc)]
            public DateTime MyUtc { get; set; }
            [DateTimeEditor]
            public DateTime OnlyDateTimeEditor { get; set; }
            [DateTimeEditor, DateTimeKind(DateTimeKind.Unspecified)]
            public DateTime DateTimeWithUnspecifiedKind { get; set; }
            [DateTimeEditor, DateTimeKind(DateTimeKind.Local)]
            public DateTime DateTimeWithLocalKind { get; set; }
            [DateTimeEditor, DateTimeKind(DateTimeKind.Utc)]
            public DateTime DateTimeWithUtcKind { get; set; }
        }

        public class DateTimeTestRow : Row
        {
            public DateTime? Date
            {
                get { return Fields.Date[this]; }
                set { Fields.Date[this] = value; }
            }

            [DateTimeKind(DateTimeKind.Unspecified)]
            public DateTime? Unspecified
            {
                get { return Fields.Unspecified[this]; }
                set { Fields.Unspecified[this] = value; }
            }

            [DateTimeKind(DateTimeKind.Local)]
            public DateTime? Local
            {
                get { return Fields.Local[this]; }
                set { Fields.Local[this] = value; }
            }

            [DateTimeKind(DateTimeKind.Utc)]
            public DateTime? Utc
            {
                get { return Fields.Utc[this]; }
                set { Fields.Utc[this] = value; }
            }

            public static RowFields Fields = new RowFields().Init();

            public class RowFields : RowFieldsBase
            {
                public DateTimeField Date;
                public DateTimeField Unspecified;
                public DateTimeField Local;
                public DateTimeField Utc;
            }

            public DateTimeTestRow()
                : base(Fields)
            {
            }
        }
    }
}