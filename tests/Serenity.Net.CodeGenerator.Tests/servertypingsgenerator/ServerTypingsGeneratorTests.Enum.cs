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

        [Fact]
        public void Enum_With_Icons_Includes_Them_In_RegisterEnum()
        {
            var generator = CreateGenerator(typeof(EnumContainer));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Enums/StatusWithIcons.ts").Text;

            Assert.Contains("registerEnum(StatusWithIcons, 'ServerTypingsTest.Enums.StatusWithIcons', {", code);
            Assert.Contains("    enumKey: 'Test.StatusIcons',", code);
            Assert.Contains("    enumIcons: {", code);
            Assert.Contains("Minus = -1", code);
            Assert.Contains("        '-1': 'sv-minus',", code);
            Assert.Contains("        '1': 'sv-one',", code);
            Assert.Contains("        '5': 'sv-five'", code);
            // the second member with the same value keeps only its own name, not another icon
            Assert.Contains("OneAlias = 1", code);
            Assert.Equal(1, code.Split("'1': ").Length - 1);
            Assert.DoesNotContain("sv-alias", code);
            // the excluded members are neither in the enum nor in its icons
            Assert.DoesNotContain("Hidden", code);
            Assert.DoesNotContain("sv-hidden", code);
        }

        [Fact]
        public void Enum_With_Icons_But_Without_EnumKey_Only_Includes_Them()
        {
            var generator = CreateGenerator(typeof(EnumContainer));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Enums/StatusWithIconsWithoutKey.ts").Text;

            Assert.Contains("registerEnum(StatusWithIconsWithoutKey, 'ServerTypingsTest.Enums.StatusWithIconsWithoutKey', {", code);
            Assert.Contains("    enumIcons: {", code);
            Assert.Contains("        '2': 'sv-two'", code);
            Assert.DoesNotContain("enumKey:", code);
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
        public StatusWithIcons? WithIcons { get; set; }
        public StatusWithIconsWithoutKey? WithIconsWithoutKey { get; set; }
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

    [EnumKey("Test.StatusIcons")]
    public enum StatusWithIcons
    {
        [IconClass("sv-minus")]
        Minus = -1,
        None = 0,
        [IconClass("sv-one")]
        One = 1,
        [IconClass("sv-alias")]
        OneAlias = 1,
        [TransformIgnore, IconClass("sv-hidden")]
        Hidden = 4,
        [IconClass("sv-five")]
        Five = 5
    }

    public enum StatusWithIconsWithoutKey
    {
        [IconClass("sv-two")]
        Two = 2
    }
}
