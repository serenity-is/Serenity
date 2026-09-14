using ServerTypingsTest.BasicTypes;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void BasicType_Renames_And_Ignores_Json_Members()
        {
            var generator = CreateGenerator(typeof(BasicTypeWithJson));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "BasicTypes/BasicTypeWithJson.ts").Text;

            Assert.Contains("renamed_prop?: string;", code);
            Assert.Contains("renamed2?: string;", code);
            Assert.Contains("PublicField?: string;", code);
            Assert.DoesNotContain("Original", code);
            Assert.DoesNotContain("Hidden", code);
            Assert.DoesNotContain("IgnoredField", code);
        }

        [Fact]
        public void BasicType_Includes_Inherited_Members()
        {
            var generator = CreateGenerator(typeof(BasicTypeDerived));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "BasicTypes/BasicTypeDerived.ts").Text;

            Assert.Contains("DerivedProp?: string;", code);
            Assert.Contains("BaseProp?: string;", code);
        }
    }
}

namespace ServerTypingsTest.BasicTypes
{
    [ScriptInclude]
    public class BasicTypeWithJson
    {
        [Newtonsoft.Json.JsonProperty("renamed_prop")]
        public string Original { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("renamed2")]
        public string Original2 { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string Hidden { get; set; }

        public string PublicField;

        [Newtonsoft.Json.JsonIgnore]
        public string IgnoredField;
    }

    public class BasicTypeBase
    {
        public string BaseProp { get; set; }
    }

    [ScriptInclude]
    public class BasicTypeDerived : BasicTypeBase
    {
        public string DerivedProp { get; set; }
    }
}
