using Microsoft.AspNetCore.Mvc;
using ServerTypingsTest.EditorDefaults;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        private static string GenerateEditorDefaults(Type attributeType, string identifier)
        {
            var result = CreateGenerator(attributeType).Run();
            return Assert.Single(result, x => x.Filename == "EditorDefaults/" + identifier + ".ts").Text;
        }

        [Fact]
        public void EditorDefaults_Are_Generated_From_Editor_Template()
        {
            var code = GenerateEditorDefaults(typeof(TemplatedEditorAttribute), "TemplatedEditorDefaults");

            Assert.Contains("import { ServiceLookupEditorOptions } from \"@serenity-is/corelib\";", code);
            Assert.Contains("export const TemplatedEditorDefaults: ServiceLookupEditorOptions = {", code);
            Assert.Contains("service: \"Test/LookupItems/ListItems\"", code);
            Assert.Contains("capabilities: " + (int)(ListRequestCapabilities.SourceAdapter & ~ListRequestCapabilities.DistinctFields), code);
            Assert.Contains("idField: \"Key\"", code);
            Assert.Contains("textField: \"Key\"", code);
            Assert.Contains("iconField: \"IconClass\"", code);
            Assert.Contains("inplaceAdd: true", code);
            Assert.Contains("includeColumns: [\"Name\", \"IconClass\"]", code);
            Assert.EndsWith("};", code.TrimEnd());
        }

        [Fact]
        public void EditorDefaults_Use_Default_Action_And_Omit_Unknown_Capabilities()
        {
            var code = GenerateEditorDefaults(typeof(DefaultActionEditorAttribute), "DefaultActionEditorDefaults");

            Assert.Contains("service: \"Test/LookupItems/List\"", code);
            Assert.DoesNotContain("capabilities", code);
            Assert.Contains("idField: \"Key\"", code);
            Assert.Contains("textField: \"Name\"", code);
        }

        [Fact]
        public void EditorDefaults_Prefer_ListLookup_Action()
        {
            var code = GenerateEditorDefaults(typeof(ListLookupEditorAttribute), "ListLookupEditorDefaults");

            Assert.Contains("service: \"Test/LookupWithListLookup/ListLookup\"", code);
            Assert.Contains("capabilities: " + (int)ListRequestCapabilities.Baseline, code);
        }

        [Fact]
        public void EditorDefaults_Use_Explicit_Service_And_Fields()
        {
            var code = GenerateEditorDefaults(typeof(ExplicitServiceEditorAttribute), "ExplicitServiceEditorDefaults");

            Assert.Contains("service: \"Custom/Items/List\"", code);
            Assert.Contains("idField: \"Code\"", code);
            Assert.Contains("textField: \"Title\"", code);
        }

        [Fact]
        public void EditorDefaults_Infer_Fields_From_Id_And_Name_Property_Attributes()
        {
            var code = GenerateEditorDefaults(typeof(AttributedItemEditorAttribute), "AttributedItemEditorDefaults");

            Assert.Contains("idField: \"ItemNo\"", code);
            Assert.Contains("textField: \"Caption\"", code);
        }

        [Fact]
        public void EditorDefaults_Are_Not_Generated_When_Template_Is_Not_On_An_Editor_Attribute()
        {
            // no visible editor attribute base class, and no client side editor for the generated attribute
            var result = CreateGenerator(typeof(LookupTemplateAttribute), typeof(NotAnEditorAttribute)).Run();

            Assert.DoesNotContain(result, x => x.Filename.Contains("LookupTemplate", StringComparison.Ordinal));
            Assert.DoesNotContain(result, x => x.Filename.Contains("NotAnEditor", StringComparison.Ordinal));
        }

        [Fact]
        public void EditorDefaults_Are_Not_Generated_For_A_Template_Of_Another_Family()
        {
            var result = CreateGenerator(typeof(MismatchedFamilyEditorAttribute)).Run();

            Assert.DoesNotContain(result, x => x.Filename.Contains("MismatchedFamily", StringComparison.Ordinal));
        }

        [Fact]
        public void EditorDefaults_Are_Generated_For_Lookup_Editor_Template()
        {
            var code = GenerateEditorDefaults(typeof(ClientLookupEditorAttribute), "ClientLookupEditorDefaults");

            Assert.Contains("import { LookupEditorOptions } from \"@serenity-is/corelib\";", code);
            Assert.Contains("export const ClientLookupEditorDefaults: LookupEditorOptions = {", code);
            Assert.Contains("lookupKey: \"Test.Items\"", code);
            Assert.Contains("inplaceAdd: true", code);
            Assert.Contains("async: true", code);
        }

        [Fact]
        public void EditorDefaults_Determine_Lookup_Key_From_Lookup_Script_Of_Lookup_Type()
        {
            Assert.Contains("lookupKey: \"Test.ExplicitLookupSource\"",
                GenerateEditorDefaults(typeof(ExplicitLookupTypeEditorAttribute), "ExplicitLookupTypeEditorDefaults"));
            Assert.Contains("lookupKey: \"EditorDefaults.AutoLookupSource\"",
                GenerateEditorDefaults(typeof(AutoLookupTypeEditorAttribute), "AutoLookupTypeEditorDefaults"));
        }

        [Fact]
        public void EditorDefaults_Are_Generated_For_Attribute_Of_A_Client_Side_Editor()
        {
            // a source generator doesn't see the base class of an attribute generated for a client side editor,
            // so the editor attribute and its family are determined from the client side editor
            var generator = CreateGenerator(typeof(ClientServiceEditorAttribute));
            generator.AddTSType(new ExternalType
            {
                Name = "ClientServiceEditor",
                Module = "MyModule",
                BaseType = "@serenity-is/corelib:ServiceLookupEditorBase",
                Attributes =
                [
                    new ExternalAttribute
                    {
                        Type = "registerEditor",
                        Arguments = [new ExternalArgument { Value = typeof(ClientServiceEditorAttribute).Namespace + ".ClientServiceEditor" }]
                    }
                ]
            });

            var code = Assert.Single(generator.Run(), x => x.Filename == "EditorDefaults/ClientServiceEditorDefaults.ts").Text;

            Assert.Contains("export const ClientServiceEditorDefaults: ServiceLookupEditorOptions = {", code);
            Assert.Contains("service: \"Test/LookupItems/List\"", code);
        }

        [Theory]
        [InlineData(nameof(EditorDefaultsForm.Templated), typeof(TemplatedEditorAttribute))]
        [InlineData(nameof(EditorDefaultsForm.DefaultAction), typeof(DefaultActionEditorAttribute))]
        [InlineData(nameof(EditorDefaultsForm.ListLookup), typeof(ListLookupEditorAttribute))]
        [InlineData(nameof(EditorDefaultsForm.ExplicitService), typeof(ExplicitServiceEditorAttribute))]
        [InlineData(nameof(EditorDefaultsForm.AttributedItem), typeof(AttributedItemEditorAttribute))]
        [InlineData(nameof(EditorDefaultsForm.ClientLookup), typeof(ClientLookupEditorAttribute))]
        [InlineData(nameof(EditorDefaultsForm.ExplicitLookupType), typeof(ExplicitLookupTypeEditorAttribute))]
        [InlineData(nameof(EditorDefaultsForm.AutoLookupType), typeof(AutoLookupTypeEditorAttribute))]
        public void EditorDefaults_Match_The_Options_Determined_At_Runtime(string propertyName, Type attributeType)
        {
            var identifier = attributeType.Name[..^"Attribute".Length] + "Defaults";
            var generated = ParseEditorDefaults(GenerateEditorDefaults(attributeType, identifier));

            var item = new Serenity.ComponentModel.PropertyItem { Name = propertyName };
            new Serenity.PropertyGrid.BasicPropertyProcessor().Process(new Serenity.PropertyGrid.PropertyInfoSource(
                typeof(EditorDefaultsForm).GetProperty(propertyName)!, null), item);

            Assert.NotEmpty(generated);
            foreach (var (key, literal) in generated)
            {
                Assert.True(item.EditorParams.TryGetValue(key, out var runtimeValue),
                    $"'{key}' is generated but not determined at runtime.");
                Assert.Equal(literal, ToLiteral(runtimeValue));
            }
        }

        private static Dictionary<string, string> ParseEditorDefaults(string code)
        {
            return code.Split('\n')
                .Select(line => System.Text.RegularExpressions.Regex.Match(line, @"^\s+(\w+): (.*?),?\s*$"))
                .Where(match => match.Success)
                .ToDictionary(match => match.Groups[1].Value, match => match.Groups[2].Value);
        }

        private static string ToLiteral(object value) => value switch
        {
            null => "null",
            string s => System.Text.Json.JsonSerializer.Serialize(s),
            bool b => b ? "true" : "false",
            Enum e => Convert.ToInt64(e, System.Globalization.CultureInfo.InvariantCulture).ToString(System.Globalization.CultureInfo.InvariantCulture),
            System.Collections.IEnumerable list => "[" + string.Join(", ", list.Cast<object>().Select(ToLiteral)) + "]",
            IFormattable f => f.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
    }
}

