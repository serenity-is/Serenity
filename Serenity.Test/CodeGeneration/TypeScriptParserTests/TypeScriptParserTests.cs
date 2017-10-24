using MsieJavaScriptEngine;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Serenity.CodeGeneration.Test
{
    /// <summary>
    /// These tests should be rewritten
    /// </summary>
    public partial class TypeScriptParserTests
    {
        private MsieJsEngine SetupJsEngine()
        {
            var jsEngine = new MsieJsEngine();
            try
            {
                using (var sr = new StreamReader(
                    typeof(TypeScriptParserTests).Assembly.GetManifestResourceStream(
                        "Serenity.Test.CodeGeneration.TypeScriptParserTests.json2.min.js")))
                {
                    jsEngine.Evaluate(sr.ReadToEnd());
                }

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

        public void CanParseFormatterType()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", @"
                    namespace Serene.Northwind {
                        export class MyBoldFormatter implements Slick.Formatter {
                            format(ctx: Slick.FormatterContext): string {
                                return "" < b > "" + Q.htmlEncode(ctx.value) + "" </ b > "";
                            }
                        }
                    }");

                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseFormatterTypes(sourceText))");
                var formatterTypes = JSON.Parse<Dictionary<string, FormatterTypeInfo>>(json);
                Assert.NotNull(formatterTypes);
                Assert.StrictEqual(1, formatterTypes.Count);
                var k = "Serene.Northwind.MyBoldFormatter";
                Assert.True(formatterTypes.ContainsKey(k));
                Assert.NotNull(formatterTypes[k].Options);
                Assert.StrictEqual(0, formatterTypes[k].Options.Count);
            }
        }

        public void CanParseFormatterOptionFieldWithCallSyntax()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", @"
                    namespace Serene.Northwind {
                        export class MyBoldFormatter implements Slick.Formatter {
                            format(ctx: Slick.FormatterContext): string {
                                return "" < b > "" + Q.htmlEncode(ctx.value) + "" </ b > "";
                            }
                           
                            @Serenity.Decorators.option()
                            someOption: string;
                        }
                    }");

                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseFormatterTypes(sourceText))");
                var formatterTypes = JSON.Parse<Dictionary<string, FormatterTypeInfo>>(json);
                Assert.NotNull(formatterTypes);
                Assert.StrictEqual(1, formatterTypes.Count);
                var k = "Serene.Northwind.MyBoldFormatter";
                Assert.True(formatterTypes.ContainsKey(k));
                var ft = formatterTypes[k];
                Assert.NotNull(ft.Options);
                Assert.StrictEqual(1, ft.Options.Count);
                Assert.True(ft.Options.ContainsKey("someOption"));
                var o1 = ft.Options["someOption"];
                Assert.Equal("someOption", o1.Name);
                Assert.Equal("string", o1.Type);
            }
        }

        public void CanParseFormatterOptionFieldWithImport()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", @"
                    namespace Serene.Northwind {
                        import D = Serenity.Decorators;
                        export class MyBoldFormatter implements Slick.Formatter {
                            format(ctx: Slick.FormatterContext): string {
                                return "" < b > "" + Q.htmlEncode(ctx.value) + "" </ b > "";
                            }
                           
                            @D.option()
                            someOption: string;
                        }
                    }");

                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseFormatterTypes(sourceText))");
                var formatterTypes = JSON.Parse<Dictionary<string, FormatterTypeInfo>>(json);
                Assert.NotNull(formatterTypes);
                Assert.StrictEqual(1, formatterTypes.Count);
                var k = "Serene.Northwind.MyBoldFormatter";
                Assert.True(formatterTypes.ContainsKey(k));
                var ft = formatterTypes[k];
                Assert.NotNull(ft.Options);
                Assert.StrictEqual(1, ft.Options.Count);
                Assert.True(ft.Options.ContainsKey("someOption"));
                var o1 = ft.Options["someOption"];
                Assert.Equal("someOption", o1.Name);
                Assert.Equal("string", o1.Type);
            }
        }

        public void CanParseFormatterOptionFieldWithNoCallSyntax()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", @"
                    namespace Serene.Northwind {
                        export class MyBoldFormatter implements Slick.Formatter {
                            format(ctx: Slick.FormatterContext): string {
                                return "" < b > "" + Q.htmlEncode(ctx.value) + "" </ b > "";
                            }
                           
                            @Serenity.Decorators.option
                            someOption: string;
                        }
                    }");

                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseFormatterTypes(sourceText))");
                var formatterTypes = JSON.Parse<Dictionary<string, FormatterTypeInfo>>(json);
                Assert.NotNull(formatterTypes);
                Assert.StrictEqual(1, formatterTypes.Count);
                var k = "Serene.Northwind.MyBoldFormatter";
                Assert.True(formatterTypes.ContainsKey(k));
                var ft = formatterTypes[k];
                Assert.NotNull(ft.Options);
                Assert.StrictEqual(1, ft.Options.Count);
                Assert.True(ft.Options.ContainsKey("someOption"));
                var o1 = ft.Options["someOption"];
                Assert.Equal("someOption", o1.Name);
                Assert.Equal("string", o1.Type);
            }
        }

        public void CanParseFormatterOptionFieldWithNoTypeSpecified()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", @"
                    namespace Serene.Northwind {
                        export class MyBoldFormatter implements Slick.Formatter {
                            format(ctx: Slick.FormatterContext): string {
                                return "" < b > "" + Q.htmlEncode(ctx.value) + "" </ b > "";
                            }
                           
                            @Serenity.Decorators.option()
                            someOption;
                        }
                    }");

                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseFormatterTypes(sourceText))");
                var formatterTypes = JSON.Parse<Dictionary<string, FormatterTypeInfo>>(json);
                Assert.NotNull(formatterTypes);
                Assert.StrictEqual(1, formatterTypes.Count);
                var k = "Serene.Northwind.MyBoldFormatter";
                Assert.True(formatterTypes.ContainsKey(k));
                var ft = formatterTypes[k];
                Assert.NotNull(ft.Options);
                Assert.StrictEqual(1, ft.Options.Count);
                Assert.True(ft.Options.ContainsKey("someOption"));
                var o1 = ft.Options["someOption"];
                Assert.Equal("someOption", o1.Name);
                Assert.Equal("", o1.Type);
            }
        }

        public void ShouldntReturnFormatterInAmbientNamespace()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", @"
                    declare namespace Serene.Northwind {
                        class MyBoldFormatter implements Slick.Formatter
                            {
                            }
                    }");

                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseFormatterTypes(sourceText))");
                var formatterTypes = JSON.Parse<Dictionary<string, FormatterTypeInfo>>(json);
                Assert.NotNull(formatterTypes);
                Assert.StrictEqual(0, formatterTypes.Count);
            }
        }

        public void ShouldOnlyReturnFormatterInNonAmbientNamespace()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", @"
                    declare namespace Serene.Northwind {
                        class MyBoldFormatter implements Slick.Formatter {
                        }
                    }

                    namespace Serene.Northwind {
                        export class MyItalicFormatter implements Slick.Formatter {
                            format(ctx: Slick.FormatterContext): string {
                                return ""<i>"" + Q.htmlEncode(ctx.value) + ""</i>"";
                            }
                        }
                    }");

                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseFormatterTypes(sourceText))");
                var formatterTypes = JSON.Parse<Dictionary<string, FormatterTypeInfo>>(json);

                Assert.NotNull(formatterTypes);
                Assert.StrictEqual(1, formatterTypes.Count);
                var k = "Serene.Northwind.MyItalicFormatter";
                Assert.True(formatterTypes.ContainsKey(k));
                Assert.NotNull(formatterTypes[k].Options);
                Assert.StrictEqual(0, formatterTypes[k].Options.Count);
            }
        }


        public void ShouldntReturnNotExportedFormatters()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", @"
                    namespace Serene.Northwind {
                        class MyBoldFormatter implements Slick.Formatter {
                        }
                    }");

                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseFormatterTypes(sourceText))");
                var formatterTypes = JSON.Parse<Dictionary<string, FormatterTypeInfo>>(json);
                Assert.NotNull(formatterTypes);
                Assert.StrictEqual(0, formatterTypes.Count);
            }
        }

        public void ShouldOnlyReturnExportedFormatters()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", @"
                    namespace Serene.Northwind {
                        class MyBoldFormatter implements Slick.Formatter {
                        }

                        export class MyItalicFormatter implements Slick.Formatter {
                            format(ctx: Slick.FormatterContext): string {
                                return ""<i>"" + Q.htmlEncode(ctx.value) + ""</i>"";
                            }
                        }
                    }");

                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseFormatterTypes(sourceText))");
                var formatterTypes = JSON.Parse<Dictionary<string, FormatterTypeInfo>>(json);

                Assert.NotNull(formatterTypes);
                Assert.StrictEqual(1, formatterTypes.Count);
                var k = "Serene.Northwind.MyItalicFormatter";
                Assert.True(formatterTypes.ContainsKey(k));
                Assert.NotNull(formatterTypes[k].Options);
                Assert.StrictEqual(0, formatterTypes[k].Options.Count);
            }
        }


        public void CanParseFormatterTypeWithImport()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", @"
                    namespace Serene.Northwind {
                        import S = Slick;

                        export class MyBoldFormatter implements S.Formatter
                        {
                            format(ctx: Slick.FormatterContext): string {
                                return ""<b>"" + Q.htmlEncode(ctx.value) + ""</b>"";
                            }
                        }
                    }");
                
            }
        }

        public void CanParseFormatterAmongOtherClasses()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", @"
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
                    }");

                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseFormatterTypes(sourceText))");
                var formatterTypes = JSON.Parse<Dictionary<string, FormatterTypeInfo>>(json);
                Assert.NotNull(formatterTypes);
                Assert.StrictEqual(1, formatterTypes.Count);
                var k = "Serene.Northwind.MyBoldFormatter";
                Assert.True(formatterTypes.ContainsKey(k));
                Assert.NotNull(formatterTypes[k].Options);
                Assert.StrictEqual(0, formatterTypes[k].Options.Count);
            }
        }

        public void ParseSourceToJson()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", @"
                    import S = Slick;
                    namespace Z {
                        import D = Serenity.Decorators;
                        export class MyBoldFormatter implements S.Formatter
                        {
                            format(ctx: Slick.FormatterContext): string {
                                return ""<b>"" + Q.htmlEncode(ctx.value) + ""</b>"";
                            }

                            @D.option()
                            test: int;

                            @D.zzz
                            abc: int;

                            @D.option()
                            set_test(value: int) {
                            }
                        }
                    }");

                var json = jsEngine.Evaluate<string>(
                    "Serenity.CodeGeneration.parseSourceToJson(sourceText)");
                throw new System.Exception(json);
            }
        }

        public void CanParseFormatterOptionInBaseClasses()
        {
            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("sourceText", @"
                    namespace Serene.Northwind {
                        import D = Serenity.Decorators;
                        
                        class MyBaseDecorator {
                            @D.option()
                            baseOption: number;
                        }

                        export class MyDerivedDecorator extends MyBaseDecorator implements Slick.Formatter {
                            format(ctx: Slick.FormatterContext): string {
                                return "" < b > "" + Q.htmlEncode(ctx.value) + "" </ b > "";
                            }
                           
                            @D.option()
                            derivedOption: string;
                        }
                    }");
            }
        }
    }
}