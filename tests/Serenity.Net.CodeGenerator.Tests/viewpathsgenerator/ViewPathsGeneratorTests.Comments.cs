namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void EmitsComments_WhenNotOmitted()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class MVC
                {
                    /// <summary>Provides paths to view files.</summary>
                    public static partial class Views
                    {
                        /// <summary>Provides view paths in the <c>Admin</c> folder.</summary>
                        public static partial class Admin
                        {
                            /// <summary>The path to the <c>class</c> view.</summary>
                            public const string Class = "~/Modules/Admin/class.cshtml";
                            /// <summary>Provides view paths in the <c>Admin/User</c> folder.</summary>
                            public static partial class User
                            {
                                /// <summary>The path to the <c>UserIndex</c> view.</summary>
                                public const string UserIndex = "~/Modules/Admin/User/UserIndex.cshtml";
                            }
                        }

                        /// <summary>Provides view paths in the <c>ÆDmÏn</c> folder.</summary>
                        public static partial class ÆDmÏn
                        {
                            /// <summary>The path to the <c>T,;a</c> view.</summary>
                            public const string T__a = "~/Modules/æDmÏn/T,;a.cshtml";
                        }

                        /// <summary>Provides view paths in the <c>ModulePage</c> folder.</summary>
                        public static partial class ModulePage
                        {
                            /// <summary>The path to the <c>ModulePage</c> view.</summary>
                            public const string ModulePage_ = "~/Modules/ModulePage/ModulePage.cshtml";
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            new Options() { OmitComments = false },
            "Modules/Admin/User/UserIndex.cshtml",
            "Modules/Admin/class.cshtml",
            "Modules/ModulePage/ModulePage.cshtml",
            "Modules/æDmÏn/T,;a.cshtml"));
    }
}