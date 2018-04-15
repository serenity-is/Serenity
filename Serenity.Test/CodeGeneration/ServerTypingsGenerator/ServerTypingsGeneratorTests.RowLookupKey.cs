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

        [Fact]
        public void GenerateCustomLookupScripts()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowForCustomLookupRow.ts", result.Keys);
            var code = result["SomeModule.RowForCustomLookupRow.ts"];
            Assert.Contains("export declare function getLookup_CustomWithAutoKeyLookup(): Q.Lookup<{}>;", code);
            Assert.Contains("export declare function getLookup_CustomWithExplicitKeyLookup(): Q.Lookup<{}>;", code);
            Assert.Contains("export declare function getLookup_CustomWithRowLookupScriptLookup(): Q.Lookup<{}>;", code);

            Assert.Contains("export enum CustomLookups {", code);
            Assert.Contains("getLookup_CustomWithAutoKeyLookup = \"SomeModule.CustomWithAutoKey\"", code);
            Assert.Contains("getLookup_CustomWithExplicitKeyLookup = \"This is custom lookup key\"", code);
            Assert.Contains("getLookup_CustomWithRowLookupScriptLookup = \"SomeModule.CustomWithRowLookupScript\"", code);

            Assert.Contains("Object.keys(CustomLookups).forEach(x => {", code);

            Assert.Contains("(<any>RowForCustomLookupRow)[x] = function () {", code);
            Assert.Contains("return Q.getLookup(CustomLookups[x]);", code);

            Assert.Contains("(<any>RowForCustomLookupRow)[x + \"Async\"] = function () {", code);
            Assert.Contains("return Q.getLookupAsync(CustomLookups[x]);", code);
        }

        [Fact]
        public void GenerateCustomLookupScriptsWithoutFlagAttributeRow()
        {
            var generator = CreateGenerator();
            var result = generator.Run();
            Assert.Contains("SomeModule.RowForCustomLookupWithoutFlagAttributeRow.ts", result.Keys);
            var code = result["SomeModule.RowForCustomLookupWithoutFlagAttributeRow.ts"];

            Assert.DoesNotContain("export enum CustomLookups {", code);
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

    [HasCustomLookup(typeof(CustomWithAutoKeyLookup))]
    [HasCustomLookup(typeof(CustomWithExplicitKeyLookup))]
    [HasCustomLookup(typeof(CustomWithExplicitKeyFromTypeHasNoLookupScriptLookup))]
    [HasCustomLookup(typeof(CustomWithExplicitKeyFromTypeHasLookupScriptLookup))]
    [HasCustomLookup(typeof(CustomWithExplicitKeyAndMultiParamsLookup))]
    [HasCustomLookup(typeof(CustomWithRowLookupScriptLookup))]
    [HasCustomLookup(typeof(CustomWithoutLookupScriptLookup))]
    public class RowForCustomLookupRow : Row
    {
        public class RowFields : RowFieldsBase
        {
        }

        public readonly static RowFields Fields = new RowFields().Init();

        public RowForCustomLookupRow()
            : base(Fields)
        {
        }
    }

    public class RowForCustomLookupWithoutFlagAttributeRow : Row
    {
        public class RowFields : RowFieldsBase
        {
        }

        public readonly static RowFields Fields = new RowFields().Init();

        public RowForCustomLookupWithoutFlagAttributeRow()
            : base(Fields)
        {
        }
    }


    [LookupScript]
    public class CustomWithAutoKeyLookup : LookupScript
    {
    }

    [LookupScript("This is custom lookup key")]
    public class CustomWithExplicitKeyLookup : LookupScript
    {
    }

    [LookupScript(typeof(CustomWithoutLookupScriptLookup))]
    public class CustomWithExplicitKeyFromTypeHasNoLookupScriptLookup : LookupScript
    {
    }

    [LookupScript(typeof(CustomWithRowLookupScriptLookup))]
    public class CustomWithExplicitKeyFromTypeHasLookupScriptLookup : LookupScript
    {
    }

    [LookupScript("This is custom lookup key", Permission = "*")]
    public class CustomWithExplicitKeyAndMultiParamsLookup : LookupScript
    {
    }

    [LookupScript]
    public class CustomWithRowLookupScriptLookup : RowLookupScript<RowForCustomLookupRow>
    {
    }

    public class CustomWithoutLookupScriptLookup : LookupScript
    {
    }
}