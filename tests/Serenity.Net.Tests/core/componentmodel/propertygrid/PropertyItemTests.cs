namespace Serenity.ComponentModel;

public class PropertyItemTests
{
    [Fact]
    public void Name_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            Name = "name"
        };
        Assert.Equal("name", attribute.Name);
    }

    [Fact]
    public void Name_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Name);
    }

    [Fact]
    public void Name_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Name = null
        };
        Assert.Null(attribute.Name);
    }

    [Fact]
    public void Title_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            Title = "title"
        };
        Assert.Equal("title", attribute.Title);
    }

    [Fact]
    public void Title_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Title);
    }

    [Fact]
    public void Title_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Title = null
        };
        Assert.Null(attribute.Title);
    }

    [Fact]
    public void Hint_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            Hint = "hint"
        };
        Assert.Equal("hint", attribute.Hint);
    }

    [Fact]
    public void Hint_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Hint);
    }

    [Fact]
    public void Hint_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Hint = null
        };
        Assert.Null(attribute.Hint);
    }

    [Fact]
    public void Placeholder_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            Placeholder = "text"
        };
        Assert.Equal("text", attribute.Placeholder);
    }

    [Fact]
    public void Placeholder_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Placeholder);
    }

    [Fact]
    public void Placeholder_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Placeholder = null
        };
        Assert.Null(attribute.Placeholder);
    }

    [Fact]
    public void EditorType_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            EditorType = "editorType"
        };
        Assert.Equal("editorType", attribute.EditorType);
    }

    [Fact]
    public void EditorType_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.EditorType);
    }

    [Fact]
    public void EditorType_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            EditorType = null
        };
        Assert.Null(attribute.EditorType);
    }
    [Fact]
    public void EditorAddons_ShouldBeInitializedCorrectly()
    {
        var editorAddons = new List<EditorAddonItem>
        {
            new() { },
            new() { }
        };
        var data = new PropertyItem
        {
            EditorAddons = editorAddons
        };

        Assert.Equal(editorAddons, data.EditorAddons);
    }

    [Fact]
    public void EditorAddons_IsNull_ByDefault()
    {
        var data = new PropertyItem();
        Assert.Null(data.EditorAddons);
    }

    [Fact]
    public void EditorCssClass_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            EditorCssClass = "sometext"
        };
        Assert.Equal("sometext", attribute.EditorCssClass);
    }

    [Fact]
    public void EditorCssClass_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.EditorCssClass);
    }

    [Fact]
    public void EditorCssClass_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            EditorCssClass = null
        };
        Assert.Null(attribute.EditorCssClass);
    }

    [Fact]
    public void Category_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            Category = "sometext"
        };
        Assert.Equal("sometext", attribute.Category);
    }

    [Fact]
    public void Category_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Category);
    }

    [Fact]
    public void Category_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Category = null
        };
        Assert.Null(attribute.Category);
    }

    [Fact]
    public void Collapsible_IsNull_ByDefault()
    {
        var attribute = new PropertyItem()
        {
            Collapsible = null
        };
        Assert.Null(attribute.Collapsible);
    }

    [Fact]
    public void Collapsible_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            Collapsible = true
        };
        Assert.True(attribute.Collapsible);
    }

    [Fact]
    public void Collapsible_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            Collapsed = false
        };
        Assert.False(attribute.Collapsed);
    }

    [Fact]
    public void Tab_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            Tab = "tab"
        };
        Assert.Equal("tab", attribute.Tab);
    }

    [Fact]
    public void Tab_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Tab = null
        };
        Assert.Null(attribute.Tab);
    }

    [Fact]
    public void Tab_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Tab);
    }

    [Fact]
    public void CssClass_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            CssClass = "sometext"
        };
        Assert.Equal("sometext", attribute.CssClass);
    }

    [Fact]
    public void CssClass_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            CssClass = null
        };
        Assert.Null(attribute.CssClass);
    }

    [Fact]
    public void CssClass_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.CssClass);
    }

    [Fact]
    public void HeaderCssClass_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            HeaderCssClass = "sometext"
        };
        Assert.Equal("sometext", attribute.HeaderCssClass);
    }

    [Fact]
    public void HeaderCssClass_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            HeaderCssClass = null
        };
        Assert.Null(attribute.HeaderCssClass);
    }

    [Fact]
    public void HeaderCssClass_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.HeaderCssClass);
    }

    [Fact]
    public void FormCssClass_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            FormCssClass = "sometext"
        };
        Assert.Equal("sometext", attribute.FormCssClass);
    }

    [Fact]
    public void FormCssClass_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            FormCssClass = null
        };
        Assert.Null(attribute.FormCssClass);
    }

    [Fact]
    public void FormCssClass_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.FormCssClass);
    }

    [Fact]
    public void MaxLenght_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.MaxLength);
    }

    [Fact]
    public void MaxLenght_CanBeSet_ToInt()
    {
        var attribute = new PropertyItem()
        {
            MaxLength = 100
        };
        Assert.Equal(100, attribute.MaxLength);
    }

    [Fact]
    public void MaxLenght_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            MaxLength = null
        };
        Assert.Null(attribute.MaxLength);
    }

    [Fact]
    public void Required_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Required);
    }

    [Fact]
    public void Required_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            Required = true
        };
        Assert.True(attribute.Required);
    }

    [Fact]
    public void Required_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            Required = false
        };
        Assert.False(attribute.Required);
    }

    [Fact]
    public void ReadOnly_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.ReadOnly);
    }

    [Fact]
    public void ReadOnly_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            ReadOnly = true
        };
        Assert.True(attribute.ReadOnly);
    }

    [Fact]
    public void ReadOnly_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            ReadOnly = false
        };
        Assert.False(attribute.ReadOnly);
    }

    [Fact]
    public void ReadPermission_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.ReadPermission);
    }

    [Fact]
    public void ReadPermission_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            ReadPermission = "text"
        };
        Assert.Equal("text", attribute.ReadPermission);
    }

    [Fact]
    public void ReadPermission_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            ReadPermission = null
        };
        Assert.Null(attribute.ReadPermission);
    }

    [Fact]
    public void Insertable_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Insertable);
    }

    [Fact]
    public void Insertable_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            Insertable = true
        };
        Assert.True(attribute.Insertable);
    }

    [Fact]
    public void Insertable_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            Insertable = false
        };
        Assert.False(attribute.Insertable);
    }

    [Fact]
    public void Insertable_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Insertable = null
        };
        Assert.Null(attribute.Insertable);
    }

    [Fact]
    public void InsertPermission_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.InsertPermission);
    }

    [Fact]
    public void InsertPermission_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            InsertPermission = "text"
        };
        Assert.Equal("text", attribute.InsertPermission);
    }

    [Fact]
    public void InsertPermission_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            InsertPermission = null
        };
        Assert.Null(attribute.InsertPermission);
    }
    [Fact]
    public void HideOnInsert_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.HideOnInsert);
    }

    [Fact]
    public void HideOnInsert_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            HideOnInsert = true
        };
        Assert.True(attribute.HideOnInsert);
    }

    [Fact]
    public void HideOnInsert_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            HideOnInsert = false
        };
        Assert.False(attribute.HideOnInsert);
    }

    [Fact]
    public void HideOnInsert_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            HideOnInsert = null
        };
        Assert.Null(attribute.HideOnInsert);
    }

    [Fact]
    public void Updatable_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Updatable);
    }

    [Fact]
    public void Updatable_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            Updatable = true
        };
        Assert.True(attribute.Updatable);
    }

    [Fact]
    public void Updatable_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            Updatable = false
        };
        Assert.False(attribute.Updatable);
    }

    [Fact]
    public void Updatable_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Updatable = null
        };
        Assert.Null(attribute.Updatable);
    }

    [Fact]
    public void UpdatePermission_IsNull_ByDefault()
    {
        var attribute = new PropertyItem()
        {
            UpdatePermission = null
        };
        Assert.Null(attribute.UpdatePermission);
    }

    [Fact]
    public void UpdatePermission_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            UpdatePermission = "text"
        };
        Assert.Equal("text", attribute.UpdatePermission);
    }

    [Fact]
    public void UpdatePermission_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            UpdatePermission = null
        };
        Assert.Null(attribute.UpdatePermission);
    }

    [Fact]
    public void HideOnUpdate_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.HideOnUpdate);
    }

    [Fact]
    public void HideOnUpdate_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            HideOnUpdate = true
        };
        Assert.True(attribute.HideOnUpdate);
    }

    [Fact]
    public void HideOnUpdate_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            HideOnUpdate = false
        };
        Assert.False(attribute.HideOnUpdate);
    }

    [Fact]
    public void HideOnUpdate_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            HideOnUpdate = null
        };
        Assert.Null(attribute.HideOnUpdate);
    }

    [Fact]
    public void OneWay_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.OneWay);
    }

    [Fact]
    public void OneWay_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            OneWay = true
        };
        Assert.True(attribute.OneWay);
    }

    [Fact]
    public void Oneway_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            OneWay = false
        };
        Assert.False(attribute.OneWay);
    }

    [Fact]
    public void Oneway_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            OneWay = null
        };
        Assert.Null(attribute.OneWay);
    }

    [Fact]
    public void DefaultValue_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.DefaultValue);
    }

    [Fact]
    public void DefaultValue_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            DefaultValue = true
        };
        Assert.True((bool)attribute.DefaultValue);
    }

    [Fact]
    public void DefaultValue_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            DefaultValue = false
        };
        Assert.False((bool)attribute.DefaultValue);
    }

    [Fact]
    public void DefaultValue_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            DefaultValue = null
        };
        Assert.Null(attribute.DefaultValue);
    }

    [Fact]
    public void DefaultValue_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            DefaultValue = "text"
        };
        Assert.Equal("text", attribute.DefaultValue);
    }

    [Fact]
    public void DefaultValue_CanBeSet_ToInt()
    {
        var attribute = new PropertyItem()
        {
            DefaultValue = 2
        };
        Assert.Equal(2, attribute.DefaultValue);
    }

    [Fact]
    public void DefaultValue_CanBeSet_ToEnum()
    {
        var attribute = new PropertyItem()
        {
            DefaultValue = Serenity.IO.DeleteType.Delete
        };
        Assert.Equal(Serenity.IO.DeleteType.Delete, attribute.DefaultValue);
    }

    [Fact]
    public void Localizable_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Localizable);
    }

    [Fact]
    public void Localizable_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            Localizable = true
        };
        Assert.True((bool)attribute.Localizable);
    }

    [Fact]
    public void Localizable_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            Localizable = false
        };
        Assert.False((bool)attribute.Localizable);
    }

    [Fact]
    public void Localizable_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Localizable = null
        };
        Assert.Null(attribute.Localizable);
    }

    [Fact]
    public void Visible_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Visible);
    }

    [Fact]
    public void Visible_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            Visible = true
        };
        Assert.True((bool)attribute.Visible);
    }

    [Fact]
    public void Visible_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            Visible = false
        };
        Assert.False((bool)attribute.Visible);
    }

    [Fact]
    public void Visible_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Visible = null
        };
        Assert.Null(attribute.Visible);
    }

    [Fact]
    public void AllowHide_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.AllowHide);
    }

    [Fact]
    public void AllowHide_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            AllowHide = true
        };
        Assert.True((bool)attribute.AllowHide);
    }

    [Fact]
    public void AllowHide_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            AllowHide = false
        };
        Assert.False((bool)attribute.AllowHide);
    }

    [Fact]
    public void AllowHide_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            AllowHide = null
        };
        Assert.Null(attribute.AllowHide);
    }

    [Fact]
    public void FormatterType_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.FormatterType);
    }

    [Fact]
    public void FormatterType_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            FormatterType = "sometext"
        };
        Assert.Equal("sometext", attribute.FormatterType);
    }

    [Fact]
    public void FormatterType_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            FormatterType = null
        };
        Assert.Null(attribute.FormatterType);
    }

    [Fact]
    public void DisplayFormat_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            DisplayFormat = "sometext"
        };
        Assert.Equal("sometext", attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            DisplayFormat = null
        };
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void Alignment_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Alignment);
    }

    [Fact]
    public void Alignment_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            Alignment = "sometext"
        };
        Assert.Equal("sometext", attribute.Alignment);
    }

    [Fact]
    public void Aligment_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Alignment = null
        };
        Assert.Null(attribute.Alignment);
    }

    [Fact]
    public void Width_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Width);
    }

    [Fact]
    public void Width_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Width = null
        };
        Assert.Null(attribute.Width);
    }

    [Fact]
    public void Width_CanBeSet_ToInt()
    {
        var attribute = new PropertyItem()
        {
            Width = 1
        };
        Assert.Equal(1, attribute.Width);
    }

    [Fact]
    public void WidthSet_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.WidthSet);
    }

    [Fact]
    public void WidthSet_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            WidthSet = true
        };
        Assert.True((bool)attribute.WidthSet);
    }

    [Fact]
    public void WidthSet_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            WidthSet = false
        };
        Assert.False((bool)attribute.WidthSet);
    }

    [Fact]
    public void WidthSet_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            WidthSet = null
        };
        Assert.Null(attribute.WidthSet);
    }

    [Fact]
    public void MinWidth_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.MinWidth);
    }

    [Fact]
    public void MinWidth_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            MinWidth = null
        };
        Assert.Null(attribute.MinWidth);
    }

    [Fact]
    public void MinWidth_CanBeSet_ToInt()
    {
        var attribute = new PropertyItem()
        {
            MinWidth = 1
        };
        Assert.Equal(1, attribute.MinWidth);
    }

    [Fact]
    public void MaxWidth_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.MaxWidth);
    }

    [Fact]
    public void MaxWidth_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            MaxWidth = null
        };
        Assert.Null(attribute.MaxWidth);
    }

    [Fact]
    public void MaxWidth_CanBeSet_ToInt()
    {
        var attribute = new PropertyItem()
        {
            MaxWidth = 1
        };
        Assert.Equal(1, attribute.MaxWidth);
    }

    [Fact]
    public void LabelWidth_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.LabelWidth);
    }

    [Fact]
    public void LabelWidth_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            LabelWidth = "sometext"
        };
        Assert.Equal("sometext", attribute.LabelWidth);
    }

    [Fact]
    public void LabelWidth_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            LabelWidth = null
        };
        Assert.Null(attribute.LabelWidth);
    }

    [Fact]
    public void Resizable_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Resizable);
    }

    [Fact]
    public void Resizable_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            Resizable = true
        };
        Assert.True((bool)attribute.Resizable);
    }

    [Fact]
    public void Resizable_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            Resizable = false
        };
        Assert.False((bool)attribute.Resizable);
    }

    [Fact]
    public void Resizable_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Resizable = null
        };
        Assert.Null(attribute.Resizable);
    }

    [Fact]
    public void Sortable_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.Sortable);
    }

    [Fact]
    public void Sortable_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            Sortable = true
        };
        Assert.True((bool)attribute.Sortable);
    }

    [Fact]
    public void Sortable_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            Sortable = false
        };
        Assert.False((bool)attribute.Sortable);
    }

    [Fact]
    public void Sortable_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            Sortable = null
        };
        Assert.Null(attribute.Sortable);
    }

    [Fact]
    public void SortOrder_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.SortOrder);
    }

    [Fact]
    public void SortOrder_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            SortOrder = null
        };
        Assert.Null(attribute.SortOrder);
    }

    [Fact]
    public void SortOrder_CanBeSet_ToInt()
    {
        var attribute = new PropertyItem()
        {
            SortOrder = 1
        };
        Assert.Equal(1, attribute.SortOrder);
    }

    [Fact]
    public void GroupOrder_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.GroupOrder);
    }

    [Fact]
    public void GroupOrder_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            GroupOrder = null
        };
        Assert.Null(attribute.GroupOrder);
    }

    [Fact]
    public void GroupOrder_CanBeSet_ToInt()
    {
        var attribute = new PropertyItem()
        {
            GroupOrder = 1
        };
        Assert.Equal(1, attribute.GroupOrder);
    }

    [Fact]
    public void SummaryType_CanBeSet_ToEnum()
    {
        var attribute = new PropertyItem()
        {
            SummaryType = Serenity.SummaryType.None
        };
        Assert.Equal(Serenity.SummaryType.None, attribute.SummaryType);
    }

    [Fact]
    public void SummaryType_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            SummaryType = null
        };
        Assert.Null(attribute.SummaryType);
    }

    [Fact]
    public void SummaryType_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.EditLink);
    }

    [Fact]
    public void EditLink_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            EditLink = true
        };
        Assert.True((bool)attribute.EditLink);
    }

    [Fact]
    public void EditLink_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            EditLink = false
        };
        Assert.False((bool)attribute.EditLink);
    }

    [Fact]
    public void EditLink_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.EditLink);
    }

    [Fact]
    public void EditLink_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            EditLink = null
        };
        Assert.Null(attribute.EditLink);
    }

    [Fact]
    public void EditLinkItemType_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.EditLinkItemType);
    }

    [Fact]
    public void EditLinkItemType_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            EditLinkItemType = "sometext"
        };
        Assert.Equal("sometext", attribute.EditLinkItemType);
    }

    [Fact]
    public void EditLinkItemType_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            EditLinkItemType = null
        };
        Assert.Null(attribute.EditLinkItemType);
    }

    [Fact]
    public void EditLinkIdField_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.EditLinkIdField);
    }

    [Fact]
    public void EditLinkIdField_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            EditLinkIdField = "sometext"
        };
        Assert.Equal("sometext", attribute.EditLinkIdField);
    }

    [Fact]
    public void EditLinkIdField_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            EditLinkIdField = null
        };
        Assert.Null(attribute.EditLinkIdField);
    }

    [Fact]
    public void EditLinkCssClass_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.EditLinkCssClass);
    }

    [Fact]
    public void EditLinkCssClass_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            EditLinkCssClass = "sometext"
        };
        Assert.Equal("sometext", attribute.EditLinkCssClass);
    }

    [Fact]
    public void EditLinkCssClass_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            FilteringType = null
        };
        Assert.Null(attribute.FilteringType);
    }

    [Fact]
    public void FilteringType_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.FilteringType);
    }

    [Fact]
    public void FilteringType_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            FilteringType = "sometext"
        };
        Assert.Equal("sometext", attribute.FilteringType);
    }

    [Fact]
    public void FilteringType_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            FilteringType = null
        };
        Assert.Null(attribute.FilteringType);
    }

    [Fact]
    public void FilteringIdField_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.FilteringIdField);
    }

    [Fact]
    public void FilteringIdField_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            FilteringIdField = "sometext"
        };
        Assert.Equal("sometext", attribute.FilteringIdField);
    }

    [Fact]
    public void FilteringIdField_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            FilteringIdField = null
        };
        Assert.Null(attribute.FilteringIdField);
    }

    [Fact]
    public void FilterOnly_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.FilterOnly);
    }

    [Fact]
    public void FilterOnly_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            FilterOnly = true
        };
        Assert.True((bool)attribute.FilterOnly);
    }

    [Fact]
    public void FilterOnly_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            FilterOnly = false
        };
        Assert.False((bool)attribute.FilterOnly);
    }

    [Fact]
    public void FilterOnly_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            FilterOnly = null
        };
        Assert.Null(attribute.FilterOnly);
    }

    [Fact]
    public void NotFilterable_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.NotFilterable);
    }

    [Fact]
    public void NotFilterable_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            NotFilterable = true
        };
        Assert.True((bool)attribute.NotFilterable);
    }

    [Fact]
    public void NotFilterable_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            NotFilterable = false
        };
        Assert.False((bool)attribute.NotFilterable);
    }

    [Fact]
    public void NotFilterable_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            NotFilterable = null
        };
        Assert.Null(attribute.NotFilterable);
    }

    [Fact]
    public void QuickFilter_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.QuickFilter);
    }

    [Fact]
    public void QuickFilter_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            QuickFilter = true
        };
        Assert.True((bool)attribute.QuickFilter);
    }

    [Fact]
    public void QuickFilter_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            QuickFilter = false
        };
        Assert.False((bool)attribute.QuickFilter);
    }

    [Fact]
    public void QuickFilter_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            QuickFilter = null
        };
        Assert.Null(attribute.QuickFilter);
    }

    [Fact]
    public void QuickFilterSeparator_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.QuickFilterSeparator);
    }

    [Fact]
    public void QuickFilterSeparator_CanBeSet_ToTrue()
    {
        var attribute = new PropertyItem()
        {
            QuickFilterSeparator = true
        };
        Assert.True((bool)attribute.QuickFilterSeparator);
    }

    [Fact]
    public void QuickFilterSeparator_CanBeSet_ToFalse()
    {
        var attribute = new PropertyItem()
        {
            QuickFilterSeparator = false
        };
        Assert.False((bool)attribute.QuickFilterSeparator);
    }

    [Fact]
    public void QuickFilterSeparator_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            QuickFilterSeparator = null
        };
        Assert.Null(attribute.QuickFilterSeparator);
    }

    [Fact]
    public void QuickFilterCssClass_IsNull_ByDefault()
    {
        var attribute = new PropertyItem();
        Assert.Null(attribute.QuickFilterCssClass);
    }

    [Fact]
    public void QuickFilterCssClass_CanBeSet()
    {
        var attribute = new PropertyItem()
        {
            QuickFilterCssClass = "sometext"
        };
        Assert.Equal("sometext", attribute.QuickFilterCssClass);
    }

    [Fact]
    public void QuickFilterCssClass_CanBeSet_ToNull()
    {
        var attribute = new PropertyItem()
        {
            QuickFilterCssClass = null
        };
        Assert.Null(attribute.QuickFilterCssClass);
    }

    [Fact]
    public void EditorType_NotNullAndNotString_ReturnsTrue()
    {
        var attribute = new PropertyItem
        {
            EditorType = "EditorType"
        };

        bool result = attribute.ShouldSerializeEditorType();
        Assert.True(result);
    }

    [Fact]
    public void EditorType_Null_ReturnsFalse()
    {
        var attribute = new PropertyItem
        {
            EditorType = null
        };

        bool result = attribute.ShouldSerializeEditorType();
        Assert.False(result);
    }

    [Fact]
    public void EditorType_String_ReturnsFalse()
    {
        var attribute = new PropertyItem
        {
            EditorType = "String"
        };

        bool result = attribute.ShouldSerializeEditorType();
        Assert.False(result);
    }

    [Fact]
    public void FilteringType_NotNullAndNotString_ReturnsTrue()
    {
        var attribute = new PropertyItem()
        {
            FilteringType = "FilteringType"
        };
        bool result = attribute.ShouldSerializeFilteringType();
        Assert.True(result);
    }

    [Fact]
    public void FilteringType_IsNull_Return_False()
    {
        var attribute = new PropertyItem()
        {
            FilteringType = null
        };
        bool result = attribute.ShouldSerializeFilteringType();
        Assert.False(result);
    }

    [Fact]
    public void FilteringType_String_ReturnFalse()
    {
        var attribute = new PropertyItem()
        {
            FilteringType = "String"
        };
        bool result = attribute.ShouldSerializeFilteringType();
        Assert.False(result);
    }

    [Fact]
    public void ShouldSerializeEditorParams_WithNonEmptyDictionary_ReturnsTrue()
    {
        var attribute = new PropertyItem
        {
            EditorParams = new Dictionary<string, object>
            {
                { "key", "value" }
            }
        };

        bool result = attribute.ShouldSerializeEditorParams();
        Assert.True(result);
    }

    [Fact]
    public void ShouldSerializeEditorParams_WithNullDictionary_ReturnsFalse()
    {
        var attribute = new PropertyItem
        {
            EditorParams = null
        };

        bool result = attribute.ShouldSerializeEditorParams();
        Assert.False(result);
    }

    [Fact]
    public void ShouldSerializeEditorParams_WithEmptyDictionary_ReturnsFalse()
    {
        var attribute = new PropertyItem
        {
            EditorParams = new Dictionary<string, object>()
        };

        bool result = attribute.ShouldSerializeEditorParams();
        Assert.False(result);
    }

    [Fact]
    public void FormatterParams_WithNonEmptyDictionary_ReturnsTrue()
    {
        var attribute = new PropertyItem
        {
            FormatterParams = new Dictionary<string, object>
            {
                { "key", "value" }
            }
        };

        bool result = attribute.ShouldSerializeFormatterParams();
        Assert.True(result);
    }

    [Fact]
    public void FormatterParams_WithNullDictionary_ReturnsFalse()
    {
        var attribute = new PropertyItem
        {
            FormatterParams = null
        };

        bool result = attribute.ShouldSerializeFormatterParams();
        Assert.False(result);
    }

    [Fact]
    public void FormatterParams_WithEmptyDictionary_ReturnsFalse()
    {
        var attribute = new PropertyItem
        {
            FormatterParams = new Dictionary<string, object>()
        };

        bool result = attribute.ShouldSerializeFormatterParams();
        Assert.False(result);
    }

    [Fact]
    public void FilteringParams_WithNonEmptyDictionary_ReturnsTrue()
    {
        var attribute = new PropertyItem
        {
            FilteringParams = new Dictionary<string, object>
            {
                { "key", "value" }
            }
        };

        bool result = attribute.ShouldSerializeFilteringParams();
        Assert.True(result);
    }

    [Fact]
    public void FilteringParams_WithNullDictionary_ReturnsFalse()
    {
        var attribute = new PropertyItem
        {
            FilteringParams = null
        };

        bool result = attribute.ShouldSerializeFilteringParams();
        Assert.False(result);
    }

    [Fact]
    public void FilteringParams_WithEmptyDictionary_ReturnsFalse()
    {
        var attribute = new PropertyItem
        {
            FilteringParams = new Dictionary<string, object>()
        };

        bool result = attribute.ShouldSerializeFilteringParams();
        Assert.False(result);
    }

    [Fact]
    public void QuickFilterParams_WithNonEmptyDictionary_ReturnsTrue()
    {
        var attribute = new PropertyItem
        {
            QuickFilterParams = new Dictionary<string, object>
            {
                { "key", "value" }
            }
        };

        bool result = attribute.ShouldSerializeQuickFilterParams();
        Assert.True(result);
    }

    [Fact]
    public void QuickFilterParams_WithNullDictionary_ReturnsFalse()
    {
        var attribute = new PropertyItem
        {
            QuickFilterParams = null
        };

        bool result = attribute.ShouldSerializeQuickFilterParams();
        Assert.False(result);
    }

    [Fact]
    public void QuickFilterParams_WithEmptyDictionary_ReturnsFalse()
    {
        var attribute = new PropertyItem
        {
            QuickFilterParams = new Dictionary<string, object>()
        };

        bool result = attribute.ShouldSerializeQuickFilterParams();
        Assert.False(result);
    }
}