namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void Generates_NestedClasses_ForDifferentPathDepths()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class MVC
                {
                    public static partial class Views
                    {
                        public static partial class _underscore_folder_
                        {
                            public const string Test12Index = "~/Modules/_underscore_folder_/Test12Index.cshtml";
                        }

                        public static partial class _dotFolder
                        {
                            public const string Test10Index = "~/Modules/.dotFolder/Test10Index.cshtml";
                        }

                        public static partial class _bracesFolder_
                        {
                            public const string Test11Index = "~/Modules/[bracesFolder]/Test11Index.cshtml";
                        }

                        public static partial class _0numberFolder
                        {
                            public const string Test14Index = "~/Modules/0numberFolder/Test14Index.cshtml";
                        }

                        public static partial class Dotinthemiddle_folder
                        {
                            public const string Test13Index = "~/Modules/dotinthemiddle.folder/Test13Index.cshtml";
                        }

                        public static partial class ModuleA
                        {
                            public static partial class Table1
                            {
                                public const string Table1Index = "~/Modules/ModuleA/Table1/Table1Index.cshtml";
                            }
                        }

                        public static partial class SameName
                        {
                            public static partial class SameName_
                            {
                                public static partial class SameName
                                {
                                    public const string Test15Index = "~/Modules/SameName/SameName/SameName/Test15Index.cshtml";
                                    public const string Test16Index = "~/Modules/SameName/SameName/SameName/Test16Index.cshtml";
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
                                        public const string Test6Index = "~/Modules/SubPath1/SubPath2/SubPath3/SubPath4/Test6Index.cshtml";
                                    }

                                    public const string Test5Index = "~/Modules/SubPath1/SubPath2/SubPath3/Test5Index.cshtml";
                                }

                                public const string Test3Index = "~/Modules/SubPath1/SubPath2/Test3Index.cshtml";
                                public const string Test4Index = "~/Modules/SubPath1/SubPath2/Test4Index.cshtml";
                            }

                            public const string Test2Index = "~/Modules/SubPath1/Test2Index.cshtml";
                        }

                        public static partial class SubPathx1
                        {
                            public static partial class SubPathx2
                            {
                                public static partial class SubPathx3
                                {
                                    public const string Test7Index = "~/Modules/SubPathx1/SubPathx2/SubPathx3/Test7Index.cshtml";
                                }
                            }
                        }

                        public static partial class SubPathy1
                        {
                            public static partial class SubPathy2
                            {
                                public static partial class SubPathy3
                                {
                                    public const string Test8Index = "~/Modules/SubPathy1/SubPathy2/SubPathy3/Test8Index.cshtml";
                                    public const string Test9Index = "~/Modules/SubPathy1/SubPathy2/SubPathy3/Test9Index.cshtml";
                                }
                            }
                        }

                        public const string Test1Index = "~/Modules/Test1Index.cshtml";
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            "Modules/ModuleA/Table1/Table1Index.cshtml",
            "Modules/Test1Index.cshtml",
            "Modules/SubPath1/Test2Index.cshtml",
            "Modules/SubPath1/SubPath2/Test3Index.cshtml",
            "Modules/SubPath1/SubPath2/Test4Index.cshtml",
            "Modules/SubPath1/SubPath2/SubPath3/Test5Index.cshtml",
            "Modules/SubPath1/SubPath2/SubPath3/SubPath4/Test6Index.cshtml",
            "Modules/SubPathx1/SubPathx2/SubPathx3/Test7Index.cshtml",
            "Modules/SubPathy1/SubPathy2/SubPathy3/Test8Index.cshtml",
            "Modules/SubPathy1/SubPathy2/SubPathy3/Test9Index.cshtml",
            "Modules/.dotFolder/Test10Index.cshtml",
            "Modules/[bracesFolder]/Test11Index.cshtml",
            "Modules/_underscore_folder_/Test12Index.cshtml",
            "Modules/dotinthemiddle.folder/Test13Index.cshtml",
            "Modules/0numberFolder/Test14Index.cshtml",
            "Modules/SameName/SameName/SameName/Test15Index.cshtml",
            "Modules/SameName/SameName/SameName/Test16Index.cshtml"));
    }
}