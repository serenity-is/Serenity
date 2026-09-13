namespace Serenity.CodeGeneration;

public partial class EsmEntryPointsGeneratorTests
{
    [Fact]
    public void Handles_TooManyPagesInDifferentPathDepths()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class ESM
                {
                    public const string SameName_Page = "~/esm/Modules/SameNamePage/SameNamePage/SameNamePage/SameName.Page.js";
                    public const string SameNamePage = "~/esm/Modules/SameNamePage/SameNamePage/SameNamePage/SameNamePage.js";
                    public const string Test10Page = "~/esm/Modules/.dotFolder/Test10Page.js";
                    public const string Test11Page = "~/esm/Modules/[bracesFolder]/Test11Page.js";
                    public const string Test12Page = "~/esm/Modules/_underscore_folder_/Test12Page.js";
                    public const string Test13Page = "~/esm/Modules/dotinthemiddle.folder/Test13Page.js";
                    public const string Test14Page = "~/esm/Modules/0numberFolder/Test14Page.js";
                    public const string Test15Page = "~/esm/Modules/SameName/SameName/SameName/Test15Page.js";
                    public const string Test16Page = "~/esm/Modules/SameName/SameName/SameName/Test16Page.js";
                    public const string Test1Page = "~/esm/Modules/Test1Page.js";
                    public const string Test2Page = "~/esm/Modules/SubPath1/Test2Page.js";
                    public const string Test3Page = "~/esm/Modules/SubPath1/SubPath2/Test3Page.js";
                    public const string Test4Page = "~/esm/Modules/SubPath1/SubPath2/Test4Page.js";
                    public const string Test5Page = "~/esm/Modules/SubPath1/SubPath2/SubPath3/Test5Page.js";
                    public const string Test6Page = "~/esm/Modules/SubPath1/SubPath2/SubPath3/SubPath4/Test6Page.js";
                    public const string Test7Page = "~/esm/Modules/SubPathx1/SubPathx2/SubPathx3/Test7Page.js";
                    public const string Test8Page = "~/esm/Modules/SubPathy1/SubPathy2/SubPathy3/Test8Page.js";
                    public const string Test9Page = "~/esm/Modules/SubPathy1/SubPathy2/SubPathy3/Test9Page.js";

                    public static partial class Modules
                    {
                        public static partial class _underscore_folder_
                        {
                            public const string Test12Page = "~/esm/Modules/_underscore_folder_/Test12Page.js";
                        }

                        public static partial class _dotFolder
                        {
                            public const string Test10Page = "~/esm/Modules/.dotFolder/Test10Page.js";
                        }

                        public static partial class _bracesFolder_
                        {
                            public const string Test11Page = "~/esm/Modules/[bracesFolder]/Test11Page.js";
                        }

                        public static partial class _0numberFolder
                        {
                            public const string Test14Page = "~/esm/Modules/0numberFolder/Test14Page.js";
                        }

                        public static partial class Dotinthemiddle_folder
                        {
                            public const string Test13Page = "~/esm/Modules/dotinthemiddle.folder/Test13Page.js";
                        }

                        public static partial class SameName
                        {
                            public static partial class SameName_
                            {
                                public static partial class SameName
                                {
                                    public const string Test15Page = "~/esm/Modules/SameName/SameName/SameName/Test15Page.js";
                                    public const string Test16Page = "~/esm/Modules/SameName/SameName/SameName/Test16Page.js";
                                }
                            }
                        }

                        public static partial class SameNamePage
                        {
                            public static partial class SameNamePage_
                            {
                                public static partial class SameNamePage
                                {
                                    public const string SameName_Page = "~/esm/Modules/SameNamePage/SameNamePage/SameNamePage/SameName.Page.js";
                                    public const string SameNamePage_ = "~/esm/Modules/SameNamePage/SameNamePage/SameNamePage/SameNamePage.js";
                                }
                            }
                        }

                        public static partial class SubPath1
                        {
                            public static partial class SubPath2
                            {
                                public static partial class SubPath3
                                {
                                    public static partial class SubPath4
                                    {
                                        public const string Test6Page = "~/esm/Modules/SubPath1/SubPath2/SubPath3/SubPath4/Test6Page.js";
                                    }

                                    public const string Test5Page = "~/esm/Modules/SubPath1/SubPath2/SubPath3/Test5Page.js";
                                }

                                public const string Test3Page = "~/esm/Modules/SubPath1/SubPath2/Test3Page.js";
                                public const string Test4Page = "~/esm/Modules/SubPath1/SubPath2/Test4Page.js";
                            }

                            public const string Test2Page = "~/esm/Modules/SubPath1/Test2Page.js";
                        }

                        public static partial class SubPathx1
                        {
                            public static partial class SubPathx2
                            {
                                public static partial class SubPathx3
                                {
                                    public const string Test7Page = "~/esm/Modules/SubPathx1/SubPathx2/SubPathx3/Test7Page.js";
                                }
                            }
                        }

                        public static partial class SubPathy1
                        {
                            public static partial class SubPathy2
                            {
                                public static partial class SubPathy3
                                {
                                    public const string Test8Page = "~/esm/Modules/SubPathy1/SubPathy2/SubPathy3/Test8Page.js";
                                    public const string Test9Page = "~/esm/Modules/SubPathy1/SubPathy2/SubPathy3/Test9Page.js";
                                }
                            }
                        }

                        public const string Test1Page = "~/esm/Modules/Test1Page.js";
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, Generate(
            "Modules/Test1Page.ts",
            "Modules/SubPath1/Test2Page.ts",
            "Modules/SubPath1/SubPath2/Test3Page.ts",
            "Modules/SubPath1/SubPath2/Test4Page.ts",
            "Modules/SubPath1/SubPath2/SubPath3/Test5Page.ts",
            "Modules/SubPath1/SubPath2/SubPath3/SubPath4/Test6Page.ts",
            "Modules/SubPathx1/SubPathx2/SubPathx3/Test7Page.ts",
            "Modules/SubPathy1/SubPathy2/SubPathy3/Test8Page.ts",
            "Modules/SubPathy1/SubPathy2/SubPathy3/Test9Page.ts",
            "Modules/.dotFolder/Test10Page.ts",
            "Modules/[bracesFolder]/Test11Page.ts",
            "Modules/_underscore_folder_/Test12Page.ts",
            "Modules/dotinthemiddle.folder/Test13Page.ts",
            "Modules/0numberFolder/Test14Page.ts",
            "Modules/SameName/SameName/SameName/Test15Page.ts",
            "Modules/SameName/SameName/SameName/Test16Page.ts",
            "Modules/SameNamePage/SameNamePage/SameNamePage/SameNamePage.ts",
            "Modules/SameNamePage/SameNamePage/SameNamePage/SameName.Page.ts"));
    }
}