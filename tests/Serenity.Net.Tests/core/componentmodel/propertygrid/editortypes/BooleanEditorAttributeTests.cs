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
    public void BooleanEditorAttribute_LabelPosition_IsStoredAsOption()
    {
        var attribute = new BooleanEditorAttribute { LabelPosition = CheckLabelPosition.EndAligned };
        Assert.Equal(CheckLabelPosition.EndAligned, attribute.LabelPosition);
        var editorParams = new Dictionary<string, object>();
        attribute.SetParams(editorParams);
        Assert.Equal(CheckLabelPosition.EndAligned, editorParams["labelPosition"]);
        Assert.Equal(CheckLabelPosition.Start, new BooleanEditorAttribute().LabelPosition);
    }
}
