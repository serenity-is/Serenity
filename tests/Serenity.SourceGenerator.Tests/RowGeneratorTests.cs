using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using Serenity.SourceGenerator;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Serenity.Tests.SourceGenerator;

using VerifyCS = CSharpSourceGeneratorVerifier<RowSourceGenerator>;

public class RowGeneratorTests
{
    [Fact]
    public async Task Test1()
    {
        var code =
@"
namespace MyTest
{
    [Serenity.ComponentModel.GeneratedRow]
    public partial class TestRow
    {
        private string testStr;
        private int? testInt;
    }

    public partial class Some {
        private string someStr;
        private int someInt;
    }
}
";

        var expected =
@"using Serenity.Data;

namespace MyTest
{
    partial class TestRow : Row<TestRow.RowFields>
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

        public partial class RowFields : RowFieldsBase
        {
            public readonly StringField TestStr;
            public readonly Int32Field TestInt;

            public RowFields()
            {
                TestStr = new StringField(this, name: ""TestStr"", caption: null, size: 0, flags: FieldFlags.Default,
                    getValue: row => ((TestRow)row).testStr,
                    setValue: (row, value) => ((TestRow)row).testStr = value);

                TestInt = new Int32Field(this, name: ""TestInt"", caption: null, size: 0, flags: FieldFlags.Default,
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
                    (typeof(RowSourceGenerator), "MyTest.TestRow.generated.cs", SourceText.From(expected, Encoding.UTF8)),
                },
                
            },
        }.RunAsync();
    }
}
