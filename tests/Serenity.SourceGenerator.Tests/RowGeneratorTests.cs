using Microsoft.CodeAnalysis.Text;
using Serenity.SourceGenerator;
using System.Threading.Tasks;

namespace Serenity.Tests.SourceGenerator;

using VerifyCS = CSharpSourceGeneratorVerifier<RowGenerator>;

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
        private int testInt;
    }

    public partial class Some {
        private string someStr;
        private int someInt;
    }
}
";

        var expected =
@"using Serenity;
using Serenity.Data;
using System;

namespace MyTest
{
    partial class TestRow
    {
        public partial class RowFields : RowFieldsBase
        {
            public readonly StringField SomeStr;
            public readonly StringField SomeInt;
        
            public RowFields()
            {
                SomeStr = new Int32Field();
                SomeInt = new StringField();
            }
        }

        public TestRow()
            : base()
        {
        }

        public TestRow(RowFields fields)
            : base(fields)
        {
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
                    (typeof(RowGenerator), "GeneratedFileName", SourceText.From(expected, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                }
            },
        }.RunAsync();
    }
}
