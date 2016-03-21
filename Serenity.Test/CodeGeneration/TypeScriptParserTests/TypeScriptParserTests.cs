using Serenity.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Xunit;

namespace Serenity.CodeGeneration.Test
{
    public partial class TypeScriptParserTests
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
    }
}