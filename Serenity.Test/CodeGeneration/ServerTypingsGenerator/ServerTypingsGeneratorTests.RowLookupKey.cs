using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Web;
using Xunit;

namespace Serenity.CodeGeneration.Test
{
    public partial class ServerTypingsGeneratorTests
    {
        private ServerTypingsGenerator CreateGenerator()
        {
            var generator = new ServerTypingsGenerator(
                typeof(ServerTypingsGeneratorTests).Assembly.Location);
            generator.RootNamespaces.Add("ServerTypingsTest");
            return generator;
        }

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
    public class RowWithExplicitLookupKeyRow : Row
    {
        public class RowFields : RowFieldsBase
        {
        }

        public static RowFields Fields = new RowFields().Init();

        public RowWithExplicitLookupKeyRow()
            : base(Fields)
        {
        }
    }

    [LookupScript]
    public class RowWithNoLookupKeyRow : Row
    {
        public class RowFields : RowFieldsBase
        {
        }

        public static RowFields Fields = new RowFields().Init();

        public RowWithNoLookupKeyRow()
            : base(Fields)
        {
        }
    }

    [LookupScript]
    public class RowWithNoLookupKeyNoRowSuffix : Row
    {
        public class RowFields : RowFieldsBase
        {
        }

        public static RowFields Fields = new RowFields().Init();

        public RowWithNoLookupKeyNoRowSuffix()
            : base(Fields)
        {
        }
    }

    [LookupScript(typeof(ExternalLookupWithExplicitKey))]
    public class RowWithExternalLookupWithExplicitKeyRow : Row
    {
        public class RowFields : RowFieldsBase
        {
        }

        public static RowFields Fields = new RowFields().Init();

        public RowWithExternalLookupWithExplicitKeyRow()
            : base(Fields)
        {
        }
    }

    [LookupScript("AModule.ExternalLookupKey")]
    public class ExternalLookupWithExplicitKey : RowLookupScript<RowWithExternalLookupWithExplicitKeyRow>
    {
    }

    [LookupScript(typeof(ExternalWithAutoKeyLookup))]
    public class RowWithExternalLookupWithAutoKeyRow : Row
    {
        public class RowFields : RowFieldsBase
        {
        }

        public static RowFields Fields = new RowFields().Init();

        public RowWithExternalLookupWithAutoKeyRow()
            : base(Fields)
        {
        }
    }

    [LookupScript]
    public class ExternalWithAutoKeyLookup : RowLookupScript<RowWithExternalLookupWithExplicitKeyRow>
    {
    }

    [LookupScript, Module("AnotherModule")]
    public class RowWithModuleRow : Row
    {
        public class RowFields : RowFieldsBase
        {
        }

        public static RowFields Fields = new RowFields().Init();

        public RowWithModuleRow()
            : base(Fields)
        {
        }
    }

}