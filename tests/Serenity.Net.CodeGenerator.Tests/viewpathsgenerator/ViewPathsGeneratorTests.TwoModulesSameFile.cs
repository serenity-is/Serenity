namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void Generates_SeparateClasses_ForSameFileInDifferentModules()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class MVC
                {
                    public static partial class Views
                    {
                        public static partial class ModuleA
                        {
                            public static partial class Table1
                            {
                                public const string Table1Index = "~/Modules/ModuleA/Table1/Table1Index.cshtml";
                            }
                        }

                        public static partial class ModuleB
                        {
                            public static partial class Table1
                            {
                                public const string Table1Index = "~/Modules/ModuleB/Table1/Table1Index.cshtml";
                            }
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            "Modules/ModuleA/Table1/Table1Index.cshtml",
            "Modules/ModuleB/Table1/Table1Index.cshtml"));
    }
}