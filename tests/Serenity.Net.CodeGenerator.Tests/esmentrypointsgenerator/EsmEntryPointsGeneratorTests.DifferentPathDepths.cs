namespace Serenity.CodeGeneration;

public partial class EsmEntryPointsGeneratorTests
{
    [Fact]
    public void Generates_TopLevelConstants_And_NestedClasses()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class ESM
                {
                    public const string Table1Page = "~/esm/Modules/TestModule/Table1/Table1Page.js";
                    public const string Table2Page = "~/esm/Modules/Table2/Table2Page.js";
                    public const string Table3Page = "~/esm/Modules/TestModule3/Table3/Table3Page.js";
                    public const string Table4Page = "~/esm/Modules/TestModule4/SubFolder/Table4/Table4Page.js";

                    public static partial class Modules
                    {
                        public static partial class Table2
                        {
                            public const string Table2Page = "~/esm/Modules/Table2/Table2Page.js";
                        }

                        public static partial class TestModule
                        {
                            public static partial class Table1
                            {
                                public const string Table1Page = "~/esm/Modules/TestModule/Table1/Table1Page.js";
                            }
                        }

                        public static partial class TestModule3
                        {
                            public static partial class Table3
                            {
                                public const string Table3Page = "~/esm/Modules/TestModule3/Table3/Table3Page.js";
                            }
                        }

                        public static partial class TestModule4
                        {
                            public static partial class SubFolder
                            {
                                public static partial class Table4
                                {
                                    public const string Table4Page = "~/esm/Modules/TestModule4/SubFolder/Table4/Table4Page.js";
                                }
                            }
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, Generate(
            "Modules/TestModule4/SubFolder/Table4/Table4Page.ts",
            "Modules/TestModule/Table1/Table1Page.ts",
            "Modules/Table2/Table2Page.ts",
            "Modules/TestModule3/Table3/Table3Page.ts"));
    }
}