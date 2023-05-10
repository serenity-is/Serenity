namespace Serenity.Tests.Entities;

[TableName("ComplexTable")]
public class ComplexRow : Row<ComplexRow.RowFields>
{
    [DisplayName("Complex ID"), Column("ComplexID"), Identity]
    public int? ID
    {
        get { return fields.ID[this]; }
        set { fields.ID[this] = value; }
    }

    [DisplayName("OverridenCaption"), Column("ManualName"), Expression("T0.OverridenExpression")]
    public string Overriden
    {
        get { return fields.Overriden[this]; }
        set { fields.Overriden[this] = value; }
    }

    [Expression("T0.Name")]
    public string Name
    {
        get { return fields.Name[this]; }
        set { fields.Name[this] = value; }
    }

    [Expression("(T0.Name + ' ' + T0.Surname)")]
    public string FullName
    {
        get { return fields.FullName[this]; }
        set { fields.FullName[this] = value; }
    }

    [ForeignKey("TheCountryTable", "TheCountryID"), LeftJoin("c")]
    public int? CountryID
    {
        get => fields.CountryID[this];
        set => fields.CountryID[this] = value;
    }

    [DisplayName("Country Name"), Expression("c.Name")]
    public string CountryName
    {
        get { return fields.CountryName[this]; }
        set { fields.CountryName[this] = value; }
    }

    [DisplayName("Concat Expression"), Concat("A", "B")]
    public string ConcatExpression
    {
        get { return fields.ConcatExpression[this]; }
        set { fields.ConcatExpression[this] = value; }
    }

    [DisplayName("Basic Expression"), Expression("SomeField")]
    public string BasicExpression
    {
        get { return fields.BasicExpression[this]; }
        set { fields.BasicExpression[this] = value; }
    }

    [DisplayName("Quoted Expression"), Expression("[SomeField]")]
    public string QuotedExpression
    {
        get { return fields.QuotedExpression[this]; }
        set { fields.QuotedExpression[this] = value; }
    }

    [DisplayName("Alias Dot Quoted Expression"), Expression("t0.[ThatField]")]
    public string AliasDotQuotedExpression
    {
        get { return fields.AliasDotQuotedExpression[this]; }
        set { fields.AliasDotQuotedExpression[this] = value; }
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

    public ComplexRow()
        : base()
    {
    }

    public ComplexRow(RowFields fields)
        : base(fields)
    {
    }
}