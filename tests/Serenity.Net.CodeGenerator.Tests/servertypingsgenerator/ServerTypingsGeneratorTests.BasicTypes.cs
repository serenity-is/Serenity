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

        [Fact]
        public void BasicType_Extends_A_Base_Class_Of_Serenity_That_Corelib_Has_Without_Repeating_Its_Members()
        {
            var generator = CreateGenerator(typeof(BasicTypeOfPropertyItem));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "BasicTypes/BasicTypeOfPropertyItem.ts").Text;

            Assert.Contains("import { PropertyItem } from \"@serenity-is/corelib\";", code);
            Assert.Contains("export interface BasicTypeOfPropertyItem extends PropertyItem {", code);
            Assert.Contains("IsNumeric?: boolean;", code);
            Assert.DoesNotContain("editorType?:", code);
            Assert.DoesNotContain("visibleWhen?:", code);
        }

        [Fact]
        public void BasicType_Lists_The_Members_Of_The_Classes_Between_It_And_The_Base_Class_Of_Serenity()
        {
            var generator = CreateGenerator(typeof(BasicTypeOfMiddle));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "BasicTypes/BasicTypeOfMiddle.ts").Text;

            Assert.Contains("export interface BasicTypeOfMiddle extends PropertyItem {", code);
            Assert.Contains("Own?: string;", code);
            Assert.Contains("IsNumeric?: boolean;", code);
            Assert.DoesNotContain("editorType?:", code);
        }

        [Fact]
        public void BasicType_Lists_The_Members_Of_A_Base_Class_Of_Serenity_That_Corelib_Has_No_Type_For()
        {
            var generator = CreateGeneratorModules(typeof(BasicTypeOfPropertyItem));
            // without the built-in script types, so corelib has no PropertyItem
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "BasicTypes/BasicTypeOfPropertyItem.ts").Text;

            Assert.Contains("export interface BasicTypeOfPropertyItem {", code);
            Assert.Contains("editorType?:", code);
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

    [ScriptInclude]
    public class BasicTypeOfPropertyItem : Serenity.ComponentModel.PropertyItem
    {
        public bool? IsNumeric { get; set; }
    }

    [ScriptInclude]
    public class BasicTypeOfMiddle : BasicTypeOfPropertyItem
    {
        public string Own { get; set; }
    }
}
