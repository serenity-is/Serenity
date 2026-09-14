using ServerTypingsTest.Columns;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Columns_Without_BasedOnRow()
        {
            var generator = CreateGenerator(typeof(ColumnsWithoutRow));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "ColumnsWithoutRow.ts").Text;

            Assert.Contains("import { Column } from \"@serenity-is/sleekgrid\";", code);
            Assert.Contains("Name: Column;", code);
            Assert.Contains("Description: Column;", code);
            Assert.Contains("export class ColumnsWithoutRow extends ColumnsBase", code);
            Assert.Contains("static readonly columnsKey = 'ServerTypingsTest.Columns.ColumnsWithoutRow';", code);
            Assert.Contains("static readonly Fields = fieldsProxy<ColumnsWithoutRow>();", code);
        }

        [Fact]
        public void Columns_With_BasedOnRow()
        {
            var generator = CreateGenerator(typeof(ColumnsWithRow));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "ColumnsWithRow.ts").Text;

            Assert.Contains("Name: Column<ColumnsRow>;", code);
            Assert.Contains("extends ColumnsBase<ColumnsRow>", code);
        }

        [Fact]
        public void Columns_With_FormatterType_References_Formatter()
        {
            var generator = CreateGenerator(typeof(ColumnsWithFormatter));
            generator.AddTSType(FormatterTypeFor("CustomFormatter", "Custom.Formatter"));

            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "ColumnsWithFormatter.ts").Text;

            Assert.Contains("import { CustomFormatter } from \"MyModule\";", code);
            Assert.Contains("[CustomFormatter]; // referenced types", code);
        }

        [Fact]
        public void Columns_Uses_Formatter_Attributes_And_Enum_References()
        {
            var generator = CreateGenerator(typeof(ColumnsWithFormatters));
            generator.AddTSType(FormatterTypeFor("KeyFormatter", "Forms.KeyFormatter"));
            generator.AddTSType(FormatterTypeFor("LiteralFormatter", "Forms.LiteralFormatter"));

            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "ColumnsWithFormatters.ts").Text;

            Assert.Contains("import { KeyFormatter, LiteralFormatter } from \"MyModule\";", code);
            Assert.Contains("[KeyFormatter, LiteralFormatter, ColumnStatus]; // referenced types", code);
            Assert.Contains(result, x => x.Filename == "ColumnStatus.ts");
        }

        private static ExternalType FormatterTypeFor(string name, string key)
        {
            return new ExternalType
            {
                Name = name,
                Module = "MyModule",
                Attributes =
                [
                    new ExternalAttribute
                    {
                        Type = "registerFormatter",
                        Arguments = [new ExternalArgument { Value = key }]
                    }
                ]
            };
        }
    }
}

namespace ServerTypingsTest.Columns
{
    [ColumnsScript]
    public class ColumnsWithoutRow
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [ColumnsScript, BasedOnRow(typeof(ColumnsRow))]
    public class ColumnsWithRow
    {
        public string Name { get; set; }
    }

    [ColumnsScript]
    public class ColumnsWithFormatter
    {
        public string Plain { get; set; }

        [FormatterType("Custom.Formatter")]
        public string Custom { get; set; }
    }

    [ColumnsScript]
    public class ColumnsWithFormatters
    {
        [MyFormatter]
        public string KeyProp { get; set; }

        [MyLiteralFormatter]
        public string LiteralProp { get; set; }

        public ColumnStatus Status { get; set; }

        [IgnoreUIField]
        public string Ignored { get; set; }
    }

    public enum ColumnStatus
    {
        Passive = 0,
        Active = 1
    }

    public class MyFormatterAttribute : CustomFormatterAttribute
    {
        public const string Key = "Forms.KeyFormatter";

        public MyFormatterAttribute()
            : base(Key)
        {
        }
    }

    public class MyLiteralFormatterAttribute : CustomFormatterAttribute
    {
        public MyLiteralFormatterAttribute()
            : base("Forms.LiteralFormatter")
        {
        }
    }

    public class ColumnsRow
    {
        public string Name { get; set; }
    }
}
