using System.Collections.Generic;
using Xunit;

namespace Serenity.CodeGeneration.Test
{
    public partial class TypeScriptParserTests
    {
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
    }
}