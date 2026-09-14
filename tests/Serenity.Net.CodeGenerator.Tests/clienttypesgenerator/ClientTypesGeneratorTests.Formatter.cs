namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    [Fact]
    public void Formatter_From_Base_Class()
    {
        var files = Generate(null, true, ("Modules/TestFormatter.ts", /*lang=typescript*/ """
            import { Decorators, FormatterBase } from "@serenity-is/corelib"

            @Decorators.registerFormatter('MyProject.MyTest.TestFormatter')
            export class TestFormatter extends FormatterBase {
                constructor(public readonly props: { genderProperty?: string } = {}) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestFormatterAttribute.cs");
        Assert.Equal("""
            using Serenity;
            using Serenity.ComponentModel;
            using System;
            using System.Collections.Generic;
            using System.ComponentModel;

            namespace MyProject.MyTest
            {
                public partial class TestFormatterAttribute : CustomFormatterAttribute
                {
                    public const string Key = "MyProject.MyTest.TestFormatter";

                    public TestFormatterAttribute()
                        : base(Key)
                    {
                    }

                    public string GenderProperty
                    {
                        get { return GetOption<string>("genderProperty"); }
                        set { SetOption("genderProperty", value); }
                    }
                }
            }
            """.ReplaceLineEndings(), text);
    }

    [Fact]
    public void Formatter_From_Formatter_Interface()
    {
        var files = Generate(null, true,
            ("Modules/Ns.ts", /*lang=typescript*/ """
                export const nsTest: "MyProject.MyTest." = "MyProject.MyTest.";
                """),
            ("Modules/TestFormatter.ts", /*lang=typescript*/ """
                import { Formatter, formatterTypeInfo, registerType } from "@serenity-is/corelib"
                import { nsTest } from "./Ns"

                export class TestFormatter implements Formatter {
                    static [Symbol.typeInfo] = formatterTypeInfo(nsTest); static { registerType(this); }
                }
                """));

        var text = Read(files, "MyTest.TestFormatterAttribute.cs");
        Assert.Contains("public partial class TestFormatterAttribute : CustomFormatterAttribute", text);
        Assert.Contains("public const string Key = \"MyProject.MyTest.TestFormatter\";", text);
    }
}
