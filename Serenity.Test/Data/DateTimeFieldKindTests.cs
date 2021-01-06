
namespace Serenity.Test.Data
{
    using FakeItEasy;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Serenity.Data;
    using System;
    using System.Data;
    using Xunit;

    public partial class DateTimeFieldKindTests
    {
        private JsonSerializerSettings JsonSettings(DateParseHandling dateParseHandling)
        {
            var s = new JsonSerializerSettings
            {
                DateParseHandling = dateParseHandling,
                TypeNameHandling = TypeNameHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Error,
                PreserveReferencesHandling = PreserveReferencesHandling.None
            };

            s.Converters.Add(new IsoDateTimeConverter());
            s.Converters.Add(new JsonSafeInt64Converter());
            return s;
        }

        [Fact]
        public void Auto_Created_DateTime_Fields_Are_Considered_Date_Only_By_Default()
        {
            Assert.True(DateTimeTestRow.Fields.Date.DateOnly);
            Assert.Equal(DateTimeKind.Unspecified, DateTimeTestRow.Fields.Date.DateTimeKind);
        }

        [Fact]
        public void Manually_Created_DateTime_Fields_Are_Considered_Date_Only_By_Default()
        {
            Assert.True(DateTimeTestRow.Fields.ManualDate1.DateOnly);
            Assert.Equal(DateTimeKind.Unspecified, DateTimeTestRow.Fields.ManualDate1.DateTimeKind);
        }

        [Fact]
        public void Manually_Created_DateTime_Fields_Can_Be_Resetted_To_Date_Only_After_Setting_A_DateTimeKind()
        {
            Assert.True(DateTimeTestRow.Fields.ManualDate2.DateOnly);
            Assert.Equal(DateTimeKind.Unspecified, DateTimeTestRow.Fields.ManualDate2.DateTimeKind);
        }

        [Fact]
        public void Manually_Created_DateTimeField_DateTimeKind_Can_Be_Set_To_Unspecified_By_Setting_DateOnly_False()
        {
            Assert.False(DateTimeTestRow.Fields.ManualUnspecified1.DateOnly);
            Assert.Equal(DateTimeKind.Unspecified, DateTimeTestRow.Fields.ManualUnspecified1.DateTimeKind);
        }

        [Fact]
        public void Manually_Created_DateTimeField_DateTimeKind_Can_Be_Set_To_Unspecified_By_Setting_DateTimeKind()
        {
            Assert.False(DateTimeTestRow.Fields.ManualUnspecified2.DateOnly);
            Assert.Equal(DateTimeKind.Unspecified, DateTimeTestRow.Fields.ManualUnspecified2.DateTimeKind);
        }

        [Fact]
        public void Manually_Created_DateTimeField_DateTimeKind_Can_Be_Set_To_Local_By_Setting_DateTimeKind()
        {
            Assert.False(DateTimeTestRow.Fields.ManualLocal.DateOnly);
            Assert.Equal(DateTimeKind.Local, DateTimeTestRow.Fields.ManualLocal.DateTimeKind);
        }

        [Fact]
        public void Manually_Created_DateTimeField_DateTimeKind_Can_Be_Set_To_Utc_By_Setting_DateTimeKind()
        {
            Assert.False(DateTimeTestRow.Fields.ManualUtc.DateOnly);
            Assert.Equal(DateTimeKind.Utc, DateTimeTestRow.Fields.ManualUtc.DateTimeKind);
        }

        [Fact]
        public void Auto_Created_DateTimeField_DateTimeKind_Can_Be_Set_To_Unspecified_With_Attribute()
        {
            Assert.False(DateTimeTestRow.Fields.Unspecified.DateOnly);
            Assert.Equal(DateTimeKind.Unspecified, DateTimeTestRow.Fields.Unspecified.DateTimeKind);
        }

        [Fact]
        public void Auto_Created_DateTimeField_DateTimeKind_Can_Be_Set_To_Local_With_Attribute()
        {
            Assert.False(DateTimeTestRow.Fields.Local.DateOnly);
            Assert.Equal(DateTimeKind.Local, DateTimeTestRow.Fields.Local.DateTimeKind);
        }

        [Fact]
        public void Auto_Created_DateTimeField_DateTimeKind_Can_Be_Set_To_Utc_With_Attribute()
        {
            Assert.False(DateTimeTestRow.Fields.Utc.DateOnly);
            Assert.Equal(DateTimeKind.Utc, DateTimeTestRow.Fields.Utc.DateTimeKind);
        }