namespace ServerTypingsTest.EditorDefaults
{
    public class LookupItem
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string IconClass { get; set; }
    }

    public class AttributedItem
    {
        public int Id { get; set; }
        [IdProperty]
        public int ItemNo { get; set; }
        [NameProperty]
        public string Caption { get; set; }
        public string Name { get; set; }
    }

    public class LookupItemListRequest : ListRequest
    {
        public int? ParentId { get; set; }
    }

    [Route("Services/Test/LookupItems/[action]")]
    public class LookupItemEndpoint : ServiceEndpoint
    {
        [ListRequestCapabilities(ListRequestCapabilities.SourceAdapter, Exclude = ListRequestCapabilities.DistinctFields)]
        public ListResponse<LookupItem> ListItems(LookupItemListRequest request) => new();

        public ListResponse<LookupItem> List(ListRequest request) => new();
    }

    [Route("Services/Test/LookupWithListLookup/[action]")]
    public class LookupWithListLookupEndpoint : ServiceEndpoint
    {
        public ListResponse<LookupItem> List(ListRequest request) => new();

        [ListRequestCapabilities(ListRequestCapabilities.Baseline)]
        public ListResponse<LookupItem> ListLookup(ListRequest request) => new();
    }

    // like the partial class of an attribute generated for a client side editor
    [ServiceLookupEditor(typeof(LookupItem), EndpointType = typeof(LookupItemEndpoint),
        ActionName = nameof(LookupItemEndpoint.ListItems), TextField = "Key", InplaceAdd = true,
        IncludeColumns = ["Name", "IconClass"])]
    public partial class TemplatedEditorAttribute() : ServiceLookupEditorBaseAttribute("Test.TemplatedEditor")
    {
    }

    [ServiceLookupEditor(typeof(LookupItem), EndpointType = typeof(LookupItemEndpoint))]
    public partial class DefaultActionEditorAttribute() : ServiceLookupEditorBaseAttribute("Test.DefaultActionEditor")
    {
    }

    [ServiceLookupEditor(typeof(LookupItem), EndpointType = typeof(LookupWithListLookupEndpoint))]
    public partial class ListLookupEditorAttribute() : ServiceLookupEditorBaseAttribute("Test.ListLookupEditor")
    {
    }

    [ServiceLookupEditor("Custom/Items/List", "Code", "Title")]
    public partial class ExplicitServiceEditorAttribute() : ServiceLookupEditorBaseAttribute("Test.ExplicitServiceEditor")
    {
    }

    [ServiceLookupEditor(typeof(AttributedItem), EndpointType = typeof(LookupItemEndpoint))]
    public partial class AttributedItemEditorAttribute() : ServiceLookupEditorBaseAttribute("Test.AttributedItemEditor")
    {
    }

    [LookupEditor("Some.Lookup")]
    public partial class LookupTemplateAttribute
    {
    }

    [ServiceLookupEditor(typeof(LookupItem), EndpointType = typeof(LookupItemEndpoint))]
    public class NotAnEditorAttribute : Attribute
    {
    }

    [LookupEditor("Test.Items")]
    public partial class MismatchedFamilyEditorAttribute() : ServiceLookupEditorBaseAttribute("Test.MismatchedFamilyEditor")
    {
    }

    [LookupEditor("Test.Items", InplaceAdd = true, Async = true)]
    public partial class ClientLookupEditorAttribute() : LookupEditorBaseAttribute("Test.ClientLookupEditor")
    {
    }

    [LookupScript("Test.ExplicitLookupSource")]
    public class ExplicitLookupSource
    {
    }

    [LookupScript]
    public class AutoLookupSource
    {
    }

    [LookupEditor(typeof(ExplicitLookupSource))]
    public partial class ExplicitLookupTypeEditorAttribute() : LookupEditorBaseAttribute("Test.ExplicitLookupTypeEditor")
    {
    }

    [LookupEditor(typeof(AutoLookupSource))]
    public partial class AutoLookupTypeEditorAttribute() : LookupEditorBaseAttribute("Test.AutoLookupTypeEditor")
    {
    }

    // the generated part, with the base class, is not visible to the server typings source generator
    [ServiceLookupEditor(typeof(LookupItem), EndpointType = typeof(LookupItemEndpoint))]
    public partial class ClientServiceEditorAttribute
    {
    }
}

namespace ServerTypingsTest.EditorDefaults
{
    public class EditorDefaultsForm
    {
        [TemplatedEditor]
        public string Templated { get; set; }

        [DefaultActionEditor]
        public string DefaultAction { get; set; }

        [ListLookupEditor]
        public string ListLookup { get; set; }

        [ExplicitServiceEditor]
        public string ExplicitService { get; set; }

        [AttributedItemEditor]
        public string AttributedItem { get; set; }

        [ClientLookupEditor]
        public string ClientLookup { get; set; }

        [ExplicitLookupTypeEditor]
        public string ExplicitLookupType { get; set; }

        [AutoLookupTypeEditor]
        public string AutoLookupType { get; set; }
    }
}
