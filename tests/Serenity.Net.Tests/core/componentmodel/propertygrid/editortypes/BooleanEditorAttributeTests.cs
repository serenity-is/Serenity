namespace Serenity.ComponentModel;

public class BooleanEditorAttributeTests
{

    [Fact]
    public void BooleanEditorAttribute_IsAssignableFrom_CustomEditorAttribute()
    {
        var attribute = new BooleanEditorAttribute();
        Assert.IsAssignableFrom<CustomEditorAttribute>(attribute);
    }

    [Fact]
    public void BooleanEditorAttribute_LabelOnEnd_IsStoredAsOption()
    {
        var attribute = new BooleanEditorAttribute { LabelOnEnd = true };
        Assert.True(attribute.LabelOnEnd);
        Assert.False(new BooleanEditorAttribute().LabelOnEnd);
    }
}
