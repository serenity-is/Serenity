using Serenity.CodeGeneration;
using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Web;
using Xunit;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
               [Fact]
        public void Determines_LookupKey_On_Row_If_Set_Explicitly()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithExplicitLookupKeyRow.ts", result.Keys);
            var code = result["SomeModule.RowWithExplicitLookupKeyRow.ts"];
            Assert.Contains("lookupKey = 'Some.ExplicitKey'", code);
        }

        [Fact]
        public void Determines_Auto_LookupKey_On_Row()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithNoLookupKeyRow.ts", result.Keys);
            var code = result["SomeModule.RowWithNoLookupKeyRow.ts"];
            Assert.Contains("lookupKey = 'SomeModule.RowWithNoLookupKey'", code);
        }

        [Fact]
        public void Determines_Auto_LookupKey_On_Row_With_No_Row_Suffix()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithNoLookupKeyNoRowSuffix.ts", result.Keys);
            var code = result["SomeModule.RowWithNoLookupKeyNoRowSuffix.ts"];
            Assert.Contains("lookupKey = 'SomeModule.RowWithNoLookupKeyNoRowSuffix'", code);
        }

        [Fact]
        public void Determines_Auto_LookupKey_On_Row_With_External_Lookup_With_Explicit_Key()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithExternalLookupWithExplicitKeyRow.ts", result.Keys);
            var code = result["SomeModule.RowWithExternalLookupWithExplicitKeyRow.ts"];
            Assert.Contains("lookupKey = 'AModule.ExternalLookupKey'", code);
        }

        [Fact]
        public void Determines_Auto_LookupKey_On_Row_With_External_Lookup_With_Auto_Key()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithExternalLookupWithAutoKeyRow.ts", result.Keys);
            var code = result["SomeModule.RowWithExternalLookupWithAutoKeyRow.ts"];
            Assert.Contains("lookupKey = 'SomeModule.ExternalWithAutoKey'", code);
        }

        [Fact]
        public void Uses_Module_Attribute()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowWithModuleRow.ts", result.Keys);
            var code = result["SomeModule.RowWithModuleRow.ts"];
            Assert.Contains("lookupKey = 'AnotherModule.RowWithModule'", code);
        }
    }
}

namespace ServerTypingsTest.SomeModule.Entities
{
    [LookupScript("Some.ExplicitKey")]
    public class RowWithExplicitLookupKeyRow : Row<RowWithExplicitLookupKeyRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }

    [LookupScript]
    public class RowWithNoLookupKeyRow : Row<RowWithNoLookupKeyRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }

    [LookupScript]
    public class RowWithNoLookupKeyNoRowSuffix : Row<RowWithNoLookupKeyNoRowSuffix.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }

    [LookupScript(typeof(ExternalLookupWithExplicitKey))]
    public class RowWithExternalLookupWithExplicitKeyRow : Row<RowWithExternalLookupWithExplicitKeyRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }

    [LookupScript("AModule.ExternalLookupKey")]
    public class ExternalLookupWithExplicitKey : RowLookupScript<RowWithExternalLookupWithExplicitKeyRow>
    {
        public ExternalLookupWithExplicitKey(ISqlConnections connections) : base(connections)
        {
        }
    }

    [LookupScript(typeof(ExternalWithAutoKeyLookup))]
    public class RowWithExternalLookupWithAutoKeyRow : Row<RowWithExternalLookupWithAutoKeyRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }

    [LookupScript]
    public class ExternalWithAutoKeyLookup : RowLookupScript<RowWithExternalLookupWithExplicitKeyRow>
    {
        public ExternalWithAutoKeyLookup(ISqlConnections connections) : base(connections)
        {
        }
    }

    [LookupScript, Module("AnotherModule")]
    public class RowWithModuleRow : Row<RowWithModuleRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }

}