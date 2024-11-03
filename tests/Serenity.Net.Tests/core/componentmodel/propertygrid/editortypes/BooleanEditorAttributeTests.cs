namespace Serenity.Tests.ComponentModel;

public class BooleanEditorAttributeTests
{

    [Fact]
    public void BooleanEditorAttribute_IsAssignableFrom_CustomEditorAttribute()
    {
        var attribute = new BooleanEditorAttribute();
        Assert.IsAssignableFrom<CustomEditorAttribute>(attribute);
    }
}
