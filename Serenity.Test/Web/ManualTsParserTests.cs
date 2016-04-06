using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Serenity.CodeGeneration.Test
{
    public partial class ManualTsParserTests
    {
        [Fact]
        public void ParserDoesntFailOnEmptyFile()
        {
            var parser = new TypeScriptParser();
            Token reportedToken = null;
            parser.ReportToken += (token) =>
            {
                reportedToken = token;
            };
            parser.Parse("");
            Assert.Equal(TokenType.End, reportedToken.Type);
        }

        [Fact]
        public void CanParseGenericClass()
        {
            var parser = new TypeScriptParser();
            var types = new List<TypeScriptParser.TypeInfo>();

            parser.ReportType += (type) =>
            {
                types.Add(type);
            };

            parser.Parse(@"
class EntityDialog<TEntity>
{
    dialogOpen(): void;
    loadByIdAndOpenDialog(id: any): void;
}");

            Assert.Equal(1, types.Count);
            var t0 = types[0];
            Assert.Equal("EntityDialog<TEntity>", t0.Name);
            Assert.Equal(2, t0.Members.Count);
        }

        [Fact]
        public void CanParseGenericClassWithExtendsConstraint()
        {
            var parser = new TypeScriptParser();
            var types = new List<TypeScriptParser.TypeInfo>();

            parser.ReportType += (type) =>
            {
                types.Add(type);
            };

            parser.Parse(@"
interface ServiceOptions<TResponse extends ServiceResponse> extends JQueryAjaxSettings {
    request?: any;
}");

            Assert.Equal(1, types.Count);
            Assert.Equal("ServiceOptions<TResponse extends ServiceResponse>", types[0].Name);
        }

        [Fact]
        public void CanParseMembers()
        {
            var parser = new TypeScriptParser();
            var types = new List<TypeScriptParser.TypeInfo>();

            parser.ReportType += (type) =>
            {
                types.Add(type);
            };

            parser.Parse(Input_Lookup);

            Assert.Equal(1, types.Count);

            var t0 = types[0];
            Assert.Equal("Lookup<TItem>", t0.Name);
            Assert.False(t0.IsDeclaration);
            Assert.Equal("Serenity", t0.Namespace.Name);
            Assert.False(t0.Namespace.IsDeclaration);

            Assert.Equal(6, t0.Members.Count);
        }

        public const string Input_Lookup = @"
namespace Serenity {
    export class Lookup<TItem> {
        private items: TItem[] = [];
        private itemById?: { [key: string]: TItem } = {};

        constructor(private options: LookupOptions<TItem>, items?: TItem[]) {
            if (items != null)
                this.update(items);
        }
        
        update(value: TItem[]) {
            this.items = [];
            this.itemById = {};
            if (value) {
                for (var k of value)
                    this.items.push(k);
            }
            var idField = this.options.idField;
            if (!Q.isEmptyOrNull(idField)) {
                for (var r of this.items) {
                    var v = r[idField];
                    if (v != null) {
                        this.itemById[v] = r;
                    }
                }
            }
        }

        public get_idField() {
            return this.options.idField;
        }

        protected get_parentIdField() {
            return this.options.parentIdField;
        }
    }
}";
        public void ParserTest()
        {
            var parser = new TypeScriptParser();
            var types = new List<TypeScriptParser.TypeInfo>();

            StringBuilder sb = new StringBuilder();

            parser.ReportToken += (token) =>
            {
                if (token.Type != TokenType.WhiteSpace &&
                   token.Type != TokenType.EndOfLine)
                    sb.AppendLine(JSON.StringifyIndented(token)); ;
            };

            parser.ReportType += (type) =>
            {
                types.Add(type);
            };

            parser.Parse(File.ReadAllText(@"P:\Sandbox\Serene\Serenity\Serenity.Script.Core\Resources\Serenity.CoreLib.ts"));
            throw new Exception(JSON.StringifyIndented(types) + sb.ToString());
        }

        [Fact]
        public void SetsIsDeclarationCorrectly()
        {
            var parser = new TypeScriptParser();
            var types = new List<TypeScriptParser.TypeInfo>();

            parser.ReportType += (type) =>
            {
                types.Add(type);
            };

            parser.Parse(Input_Declaration);

            Assert.Equal(6, types.Count);

            var t0 = types[0];
            Assert.Equal("A", t0.Name);
            Assert.False(t0.IsDeclaration);
            Assert.Equal("Normal", t0.Namespace.Name);
            Assert.False(t0.Namespace.IsDeclaration);

            var t1 = types[1];
            Assert.Equal("B", t1.Name);
            Assert.False(t1.IsDeclaration);
            Assert.Equal("Normal.Sub", t1.Namespace.Name);
            Assert.False(t1.Namespace.IsDeclaration);

            var t2 = types[2];
            Assert.Equal("C", t2.Name);
            Assert.True(t2.IsDeclaration);
            Assert.Equal("Declared", t2.Namespace.Name);
            Assert.True(t2.Namespace.IsDeclaration);

            var t3 = types[3];
            Assert.Equal("S", t3.Name);
            Assert.True(t3.IsDeclaration);
            Assert.Equal("Declared.DSub", t3.Namespace.Name);
            Assert.True(t3.Namespace.IsDeclaration);

            var t4 = types[4];
            Assert.Equal("D", t4.Name);
            Assert.False(t4.IsDeclaration);
            Assert.Equal("", t4.Namespace.Name);
            Assert.False(t4.Namespace.IsDeclaration);

            var t5 = types[5];
            Assert.Equal("E", t5.Name);
            Assert.True(t5.IsDeclaration);
            Assert.Equal("", t5.Namespace.Name);
            Assert.False(t5.Namespace.IsDeclaration);
        }

        public const string Input_Declaration = @"
    namespace Normal {
        class A {
        }

        namespace Sub {
            interface B {
            }
        }
    }

    declare namespace Declared {
        class C {
        }

        namespace DSub {
            class S {
            }
        }
    }

    class D {
    }

    declare class E {
    }
";

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
    }
}