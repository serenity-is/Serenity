namespace Serenity.CodeGeneration;

public partial class EsmEntryPointsGeneratorTests
{
    [Fact]
    public void Generates_SeparateClasses_ForSamePageNameInDifferentModules()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class ESM
                {
                    public static partial class Modules
                    {
                        public static partial class TestModule
                        {
                            public static partial class Table1
                            {
                                public const string Table1Page = "~/esm/Modules/TestModule/Table1/Table1Page.js";
                            }
                        }

                        public static partial class TestModule1
                        {
                            public static partial class Table1
                            {
                                public const string Table1Page = "~/esm/Modules/TestModule1/Table1/Table1Page.js";
                            }
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, Generate(
            "Modules/TestModule/Table1/Table1Page.ts",
            "Modules/TestModule1/Table1/Table1Page.ts"));
    }
}