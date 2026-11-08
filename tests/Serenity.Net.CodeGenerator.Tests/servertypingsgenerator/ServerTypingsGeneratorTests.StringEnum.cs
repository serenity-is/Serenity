using ServerTypingsTest.StringEnums;
using ServerTypingsTest.StringEnums.Entities;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void StringEnum_IsGeneratedAsConstObjectWithUnionAndRegisterEnum()
        {
            var generator = CreateGenerator(typeof(StringEnumContainer));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "StringEnums/WidgetCategory.ts").Text;

            Assert.Contains("export const WidgetCategory = {", code);
            Assert.Contains("    Charts: 'charts',", code);
            Assert.Contains("    Tables: 'tables'", code);
            Assert.Contains("} as const;", code);
            Assert.Contains("export type WidgetCategory = typeof WidgetCategory[keyof typeof WidgetCategory];", code);

            // an alias keeps its own name, like the members of an enum with the same value
            Assert.Contains("    ChartsAlias: 'charts'", code);

            // the members hidden from the client are neither in the object nor in its icons
            Assert.DoesNotContain("Hidden", code);
            Assert.DoesNotContain("sv-hidden", code);
            Assert.DoesNotContain("Internal", code);
            Assert.DoesNotContain("NotAString", code);
        }

        [Fact]
        public void StringEnum_IncludesIconsKeyedByTheirValues()
        {
            var generator = CreateGenerator(typeof(StringEnumContainer));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "StringEnums/WidgetCategory.ts").Text;

            Assert.Contains("registerEnum(WidgetCategory, 'ServerTypingsTest.StringEnums.WidgetCategory', {", code);
            Assert.Contains("    enumKey: 'Test.WidgetCategory',", code);
            Assert.Contains("    enumIcons: {", code);
            Assert.Contains("        'charts': 'sv-ph-chart-bar-duotone'", code);
        }

        [Fact]
        public void StringEnum_EscapesValuesAndIcons()
        {
            var generator = CreateGenerator(typeof(StringEnumContainer));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "StringEnums/WidgetCategory.ts").Text;

            Assert.Contains("""    Quotes: 'don\'t'""", code);
            Assert.Contains("""        'don\'t': 'sv-icon\'s'""", code);
        }

        [Fact]
        public void StringEnum_WithoutIcons_UsesEnumKeyArgument()
        {
            var generator = CreateGenerator(typeof(StringEnumContainer));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "StringEnums/CategoryFromAttribute.ts").Text;

            Assert.Contains("export const CategoryFromAttribute = {", code);
            Assert.Contains("export type CategoryFromAttribute",
                code);
            Assert.Contains("registerEnum(CategoryFromAttribute, 'ServerTypingsTest.StringEnums.CategoryFromAttribute');", code);
            Assert.DoesNotContain("enumIcons", code);
        }

        [Fact]
        public void StringEnum_ReferencedByAttributeArgument_IsGenerated()
        {
            // [EnumEditor(typeof(CategoryFromAttribute))] on a string property is what makes it a dependency
            var generator = CreateGenerator(typeof(StringEnumContainer));
            var result = generator.Run();

            Assert.Contains(result, x => x.Filename == "StringEnums/CategoryFromAttribute.ts");
        }

        [Fact]
        public void StringEnum_ReferencedByFieldGenericArgument_IsGenerated()
        {
            // StringEnumField<CategoryFromField> is what makes it a dependency
            var generator = CreateGenerator(typeof(StringEnumRow));
            var result = generator.Run();

            Assert.Contains(result, x => x.Filename == "StringEnums/CategoryFromField.ts");
        }

        [Fact]
        public void StringEnum_FieldProperty_IsTypedWithTheUnion()
        {
            var generator = CreateGenerator(typeof(StringEnumRow));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "StringEnums/StringEnumRow.ts").Text;

            Assert.Contains("Category?: CategoryFromField;", code);
            // the plain string field of the same row keeps its type
            Assert.Contains("Name?: string;", code);
            Assert.Contains("import { CategoryFromField } from \"./CategoryFromField\";", code);
        }

        [Fact]
        public void StringEnum_PropertyOfAContainer_IsTypedWithTheUnion()
        {
            var generator = CreateGenerator(typeof(StringEnumContainer));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "StringEnums/StringEnumContainer.ts").Text;

            Assert.Contains("Category?: WidgetCategory;", code);
            Assert.Contains("AttrCategory?: string;", code);
        }
    }
}

namespace ServerTypingsTest.StringEnums
{
    [ScriptInclude]
    public class StringEnumContainer
    {
        public WidgetCategory? Category { get; set; }

        [EnumEditor(typeof(CategoryFromAttribute))]
        public string? AttrCategory { get; set; }
    }

    [EnumKey("Test.WidgetCategory")]
    public abstract class WidgetCategory : StringEnum
    {
        [Description("Charts")]
        [IconClass("sv-ph-chart-bar-duotone")]
        public const string Charts = "charts";

        public const string Tables = "tables";

        public const string ChartsAlias = "charts";

        [IconClass("sv-icon's")]
        public const string Quotes = "don't";

        [TransformIgnore, IconClass("sv-hidden")]
        public const string Hidden = "hidden";

        [IgnoreUIField]
        public const string Internal = "internal";

        public const int NotAString = 1;
    }

    public abstract class CategoryFromAttribute : StringEnum
    {
        public const string First = "first";
    }

    public abstract class CategoryFromField : StringEnum
    {
        [IconClass("sv-lucide-map")]
        public const string Maps = "maps";
        public const string Charts = "charts";
    }
}

namespace ServerTypingsTest.StringEnums.Entities
{
    public class StringEnumRow : Row<StringEnumRow.RowFields>
    {
        public string? Name
        {
            get => fields.Name[this];
            set => fields.Name[this] = value;
        }

        public string? Category
        {
            get => fields.Category[this];
            set => fields.Category[this] = value;
        }

        public class RowFields : RowFieldsBase
        {
            public StringField Name;
            public StringEnumField<CategoryFromField> Category;
        }
    }
}
