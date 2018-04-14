using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Web;
using Xunit;

namespace Serenity.CodeGeneration.Test
{
    public partial class ServerTypingsGeneratorTests
    {
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

        public static RowFields Fields = new RowFields().Init();

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

        public static RowFields Fields = new RowFields().Init();

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