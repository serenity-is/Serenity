using MsieJavaScriptEngine;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Serenity.CodeGeneration.Test
{
    public partial class TypeScriptParserTests
    {
        [Fact]
        public void CanParseInterfaceDialogFormatter()
        {
            var parser = new TypeScriptParser();
            var types = new List<TypeScriptParser.TypeInfo>();

            parser.ReportType += (type) =>
            {
                types.Add(type);
            };

            parser.Parse(Input_InterfaceDialogFormatter);

            Assert.Equal(3, types.Count);

            var t0 = types[0];
            Assert.True(t0.IsInterface);
            Assert.Equal("CustomerRow", t0.Name);
            Assert.Equal("", t0.Namespace.Name);
            Assert.Equal("", t0.Extends);
            Assert.Equal("", t0.Implements);
        }

        private MsieJsEngine SetupJsEngine()
        {
            var jsEngine = new MsieJsEngine();
            try
            {
                using (var sr = new StreamReader(
                    typeof(TypeScriptParserTests).Assembly.GetManifestResourceStream(
                        "Serenity.Test.CodeGeneration.TypeScriptParserTests.typescriptServices.js")))
                {
                    jsEngine.Evaluate(sr.ReadToEnd());
                }

                using (var sr = new StreamReader(
                    typeof(DtoGenerator).Assembly.GetManifestResourceStream(
                        "Serenity.Web.Scripts.tsservices.Serenity.CodeGeneration.js")))
                {
                    jsEngine.Evaluate(sr.ReadToEnd());
                }

                return jsEngine;
            }
            catch
            {
                jsEngine.Dispose();
                throw;
            }
        }


        public const string Input_FormatterOnly = @"
namespace Serene.Northwind {
    export class MyBoldFormatter implements Slick.Formatter
    {
        format(ctx: Slick.FormatterContext): string {
            return ""<b>"" + Q.htmlEncode(ctx.value) + ""</b>"";
        }
    }
}";

        [Fact]
        public void CanParseFormatterType()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", Input_FormatterOnly);
                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseFormatterTypes(sourceText))");
                var formatterTypes = JSON.Parse<Dictionary<string, FormatterTypeInfo>>(json);
                Assert.NotNull(formatterTypes);
                Assert.Equal(1, formatterTypes.Count);
                var k = "Serene.Northwind.MyBoldFormatter";
                Assert.True(formatterTypes.ContainsKey(k));
                Assert.NotNull(formatterTypes[k].Options);
                Assert.Equal(0, formatterTypes[k].Options.Count);
            }
        }

        public const string Input_FormatterOnlyWithImport = @"
namespace Serene.Northwind {
    import S = Slick;

    export class MyBoldFormatter implements S.Formatter
    {
        format(ctx: Slick.FormatterContext): string {
            return ""<b>"" + Q.htmlEncode(ctx.value) + ""</b>"";
        }
    }
}";

        [Fact]
        public void CanParseFormatterTypeWithImport()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", Input_FormatterOnly);
                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseFormatterTypes(sourceText))");
                var formatterTypes = JSON.Parse<Dictionary<string, FormatterTypeInfo>>(json);
                Assert.NotNull(formatterTypes);
                Assert.Equal(1, formatterTypes.Count);
                var k = "Serene.Northwind.MyBoldFormatter";
                Assert.True(formatterTypes.ContainsKey(k));
                Assert.NotNull(formatterTypes[k].Options);
                Assert.Equal(0, formatterTypes[k].Options.Count);
            }
        }

        public const string Input_InterfaceDialogFormatter = @"
interface CustomerRow {
    ID: number;
    CustomerID: string;
}

namespace Serene.Northwind {
    import D = Serenity.Decorators;
    import S = Slick;

    @D.formKey(""Northwind.Customer"") @D.idProperty(""ID"") @D.nameProperty(""CustomerID"") 
    @D.service(""Northwind/Customer"") @D.flexify() @D.maximizable()
    export class MyCustomerDialog extends Serenity.EntityDialog<CustomerRow> {
    }

    export class MyBoldFormatter implements S.Formatter
    {
        format(ctx: Slick.FormatterContext): string {
            return ""<b>"" + Q.htmlEncode(ctx.value) + ""</b>"";
        }
    }
}

namespace XYZ
{
}";

        [Fact]
        public void CanParseFormatterAmongOtherClasses()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", Input_InterfaceDialogFormatter);
                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseFormatterTypes(sourceText))");
                var formatterTypes = JSON.Parse<Dictionary<string, FormatterTypeInfo>>(json);
                Assert.NotNull(formatterTypes);
                Assert.Equal(1, formatterTypes.Count);
                var k = "Serene.Northwind.MyBoldFormatter";
                Assert.True(formatterTypes.ContainsKey(k));
                Assert.NotNull(formatterTypes[k].Options);
                Assert.Equal(0, formatterTypes[k].Options.Count);
            }
        }
    }
}