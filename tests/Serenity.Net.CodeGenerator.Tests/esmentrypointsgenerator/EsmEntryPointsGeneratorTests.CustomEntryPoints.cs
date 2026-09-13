namespace Serenity.CodeGeneration;

public partial class EsmEntryPointsGeneratorTests
{
    [Fact]
    public void Uses_CustomEntryPoints()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class ESM
                {
                    public const string Helper = "~/esm/Modules/Admin/Helper.js";
                    public const string TestPage = "~/esm/Modules/Admin/TestPage.js";

                    public static partial class Modules
                    {
                        public static partial class Admin
                        {
                            public const string Helper = "~/esm/Modules/Admin/Helper.js";
                            public const string TestPage = "~/esm/Modules/Admin/TestPage.js";
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, Generate(
            new Options() { EntryPoints = ["Modules/**/*.ts"] },
            "Modules/Admin/TestPage.ts",
            "Modules/Admin/Helper.ts"));
    }
}