namespace Serenity.CodeGeneration;

public partial class EsmEntryPointsGeneratorTests
{
    [Fact]
    public void Uses_CustomEsmAssetBasePath()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class ESM
                {
                    public const string AnotherPage = "~/assets/esm/Modules/AnotherPage.js";
                    public const string SamplePage = "~/assets/esm/Modules/Sample/SamplePage.js";
                    public const string ScriptInit = "~/assets/esm/Modules/Common/ScriptInit.js";
                    public const string Sub2Page = "~/assets/esm/Modules/Sample/SubPage/Sub2Page.js";
                    public const string SubPage = "~/assets/esm/Modules/Sample/SubPage/SubPage.js";
                    public const string TestPage = "~/assets/esm/Modules/Admin/TestPage.js";

                    public static partial class Modules
                    {
                        public static partial class Admin
                        {
                            public const string TestPage = "~/assets/esm/Modules/Admin/TestPage.js";
                        }

                        public const string AnotherPage = "~/assets/esm/Modules/AnotherPage.js";

                        public static partial class Common
                        {
                            public const string ScriptInit = "~/assets/esm/Modules/Common/ScriptInit.js";
                        }

                        public static partial class Sample
                        {
                            public const string SamplePage = "~/assets/esm/Modules/Sample/SamplePage.js";

                            public static partial class SubPage
                            {
                                public const string Sub2Page = "~/assets/esm/Modules/Sample/SubPage/Sub2Page.js";
                                public const string SubPage_ = "~/assets/esm/Modules/Sample/SubPage/SubPage.js";
                            }
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, Generate(
            new Options() { EsmAssetBasePath = "/assets/esm" },
            "Modules/Admin/TestPage.ts",
            "Modules/Common/ScriptInit.ts",
            "Modules/Sample/SamplePage.ts",
            "Modules/Sample/SubPage/SubPage.ts",
            "Modules/Sample/SubPage/Sub2Page.ts",
            "Modules/AnotherPage.ts"));
    }
}