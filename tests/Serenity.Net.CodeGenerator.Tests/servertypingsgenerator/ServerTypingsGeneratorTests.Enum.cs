using ServerTypingsTest.Enums;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Enum_With_EnumKey_Uses_It_In_RegisterEnum()
        {
            var generator = CreateGenerator(typeof(EnumContainer));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Enums/StatusWithKey.ts").Text;

            Assert.Contains("export enum StatusWithKey", code);
            Assert.Contains("Inactive = 0", code);
            Assert.Contains("Active = 1", code);
            Assert.Contains("Deleted = 5", code);
            Assert.Contains("registerEnum(StatusWithKey, 'ServerTypingsTest.Enums.StatusWithKey', 'Test.Status');", code);
            Assert.DoesNotContain("Hidden", code);
        }

        [Fact]
        public void Enum_Without_EnumKey_Uses_FullName()
        {
            var generator = CreateGenerator(typeof(EnumContainer));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Enums/StatusWithoutKey.ts").Text;

            Assert.Contains("export enum StatusWithoutKey", code);
            Assert.Contains("registerEnum(StatusWithoutKey, 'ServerTypingsTest.Enums.StatusWithoutKey');", code);
            Assert.DoesNotContain("DoNotInclude", code);
        }
    }
}

namespace ServerTypingsTest.Enums
{
    [ScriptInclude]
    public class EnumContainer
    {
        public StatusWithKey? Status { get; set; }
        public StatusWithoutKey? Other { get; set; }
    }

    [EnumKey("Test.Status")]
    public enum StatusWithKey
    {
        Inactive = 0,
        Active = 1,
        [TransformIgnore]
        Hidden = 4,
        Deleted = 5
    }

    public enum StatusWithoutKey
    {
        [TransformIgnore]
        DoNotInclude = 0,
        Include = 1
    }
}
