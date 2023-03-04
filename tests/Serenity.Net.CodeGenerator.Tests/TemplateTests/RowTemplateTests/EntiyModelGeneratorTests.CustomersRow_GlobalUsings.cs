using System.Text.RegularExpressions;

namespace Serenity.Tests.CodeGenerator;

public partial class RowTemplateTests
{
    [Fact]
    public void CustomersRow_GlobalUsings_ComponentModel()
    {
        var model = new MockEntityModel();
        model.GlobalUsings.Add("Serenity.ComponentModel");
        model.GlobalUsings.Add("System.ComponentModel");
        var actual = RenderTemplate(model);
        var expected = Regex.Replace(ExpectedDefault, 
            @"^using (Serenity|System)\.ComponentModel\s*;\r?\n", "", 
            RegexOptions.Multiline);

        AssertEqual(expected, actual);
        foreach (var ns in model.GlobalUsings)
            Assert.DoesNotContain(ns, actual);
    }

    [Fact]
    public void CustomersRow_GlobalUsings_All()
    {
        var model = new MockEntityModel();
        model.GlobalUsings.Add("Serenity.ComponentModel");
        model.GlobalUsings.Add("Serenity.Data");
        model.GlobalUsings.Add("Serenity.Data.Mapping");
        model.GlobalUsings.Add("System.ComponentModel");
        var actual = RenderTemplate(model);
        var expected = Regex.Replace(ExpectedDefault,
            @"^using.*;\r?\n", "",
            RegexOptions.Multiline).TrimStart();

        AssertEqual(expected, actual);
        foreach (var ns in model.GlobalUsings)
            Assert.DoesNotContain(ns, actual);
    }

    [Fact]
    public void CustomersRow_GlobalUsings_Unused()
    {
        var model = new MockEntityModel();
        model.GlobalUsings.Add("System.Threading.Tasks");
        var actual = RenderTemplate(model);
        AssertEqual(ExpectedDefault, actual);
        foreach (var ns in model.GlobalUsings)
            Assert.DoesNotContain(ns, actual);
    }
}
