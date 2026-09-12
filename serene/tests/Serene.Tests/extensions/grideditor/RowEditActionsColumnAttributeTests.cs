namespace Serenity.ComponentModel;

public class RowEditActionsColumnAttributeTests
{
    [Fact]
    public void PropertyAttributes_Is_Null_By_Default()
    {
        var attribute = new RowEditActionsColumnAttribute();
        Assert.Null(attribute.PropertyAttributes);
    }
}
