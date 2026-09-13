namespace Serenity.CodeGeneration;

public partial class EsmEntryPointsGeneratorTests
{
    [Fact]
    public void Uses_DefaultEntryPoints()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class ESM
                {
                    public const string Test = "~/esm/Modules/Admin/Test.js";

                    public static partial class Modules
                    {
                        public static partial class Admin
                        {
                            public const string Test = "~/esm/Modules/Admin/Test.js";
                            public const string TestPage = "~/esm/Modules/Admin/TestPage.js";
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, Generate(
            "Modules/Admin/TestPage.ts",
            "Modules/Admin/Helper.ts",
            "Modules/Admin/TestPage.tsx",
            "Modules/Admin/Test.mts"));
    }
}