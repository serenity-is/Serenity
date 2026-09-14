namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    [Fact]
    public void BasicType_Maps_Member_Types_And_Skips_Complex()
    {
        var files = Generate(null, true, ("Modules/TestOptions.ts", /*lang=typescript*/ """
            import { TransformInclude } from "@serenity-is/corelib"

            export interface TestOptions extends TransformInclude {
                boolProp?: boolean;
                dateProp?: Date;
                numProp?: number;
                strProp?: string;
                complexProp?: Function;
            }
            """));

        var text = Read(files, "TestOptions.generated.cs");
        Assert.Equal("""
            namespace MyProject
            {
                public partial class TestOptions
                {
                    public bool? boolProp { get; set; }

                    public DateTime dateProp { get; set; }

                    public double? numProp { get; set; }

                    public string strProp { get; set; }
                }
            }
            """.ReplaceLineEndings(), text);
    }
}
