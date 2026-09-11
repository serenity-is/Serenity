using System.Xml.Linq;

namespace Serenity.Data;

public class ForXmlHelperTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Enumerate_Returns_Empty_For_Null_Or_Empty(string? forXml)
    {
        Assert.Empty(ForXmlHelper.Enumerate(forXml));
    }

    [Fact]
    public void Enumerate_Returns_Row_Elements()
    {
        var elements = ForXmlHelper.Enumerate("""<row ID="1"/><row ID="2"/>""").ToList();

        Assert.Equal(2, elements.Count);
        Assert.Equal("1", elements[0].Attr("ID"));
        Assert.Equal("2", elements[1].Attr("ID"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ToRows_Returns_Empty_For_Null_Or_Empty(string? forXml)
    {
        Assert.Empty(ForXmlHelper.ToRows<IdNameRow>(forXml, (_, _) => { }));
    }

    [Fact]
    public void ToRows_Invokes_ReadRow_For_Each_Row()
    {
        var rows = ForXmlHelper.ToRows<IdNameRow>(
            """<row ID="1" Name="One"/><row ID="2" Name="Two"/>""",
            (element, row) =>
            {
                row.ID = int.Parse(element.Attr("ID")!);
                row.Name = element.Attr("Name")!;
            });

        Assert.Equal(2, rows.Count);
        Assert.Equal(1, rows[0].ID);
        Assert.Equal("One", rows[0].Name);
        Assert.Equal(2, rows[1].ID);
        Assert.Equal("Two", rows[1].Name);
    }

    [Fact]
    public void Attr_Returns_Null_For_Null_Element()
    {
        Assert.Null(ForXmlHelper.Attr(null!, "ID"));
    }

    [Fact]
    public void Attr_Returns_Null_For_Missing_Attribute()
    {
        Assert.Null(XElement.Parse("<row />").Attr("ID"));
    }

    [Fact]
    public void Attr_Returns_Attribute_Value()
    {
        Assert.Equal("1", XElement.Parse("""<row ID="1" />""").Attr("ID"));
    }
}
