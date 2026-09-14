namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    [Fact]
    public void BasicType_Generates_Properties()
    {
        var files = Generate(null, true, ("Modules/TestOptions.ts", /*lang=typescript*/ """
            import { TransformInclude } from "@serenity-is/corelib"

            export interface TestOptions extends TransformInclude {
                minPasswordLength?: number;
                token?: string;
            }
            """));

        var text = Read(files, "TestOptions.generated.cs");
        Assert.Equal("""
            namespace MyProject
            {
                public partial class TestOptions
                {
                    public double? minPasswordLength { get; set; }

                    public string token { get; set; }
                }
            }
            """.ReplaceLineEndings(), text);
    }

    [Fact]
    public void BasicType_NullableRefTypes_Enabled()
    {
        var files = Generate("enable", true, ("Modules/TestOptions.ts", /*lang=typescript*/ """
            import { TransformInclude } from "@serenity-is/corelib"

            export interface TestOptions extends TransformInclude {
                minPasswordLength?: number;
                token?: string;
            }
            """));

        var text = Read(files, "TestOptions.generated.cs");
        Assert.Equal("""
            #nullable enable

            namespace MyProject
            {
                public partial class TestOptions
                {
                    public double? minPasswordLength { get; set; }

                    public string? token { get; set; }
                }
            }
            """.ReplaceLineEndings(), text);
    }
}
