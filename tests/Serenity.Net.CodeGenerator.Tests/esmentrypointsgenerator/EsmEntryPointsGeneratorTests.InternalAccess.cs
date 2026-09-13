namespace Serenity.CodeGeneration;

public partial class EsmEntryPointsGeneratorTests
{
    [Fact]
    public void Uses_InternalAccess()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                internal static partial class ESM
                {
                    public const string AnotherPage = "~/esm/Modules/AnotherPage.js";
                    public const string SamplePage = "~/esm/Modules/Sample/SamplePage.js";
                    public const string ScriptInit = "~/esm/Modules/Common/ScriptInit.js";
                    public const string Sub2Page = "~/esm/Modules/Sample/SubPage/Sub2Page.js";
                    public const string SubPage = "~/esm/Modules/Sample/SubPage/SubPage.js";
                    public const string TestPage = "~/esm/Modules/Admin/TestPage.js";

                    public static partial class Modules
                    {
                        public static partial class Admin
                        {
                            public const string TestPage = "~/esm/Modules/Admin/TestPage.js";
                        }

                        public const string AnotherPage = "~/esm/Modules/AnotherPage.js";

                        public static partial class Common
                        {
                            public const string ScriptInit = "~/esm/Modules/Common/ScriptInit.js";
                        }

                        public static partial class Sample
                        {
                            public const string SamplePage = "~/esm/Modules/Sample/SamplePage.js";

                            public static partial class SubPage
                            {
                                public const string Sub2Page = "~/esm/Modules/Sample/SubPage/Sub2Page.js";
                                public const string SubPage_ = "~/esm/Modules/Sample/SubPage/SubPage.js";
                            }
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, Generate(
            new Options() { InternalAccess = true },
            "Modules/Admin/TestPage.ts",
            "Modules/Common/ScriptInit.ts",
            "Modules/Sample/SamplePage.ts",
            "Modules/Sample/SubPage/SubPage.ts",
            "Modules/Sample/SubPage/Sub2Page.ts",
            "Modules/AnotherPage.ts"));
    }
}