        [Fact]
        public void Date_Only_DateTimeField_Does_Not_Change_DateTimeKind_While_Getting_From_Data_Reader()
        {
            DateTime testDate = DateTime.MinValue;
            var reader = A.Fake<IDataReader>();
            A.CallTo(() => reader.IsDBNull(0)).Returns(false);
            A.CallTo(() => reader.GetDateTime(0)).ReturnsLazily(() => testDate);
            A.CallTo(() => reader.GetValue(0)).ReturnsLazily(() => testDate);

            testDate = DateTime.Today;
            var dateOnlyRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Date.GetFromReader(reader, 0, dateOnlyRow);
            Assert.NotNull(dateOnlyRow.Date);
            Assert.Equal(testDate.Kind, dateOnlyRow.Date.Value.Kind);
            Assert.Equal(testDate, dateOnlyRow.Date.Value);

            testDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            var unspecifiedRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Date.GetFromReader(reader, 0, unspecifiedRow);
            Assert.NotNull(unspecifiedRow.Date);
            Assert.Equal(testDate.Kind, unspecifiedRow.Date.Value.Kind);
            Assert.Equal(testDate, unspecifiedRow.Date.Value);

            testDate = DateTime.Now;
            var localRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Date.GetFromReader(reader, 0, localRow);
            Assert.NotNull(localRow.Date);
            Assert.Equal(testDate.Kind, localRow.Date.Value.Kind);
            Assert.Equal(testDate, localRow.Date.Value);

            testDate = DateTime.UtcNow;
            var utcRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Date.GetFromReader(reader, 0, utcRow);
            Assert.NotNull(utcRow.Date);
            Assert.Equal(testDate.Kind, utcRow.Date.Value.Kind);
            Assert.Equal(testDate, utcRow.Date.Value);
        }

        [Fact]
        public void Unspecified_DateTimeField_Does_Not_Change_DateTimeKind_While_Getting_From_Data_Reader()
        {
            DateTime testDate = DateTime.MinValue;
            var reader = A.Fake<IDataReader>();
            A.CallTo(() => reader.IsDBNull(0)).Returns(false);
            A.CallTo(() => reader.GetDateTime(0)).ReturnsLazily(() => testDate);
            A.CallTo(() => reader.GetValue(0)).ReturnsLazily(() => testDate);

            testDate = DateTime.Today;
            var dateOnlyRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Unspecified.GetFromReader(reader, 0, dateOnlyRow);
            Assert.NotNull(dateOnlyRow.Unspecified);
            Assert.Equal(testDate.Kind, dateOnlyRow.Unspecified.Value.Kind);
            Assert.Equal(testDate, dateOnlyRow.Unspecified.Value);

            testDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            var unspecifiedRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Unspecified.GetFromReader(reader, 0, unspecifiedRow);
            Assert.NotNull(unspecifiedRow.Unspecified);
            Assert.Equal(testDate.Kind, unspecifiedRow.Unspecified.Value.Kind);
            Assert.Equal(testDate, unspecifiedRow.Unspecified.Value);

            testDate = DateTime.Now;
            var localRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Unspecified.GetFromReader(reader, 0, localRow);
            Assert.NotNull(localRow.Unspecified);
            Assert.Equal(testDate.Kind, localRow.Unspecified.Value.Kind);
            Assert.Equal(testDate, localRow.Unspecified.Value);

            testDate = DateTime.UtcNow;
            var utcRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Unspecified.GetFromReader(reader, 0, utcRow);
            Assert.NotNull(utcRow.Unspecified);
            Assert.Equal(testDate.Kind, utcRow.Unspecified.Value.Kind);
            Assert.Equal(testDate, utcRow.Unspecified.Value);
        }

