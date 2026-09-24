using ServerTypingsTest.MemberTypes;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Maps_Member_Types_To_TypeScript()
        {
            var generator = CreateGenerator(typeof(MemberTypeSamples));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "MemberTypes/MemberTypeSamples.ts").Text;

            Assert.Contains("StringValue?: string;", code);
            Assert.Contains("NumberValue?: number;", code);
            Assert.Contains("NullableNumberValue?: number;", code);
            Assert.Contains("BoolValue?: boolean;", code);
            Assert.Contains("DecimalValue?: number;", code);
            Assert.Contains("DateValue?: string;", code);
            Assert.Contains("DateOnlyValue?: string;", code);
            Assert.Contains("TimeSpanValue?: string;", code);
            Assert.Contains("DateTimeOffsetValue?: string;", code);
            Assert.Contains("GuidValue?: string;", code);
            Assert.Contains("ObjectValue?: any;", code);
            Assert.Contains("UserId?: any;", code);
            Assert.Contains("DynamicValue?: any;", code);
            Assert.Contains("ArrayValue?: number[];", code);
            Assert.Contains("StreamValue?: number[];", code);
            Assert.Contains("ListValue?: string[];", code);
            Assert.Contains("DictValue?: { [key: string]: number };", code);
            Assert.Contains("SortByValue?: string[];", code);
            Assert.Contains("IgnoredValue?: any;", code);
        }
    }
}

namespace ServerTypingsTest.MemberTypes
{
    [ScriptInclude]
    public class MemberTypeSamples
    {
        public string StringValue { get; set; }
        public int NumberValue { get; set; }
        public int? NullableNumberValue { get; set; }
        public bool BoolValue { get; set; }
        public decimal DecimalValue { get; set; }
        public DateTime DateValue { get; set; }
        public DateOnly DateOnlyValue { get; set; }
        public TimeSpan TimeSpanValue { get; set; }
        public DateTimeOffset DateTimeOffsetValue { get; set; }
        public Guid GuidValue { get; set; }
        public object ObjectValue { get; set; }
        [Serenity.Data.Mapping.UserIdFieldType]
        public object? UserId { get; set; }
        public dynamic DynamicValue { get; set; }
        public int[] ArrayValue { get; set; }
        public System.IO.Stream StreamValue { get; set; }
        public List<string> ListValue { get; set; }
        public Dictionary<string, int> DictValue { get; set; }
        public SortBy[] SortByValue { get; set; }
        public IgnoredType IgnoredValue { get; set; }
    }

    [TransformIgnore]
    public class IgnoredType
    {
    }
}
