using Microsoft.CodeAnalysis.Text;
using Serenity.SourceGenerator;
using System.Threading.Tasks;

namespace Serenity.Tests.SourceGenerator;

using VerifyCS = CSharpSourceGeneratorVerifier<RowFieldsSourceGenerator>;

public class RowGeneratorTests
{
    [Fact]
    public async Task Test1()
    {
        var code =
@"
namespace MyTest
{
    [Serenity.ComponentModel.GenerateRowFields]
    partial class TestRow
    {
        public string TestStr
        {
            get => fields.TestStr[this];
            set => fields.TestStr[this] = value;
        }

        public int? TestInt
        {
            get => fields.TestInt[this];
            set => fields.TestInt[this] = value;
        }
    }
}
";

        var expected =
@"using Serenity.Data;

namespace MyTest
{
    partial class TestRow : Row<TestRow.RowFields>
    {
        private string testStr;
        private int? testInt;

        public partial class RowFields : RowFieldsBase
        {
            public StringField TestStr;
            public Int32Field TestInt;

            protected override void CreateGeneratedFields()
            {
                TestStr = new StringField(this, ""TestStr"", null, 0, FieldFlags.Default,
                    getValue: row => ((TestRow)row).testStr,
                    setValue: (row, value) => ((TestRow)row).testStr = value);

                TestInt = new Int32Field(this, ""TestInt"", null, 0, FieldFlags.Default,
                    getValue: row => ((TestRow)row).testInt,
                    setValue: (row, value) => ((TestRow)row).testInt = value);
            }
        }
    }
}";

        await new VerifyCS.Test
        {
            TestState =
            {
                Sources = { code },
                GeneratedSources =
                {
                    (typeof(RowFieldsSourceGenerator), "MyTest.TestRow.generated.cs", SourceText.From(expected, Encoding.UTF8)),
                },
                
            },
        }.RunAsync();
    }
}
