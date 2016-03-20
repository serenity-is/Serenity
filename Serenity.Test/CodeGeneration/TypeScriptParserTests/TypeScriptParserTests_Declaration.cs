using System.Collections.Generic;
using Xunit;

namespace Serenity.CodeGeneration.Test
{
    public partial class TypeScriptParserTests
    {
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
    }
}