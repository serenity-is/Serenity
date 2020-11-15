using Serenity.Data;
using Serenity.Data.Mapping;
using System.ComponentModel;

namespace Serenity.Net.Data.Entity.Tests
{
    [TableName("ComplexTable")]
    public class ComplexRow : Row<ComplexRow.RowFields>
    {
        [DisplayName("Complex ID"), Column("ComplexID"), Identity]
        public int? ID
        {
            get { return Fields.ID[this]; }
            set { Fields.ID[this] = value; }
        }

        [DisplayName("OverridenCaption"), Column("ManualName"), Expression("T0.OverridenExpression")]
        public string Overriden
        {
            get { return Fields.Overriden[this]; }
            set { Fields.Overriden[this] = value; }
        }

        [Expression("T0.Name")]
        public string Name
        {
            get { return Fields.Name[this]; }
            set { Fields.Name[this] = value; }
        }

        [Expression("(T0.Name + ' ' + T0.Surname)")]
        public string FullName
        {
            get { return Fields.FullName[this]; }
            set { Fields.FullName[this] = value; }
        }

        [ForeignKey("TheCountryTable", "TheCountryID"), LeftJoin("c")]
        public int? CountryID
        {
            get { return Fields.CountryID[this]; }
            set { Fields.CountryID[this] = value; }
        }

        [DisplayName("Country Name"), Expression("c.Name")]
        public string CountryName
        {
            get { return Fields.CountryName[this]; }
            set { Fields.CountryName[this] = value; }
        }

        [DisplayName("Concat Expression"), Expression("CONCAT('A', 'B')")]
        public string ConcatExpression
        {
            get { return Fields.ConcatExpression[this]; }
            set { Fields.ConcatExpression[this] = value; }
        }

        [DisplayName("Basic Expression"), Expression("SomeField")]
        public string BasicExpression
        {
            get { return Fields.BasicExpression[this]; }
            set { Fields.BasicExpression[this] = value; }
        }

        [DisplayName("Quoted Expression"), Expression("[SomeField]")]
        public string QuotedExpression
        {
            get { return Fields.QuotedExpression[this]; }
            set { Fields.QuotedExpression[this] = value; }
        }

        [DisplayName("Alias Dot Quoted Expression"), Expression("t0.[ThatField]")]
        public string AliasDotQuotedExpression
        {
            get { return Fields.AliasDotQuotedExpression[this]; }
            set { Fields.AliasDotQuotedExpression[this] = value; }
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID;
            public StringField Name;
            public StringField Overriden;
            public Int32Field CountryID;
            public StringField CountryName;
            public StringField FullName;
            public StringField ConcatExpression;
            public StringField BasicExpression;
            public StringField QuotedExpression;
            public StringField AliasDotQuotedExpression;

            public RowFields()
            {
                Overriden = new StringField(this, "ManualName", "ManualCaption", 999, FieldFlags.Default)
                {
                    Expression = "T0.ManualExpression"
                };
            }
        }

        public ComplexRow(RowFields fields = null)
            : base(fields)
        {
        }
    }
}