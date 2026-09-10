using System.IO;

namespace Serenity.TestUtils;

public enum SampleEnum
{
    None = 0,
    First = 1,
    Second = 2
}

[TableName("AllFields")]
public class AllFieldsRow : Row<AllFieldsRow.RowFields>
{
    public class RowFields : RowFieldsBase
    {
        public BooleanField ABoolean;
        public ByteArrayField AByteArray;
        public DateOnlyField ADateOnly;
        public DateTimeField ADateTime;
        public DateTimeOffsetField ADateTimeOffset;
        public DecimalField ADecimal;
        public DoubleField ADouble;
        public EnumField<SampleEnum> AEnum;
        public GuidField AGuid;
        public Int16Field AInt16;
        public Int32Field AInt32;
        public Int64Field AInt64;
        public JsonField<SampleJson> AJson;
        public ListField<string> AList;
        public RowField<IdNameRow> ARow;
        public RowListField<IdNameRow> ARowList;
        public SingleField ASingle;
        public StreamField AStream;
        public StringField AString;
        public TimeSpanField ATimeSpan;
        public VariantField AVariant;

        public RowFields()
        {
            ABoolean = new(this, "ABoolean");
            AByteArray = new(this, "AByteArray");
            ADateOnly = new(this, "ADateOnly");
            ADateTime = new(this, "ADateTime");
            ADateTimeOffset = new(this, "ADateTimeOffset");
            ADecimal = new(this, "ADecimal");
            ADouble = new(this, "ADouble");
            AEnum = new(this, "AEnum");
            AGuid = new(this, "AGuid");
            AInt16 = new(this, "AInt16");
            AInt32 = new(this, "AInt32");
            AInt64 = new(this, "AInt64");
            AJson = new(this, "AJson");
            AList = new(this, "AList");
            ARow = new(this, "ARow");
            ARowList = new(this, "ARowList");
            ASingle = new(this, "ASingle");
            AStream = new(this, "AStream");
            AString = new(this, "AString");
            ATimeSpan = new(this, "ATimeSpan");
            AVariant = new(this, "AVariant");
        }
    }

    public bool? ABoolean { get => fields.ABoolean[this]; set => fields.ABoolean[this] = value; }
    public byte[]? AByteArray { get => fields.AByteArray[this]; set => fields.AByteArray[this] = value; }
    public DateOnly? ADateOnly { get => fields.ADateOnly[this]; set => fields.ADateOnly[this] = value; }
    public DateTime? ADateTime { get => fields.ADateTime[this]; set => fields.ADateTime[this] = value; }
    public DateTimeOffset? ADateTimeOffset { get => fields.ADateTimeOffset[this]; set => fields.ADateTimeOffset[this] = value; }
    public decimal? ADecimal { get => fields.ADecimal[this]; set => fields.ADecimal[this] = value; }
    public double? ADouble { get => fields.ADouble[this]; set => fields.ADouble[this] = value; }
    public SampleEnum? AEnum { get => fields.AEnum[this]; set => fields.AEnum[this] = value; }
    public Guid? AGuid { get => fields.AGuid[this]; set => fields.AGuid[this] = value; }
    public short? AInt16 { get => fields.AInt16[this]; set => fields.AInt16[this] = value; }
    public int? AInt32 { get => fields.AInt32[this]; set => fields.AInt32[this] = value; }
    public long? AInt64 { get => fields.AInt64[this]; set => fields.AInt64[this] = value; }
    public SampleJson? AJson { get => fields.AJson[this]; set => fields.AJson[this] = value; }
    public List<string>? AList { get => fields.AList[this]; set => fields.AList[this] = value; }
    public IdNameRow? ARow { get => fields.ARow[this]; set => fields.ARow[this] = value; }
    public List<IdNameRow>? ARowList { get => fields.ARowList[this]; set => fields.ARowList[this] = value; }
    public float? ASingle { get => fields.ASingle[this]; set => fields.ASingle[this] = value; }
    public Stream? AStream { get => fields.AStream[this]; set => fields.AStream[this] = value; }
    public string? AString { get => fields.AString[this]; set => fields.AString[this] = value; }
    public TimeSpan? ATimeSpan { get => fields.ATimeSpan[this]; set => fields.ATimeSpan[this] = value; }
    public object? AVariant { get => fields.AVariant[this]; set => fields.AVariant[this] = value; }

    public class SampleJson
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }
}
