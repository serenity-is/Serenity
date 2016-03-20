using Serenity.Web;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
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

         public const string Input_InterfaceDialogFormatter = @"
interface CustomerRow {
    ID: number;
    CustomerID: string;
}

namespace Serene.Northwind {
    import D = Serenity.Decorators;

    @D.formKey(""Northwind.Customer"") @D.idProperty(""ID"") @D.nameProperty(""CustomerID"") 
    @D.service(""Northwind/Customer"") @D.flexify() @D.maximizable()
    export class MyCustomerDialog extends Serenity.EntityDialog<CustomerRow> {
    }

    export class MyBoldFormatter implements Slick.Formatter
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