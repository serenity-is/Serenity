namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    [Fact]
    public void BasicType_Includes_Inherited_Members()
    {
        var files = Generate(null, true, ("Modules/TestOptions.ts", /*lang=typescript*/ """
            import { TransformInclude } from "@serenity-is/corelib"

            export interface BaseOptions {
                baseProp?: number;
            }

            export interface TestOptions extends BaseOptions, TransformInclude {
                ownProp?: string;
            }
            """));

        var text = Read(files, "TestOptions.generated.cs");
        Assert.Equal("""
            namespace MyProject
            {
                public partial class TestOptions
                {
                    public double? baseProp { get; set; }

                    public string ownProp { get; set; }
                }
            }
            """.ReplaceLineEndings(), text);
    }

    [Fact]
    public void BasicType_Skips_Non_Interfaces()
    {
        var files = Generate(null, true, ("Modules/NotBasic.ts", /*lang=typescript*/ """
            import { TransformInclude } from "@serenity-is/corelib"

            export class NotGenerated implements TransformInclude {
                someProp?: string;
            }

            export interface PlainInterface {
                otherProp?: string;
            }
            """));

        Assert.Empty(files);
    }
}
