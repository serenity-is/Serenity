namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    private const string NsExtensionsPath = "Modules/Ns.ts";

    private const string NsExtensions = /*lang=typescript*/ """
        export const nsExtensions: "Serenity.Extensions." = "Serenity.Extensions.";
        """;

    [Fact]
    public void TransformInclude_TypeName_From_Namespace_Constant()
    {
        var files = Generate(null, true,
            (NsExtensionsPath, NsExtensions),
            ("Modules/ResetPasswordOptions.ts", /*lang=typescript*/ """
                import { TransformInclude } from "@serenity-is/corelib";
                import { nsExtensions } from "./Ns";

                export interface ResetPasswordOptions extends TransformInclude<typeof nsExtensions> {
                    token: string;
                    minPasswordLength: number;
                }
                """));

        var text = Read(files, "Extensions.ResetPasswordOptions.generated.cs");
        Assert.Equal("""
            namespace Serenity.Extensions
            {
                public partial class ResetPasswordOptions
                {
                    public double? minPasswordLength { get; set; }

                    public string token { get; set; }
                }
            }
            """.ReplaceLineEndings(), text);
    }

    [Fact]
    public void TransformInclude_TypeName_From_Namespace_Literal()
    {
        var files = Generate(null, true, ("Modules/ResetPasswordOptions.ts", /*lang=typescript*/ """
            import { TransformInclude } from "@serenity-is/corelib";

            export interface ResetPasswordOptions extends TransformInclude<"Serenity.Extensions.Membership."> {
                token: string;
            }
            """));

        var text = Read(files, "Extensions.Membership.ResetPasswordOptions.generated.cs");
        Assert.Equal("""
            namespace Serenity.Extensions.Membership
            {
                public partial class ResetPasswordOptions
                {
                    public string token { get; set; }
                }
            }
            """.ReplaceLineEndings(), text);
    }

    [Fact]
    public void TransformInclude_TypeName_From_FullName_Literal()
    {
        var files = Generate(null, true, ("Modules/ResetPasswordOptions.ts", /*lang=typescript*/ """
            import { TransformInclude } from "@serenity-is/corelib";

            export interface ResetPasswordOptions extends TransformInclude<"Serenity.Extensions.SomeOtherModule.ResetPasswordOptions"> {
                token: string;
            }
            """));

        var text = Read(files, "Extensions.SomeOtherModule.ResetPasswordOptions.generated.cs");
        Assert.Equal("""
            namespace Serenity.Extensions.SomeOtherModule
            {
                public partial class ResetPasswordOptions
                {
                    public string token { get; set; }
                }
            }
            """.ReplaceLineEndings(), text);
    }

    [Fact]
    public void TransformInclude_Without_TypeName_Uses_Root_Namespace()
    {
        var files = Generate(null, true, ("Modules/TestOptions.ts", /*lang=typescript*/ """
            import { TransformInclude } from "@serenity-is/corelib"

            export interface TestOptions extends TransformInclude {
                token?: string;
            }
            """));

        var text = Read(files, "TestOptions.generated.cs");
        Assert.Equal("""
            namespace MyProject
            {
                public partial class TestOptions
                {
                    public string token { get; set; }
                }
            }
            """.ReplaceLineEndings(), text);
    }
}