        [Fact]
        public void Local_DateTimeField_Specifies_DateTimeKind_As_Local_While_Getting_From_Data_Reader()
        {
            DateTime testDate = DateTime.MinValue;
            var reader = A.Fake<IDataReader>();
            A.CallTo(() => reader.IsDBNull(0)).Returns(false);
            A.CallTo(() => reader.GetDateTime(0)).ReturnsLazily(() => testDate);
            A.CallTo(() => reader.GetValue(0)).ReturnsLazily(() => testDate);

            testDate = DateTime.Today;
            var dateOnlyRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Local.GetFromReader(reader, 0, dateOnlyRow);
            Assert.NotNull(dateOnlyRow.Local);
            Assert.Equal(DateTimeKind.Local, dateOnlyRow.Local.Value.Kind);
            Assert.Equal(DateTime.SpecifyKind(testDate, DateTimeKind.Local), dateOnlyRow.Local.Value);

            testDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            var unspecifiedRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Local.GetFromReader(reader, 0, unspecifiedRow);
            Assert.NotNull(unspecifiedRow.Local);
            Assert.Equal(DateTimeKind.Local, unspecifiedRow.Local.Value.Kind);
            Assert.Equal(DateTime.SpecifyKind(testDate, DateTimeKind.Local), unspecifiedRow.Local.Value);

            testDate = DateTime.Now;
            var localRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Local.GetFromReader(reader, 0, localRow);
            Assert.NotNull(localRow.Local);
            Assert.Equal(DateTimeKind.Local, localRow.Local.Value.Kind);
            Assert.Equal(DateTime.SpecifyKind(testDate, DateTimeKind.Local), localRow.Local.Value);

            testDate = DateTime.UtcNow;
            var utcRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Local.GetFromReader(reader, 0, utcRow);
            Assert.NotNull(utcRow.Local);
            Assert.Equal(DateTimeKind.Local, utcRow.Local.Value.Kind);
            Assert.Equal(DateTime.SpecifyKind(testDate, DateTimeKind.Local), utcRow.Local.Value);
        }

        [Fact]
        public void Utc_DateTimeField_Specifies_DateTimeKind_As_Utc_While_Getting_From_Data_Reader()
        {
            DateTime testDate = DateTime.MinValue;
            var reader = A.Fake<IDataReader>();
            A.CallTo(() => reader.IsDBNull(0)).Returns(false);
            A.CallTo(() => reader.GetDateTime(0)).ReturnsLazily(() => testDate);
            A.CallTo(() => reader.GetValue(0)).ReturnsLazily(() => testDate);

            testDate = DateTime.Today;
            var dateOnlyRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Utc.GetFromReader(reader, 0, dateOnlyRow);
            Assert.NotNull(dateOnlyRow.Utc);
            Assert.Equal(DateTimeKind.Utc, dateOnlyRow.Utc.Value.Kind);
            Assert.Equal(DateTime.SpecifyKind(testDate, DateTimeKind.Utc), dateOnlyRow.Utc.Value);

            testDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            var unspecifiedRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Utc.GetFromReader(reader, 0, unspecifiedRow);
            Assert.NotNull(unspecifiedRow.Utc);
            Assert.Equal(DateTimeKind.Utc, unspecifiedRow.Utc.Value.Kind);
            Assert.Equal(DateTime.SpecifyKind(testDate, DateTimeKind.Utc), unspecifiedRow.Utc.Value);

            testDate = DateTime.Now;
            var UtcRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Utc.GetFromReader(reader, 0, UtcRow);
            Assert.NotNull(UtcRow.Utc);
            Assert.Equal(DateTimeKind.Utc, UtcRow.Utc.Value.Kind);
            Assert.Equal(DateTime.SpecifyKind(testDate, DateTimeKind.Utc), UtcRow.Utc.Value);

            testDate = DateTime.UtcNow;
            var utcRow = new DateTimeTestRow();
            DateTimeTestRow.Fields.Utc.GetFromReader(reader, 0, utcRow);
            Assert.NotNull(utcRow.Utc);
            Assert.Equal(DateTimeKind.Utc, utcRow.Utc.Value.Kind);
            Assert.Equal(DateTime.SpecifyKind(testDate, DateTimeKind.Utc), utcRow.Utc.Value);
        }

        [Theory]
        [InlineData(DateParseHandling.DateTime)]
        [InlineData(DateParseHandling.DateTimeOffset)]
        public void Utc_Kind_DateTimeField_Deserializes_Utc_Time_From_Json_AsIs(DateParseHandling dateParseHandling)
        {
            var json = "{ \"Utc\": \"2017-11-23T11:40:51Z\" }";
            var row = JsonConvert.DeserializeObject<DateTimeTestRow>(json, JsonSettings(dateParseHandling));
            Assert.NotNull(row);
            Assert.Equal(new DateTime(2017, 11, 23, 11, 40, 51, DateTimeKind.Utc), row.Utc);
            Assert.Equal(DateTimeKind.Utc, row.Utc.Value.Kind);
        }

        [Theory]
        [InlineData(DateParseHandling.DateTime)]
        [InlineData(DateParseHandling.DateTimeOffset)]
        public void Utc_Kind_DateTimeField_Converts_Unspecified_Time_From_Json_To_Utc_When_DateParseHandling_Is_DateTimeOffset(DateParseHandling dateParseHandling)
        {
            var now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 978, DateTimeKind.Local);
            var utcNow = now.ToUniversalTime();
            var json = "{ \"Utc\": \"" + now.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "\" }";
            var row = JsonConvert.DeserializeObject<DateTimeTestRow>(json, JsonSettings(dateParseHandling));
            Assert.NotNull(row);
            Assert.Equal(DateTimeKind.Utc, row.Utc.Value.Kind);
            Assert.Equal(utcNow, row.Utc);
        }

