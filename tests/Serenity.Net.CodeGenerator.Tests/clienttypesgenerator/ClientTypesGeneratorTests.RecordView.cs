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
    public void RecordView_Card_View_Attribute_Has_The_Card_Placement_And_No_Base_Options()
    {
        var files = Generate(null, true,
            ("Modules/Ns.ts", /*lang=typescript*/ """
                export const nsTest: "MyProject.MyTest." = "MyProject.MyTest.";
                """),
            ("Modules/TestCardView.tsx", /*lang=typescript*/ """
                import { CardView } from "@serenity-is/pro.extensions"
                import { nsTest } from "./Ns"

                export class TestCardView extends CardView<{ idField?: string, default?: boolean, below?: string, hideSwitch?: boolean, columns?: number }> {
                    static override [Symbol.typeInfo] = this.registerRecordView(nsTest);
                    constructor(props: { idField?: string, default?: boolean, below?: string, hideSwitch?: boolean, columns?: number }) {
                        super(props);
                    }
                }
                """));

        var text = Read(files, "MyTest.TestCardViewAttribute.cs");
        Assert.Contains("public partial class TestCardViewAttribute : RecordViewTypeAttribute", text);
        Assert.Contains("Placement = RecordViewPlacement.Card;", text);
        Assert.Contains("public double Columns", text);
        // the ones that the base attribute has are not declared again
        Assert.DoesNotContain("Default", text.Replace("RecordViewPlacement", ""));
        Assert.DoesNotContain("Below", text);
        Assert.DoesNotContain("HideSwitch", text);
        Assert.DoesNotContain("IdField", text);
    }

    [Fact]
    public void RecordView_Hover_Card_Attribute_Has_No_Placement()
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
                }
                """));

        Assert.DoesNotContain("Placement", Read(files, "MyTest.TestCardAttribute.cs"));
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
