namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    [Fact]
    public void RecordView_Generates_Attribute_With_Options()
    {
        var files = Generate(null, true,
            ("Modules/Ns.ts", /*lang=typescript*/ """
                export const nsTest: "MyProject.MyTest." = "MyProject.MyTest.";
                """),
            ("Modules/TestCard.tsx", /*lang=typescript*/ """
                import { HoverCard } from "@serenity-is/pro.extensions"
                import { nsTest } from "./Ns"

                export class TestCard extends HoverCard<any> {
                    static override [Symbol.typeInfo] = this.registerRecordView(nsTest);
                    constructor(props: { idField?: string, showRevenue?: boolean }) {
                        super(props);
                    }
                }
                """));

        var text = Read(files, "MyTest.TestCardAttribute.cs");
        Assert.Equal("""
            using Serenity;
            using Serenity.ComponentModel;
            using System;
            using System.Collections.Generic;
            using System.ComponentModel;

            namespace MyProject.MyTest
            {
                public partial class TestCardAttribute : RecordViewTypeAttribute
                {
                    public const string Key = "MyProject.MyTest.TestCard";

                    public TestCardAttribute()
                        : base(Key)
                    {
                    }

                    public bool ShowRevenue
                    {
                        get { return GetOption<bool>("showRevenue"); }
                        set { SetOption("showRevenue", value); }
                    }
                }
            }
            """.ReplaceLineEndings(), text);
    }

    [Fact]
    public void RecordView_Abstract_Base_Does_Not_Generate_Attribute()
    {
        var files = Generate(null, true,
            ("Modules/BaseCard.tsx", /*lang=typescript*/ """
                import { HoverCard } from "@serenity-is/pro.extensions"

                export abstract class BaseCard extends HoverCard<any> {
                    static override [Symbol.typeInfo] = this.registerRecordView("MyProject.BaseCard");
                }
                """));

        Assert.DoesNotContain(files, x => x.Filename.EndsWith("BaseCardAttribute.cs", StringComparison.Ordinal));
    }
}