        [Theory]
        [InlineData(DateParseHandling.DateTime)]
        [InlineData(DateParseHandling.DateTimeOffset)]
        public void Local_Kind_DateTimeField_Converts_Utc_Time_From_Json_To_Local_When_DateParseHandling_Is_DateTimeOffset(DateParseHandling dateParseHandling)
        {
            var now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 978, DateTimeKind.Local);
            var utcNow = now.ToUniversalTime();
            var json = "{ \"Local\": \"" + utcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + "\" }";
            var row = JsonConvert.DeserializeObject<DateTimeTestRow>(json, JsonSettings(dateParseHandling));
            Assert.NotNull(row);
            Assert.Equal(DateTimeKind.Local, row.Local.Value.Kind);
            Assert.Equal(now, row.Local);
        }

        [Theory]
        [InlineData(DateParseHandling.DateTime)]
        [InlineData(DateParseHandling.DateTimeOffset)]
        public void Local_Kind_DateTimeField_Assumes_Unspecified_Time_From_Json_As_Local(DateParseHandling dateParseHandling)
        {
            var now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 978, DateTimeKind.Local);
            var utcNow = now.ToUniversalTime();
            var json = "{ \"Local\": \"" + utcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "\" }";
            var row = JsonConvert.DeserializeObject<DateTimeTestRow>(json, JsonSettings(dateParseHandling));
            Assert.NotNull(row);
            Assert.Equal(DateTimeKind.Local, row.Local.Value.Kind);
            // when timezone unspecified json is parsed to datetimeoffset it works differently unfortunately
            // between DateTime / DateTimeOffset
            Assert.Equal(dateParseHandling == DateParseHandling.DateTimeOffset ? utcNow : now, row.Local);
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

            public DateTime? ManualDate1
            {
                get { return Fields.ManualDate1[this]; }
                set { Fields.ManualDate1[this] = value; }
            }

            public DateTime? ManualDate2
            {
                get { return Fields.ManualDate2[this]; }
                set { Fields.ManualDate2[this] = value; }
            }

            public DateTime? ManualUnspecified1
            {
                get { return Fields.ManualUnspecified1[this]; }
                set { Fields.ManualUnspecified1[this] = value; }
            }

            public DateTime? ManualUnspecified2
            {
                get { return Fields.ManualUnspecified2[this]; }
                set { Fields.ManualUnspecified2[this] = value; }
            }

            public DateTime? ManualLocal
            {
                get { return Fields.ManualLocal[this]; }
                set { Fields.ManualLocal[this] = value; }
            }

            public DateTime? ManualUtc
            {
                get { return Fields.ManualUtc[this]; }
                set { Fields.ManualUtc[this] = value; }
            }

            public static RowFields Fields = new RowFields().Init();

            public class RowFields : RowFieldsBase
            {
                public DateTimeField Date;
                public DateTimeField Unspecified;
                public DateTimeField Local;
                public DateTimeField Utc;
                public DateTimeField ManualDate1;
                public DateTimeField ManualDate2;
                public DateTimeField ManualUnspecified1;
                public DateTimeField ManualUnspecified2;
                public DateTimeField ManualLocal;
                public DateTimeField ManualUtc;

                public RowFields()
                {
                    ManualDate1 = new DateTimeField(this, "ManualDate1");

                    ManualDate2 = new DateTimeField(this, "ManualDate2");
                    ManualDate2.DateTimeKind = DateTimeKind.Utc;
                    // this should also reset dateTimeKind to null
                    ManualDate2.DateOnly = true;

                    ManualUnspecified1 = new DateTimeField(this, "ManualUnspecified1");
                    // this should also set dateTimeKind to DateTimeKind.Unspecified
                    ManualUnspecified1.DateOnly = false;

                    ManualUnspecified2 = new DateTimeField(this, "ManualUnspecified2");
                    ManualUnspecified2.DateTimeKind = DateTimeKind.Unspecified;

                    ManualLocal = new DateTimeField(this, "ManualUnspecified");
                    ManualLocal.DateTimeKind = DateTimeKind.Local;

                    ManualUtc = new DateTimeField(this, "ManualUtc");
                    ManualUtc.DateTimeKind = DateTimeKind.Utc;
                }
            }

            public DateTimeTestRow()
                : base(Fields)
            {
            }

        }
    }
}