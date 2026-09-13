namespace Serenity.ComponentModel;

public class PropertyItemTests
{
    [Fact]
    public void Name_CanBeSet()
    {
        var item = new PropertyItem()
        {
            Name = "name"
        };
        Assert.Equal("name", item.Name);
    }

    [Fact]
    public void Name_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Name);
    }

    [Fact]
    public void Name_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Name = null
        };
        Assert.Null(item.Name);
    }

    [Fact]
    public void Title_CanBeSet()
    {
        var item = new PropertyItem()
        {
            Title = "title"
        };
        Assert.Equal("title", item.Title);
    }

    [Fact]
    public void Title_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Title);
    }

    [Fact]
    public void Title_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Title = null
        };
        Assert.Null(item.Title);
    }

    [Fact]
    public void Hint_CanBeSet()
    {
        var item = new PropertyItem()
        {
            Hint = "hint"
        };
        Assert.Equal("hint", item.Hint);
    }

    [Fact]
    public void Hint_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Hint);
    }

    [Fact]
    public void Hint_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Hint = null
        };
        Assert.Null(item.Hint);
    }

    [Fact]
    public void Placeholder_CanBeSet()
    {
        var item = new PropertyItem()
        {
            Placeholder = "text"
        };
        Assert.Equal("text", item.Placeholder);
    }

    [Fact]
    public void Placeholder_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Placeholder);
    }

    [Fact]
    public void Placeholder_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Placeholder = null
        };
        Assert.Null(item.Placeholder);
    }

    [Fact]
    public void EditorType_CanBeSet()
    {
        var item = new PropertyItem()
        {
            EditorType = "editorType"
        };
        Assert.Equal("editorType", item.EditorType);
    }

    [Fact]
    public void EditorType_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.EditorType);
    }

    [Fact]
    public void EditorType_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            EditorType = null
        };
        Assert.Null(item.EditorType);
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
        var item = new PropertyItem()
        {
            EditorCssClass = "sometext"
        };
        Assert.Equal("sometext", item.EditorCssClass);
    }

    [Fact]
    public void EditorCssClass_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.EditorCssClass);
    }

    [Fact]
    public void EditorCssClass_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            EditorCssClass = null
        };
        Assert.Null(item.EditorCssClass);
    }

    [Fact]
    public void Category_CanBeSet()
    {
        var item = new PropertyItem()
        {
            Category = "sometext"
        };
        Assert.Equal("sometext", item.Category);
    }

    [Fact]
    public void Category_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Category);
    }

    [Fact]
    public void Category_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Category = null
        };
        Assert.Null(item.Category);
    }

    [Fact]
    public void Collapsible_IsNull_ByDefault()
    {
        var item = new PropertyItem()
        {
            Collapsible = null
        };
        Assert.Null(item.Collapsible);
    }

    [Fact]
    public void Collapsible_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            Collapsible = true
        };
        Assert.True(item.Collapsible);
    }

    [Fact]
    public void Collapsible_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            Collapsed = false
        };
        Assert.False(item.Collapsed);
    }

    [Fact]
    public void Tab_CanBeSet()
    {
        var item = new PropertyItem()
        {
            Tab = "tab"
        };
        Assert.Equal("tab", item.Tab);
    }

    [Fact]
    public void Tab_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Tab = null
        };
        Assert.Null(item.Tab);
    }

    [Fact]
    public void Tab_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Tab);
    }

    [Fact]
    public void CssClass_CanBeSet()
    {
        var item = new PropertyItem()
        {
            CssClass = "sometext"
        };
        Assert.Equal("sometext", item.CssClass);
    }

    [Fact]
    public void CssClass_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            CssClass = null
        };
        Assert.Null(item.CssClass);
    }

    [Fact]
    public void CssClass_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.CssClass);
    }

    [Fact]
    public void HeaderCssClass_CanBeSet()
    {
        var item = new PropertyItem()
        {
            HeaderCssClass = "sometext"
        };
        Assert.Equal("sometext", item.HeaderCssClass);
    }

    [Fact]
    public void HeaderCssClass_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            HeaderCssClass = null
        };
        Assert.Null(item.HeaderCssClass);
    }

    [Fact]
    public void HeaderCssClass_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.HeaderCssClass);
    }

    [Fact]
    public void FormCssClass_CanBeSet()
    {
        var item = new PropertyItem()
        {
            FormCssClass = "sometext"
        };
        Assert.Equal("sometext", item.FormCssClass);
    }

    [Fact]
    public void FormCssClass_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            FormCssClass = null
        };
        Assert.Null(item.FormCssClass);
    }

    [Fact]
    public void FormCssClass_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.FormCssClass);
    }

    [Fact]
    public void MaxLenght_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.MaxLength);
    }

    [Fact]
    public void MaxLenght_CanBeSet_ToInt()
    {
        var item = new PropertyItem()
        {
            MaxLength = 100
        };
        Assert.Equal(100, item.MaxLength);
    }

    [Fact]
    public void MaxLenght_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            MaxLength = null
        };
        Assert.Null(item.MaxLength);
    }

    [Fact]
    public void Required_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Required);
    }

    [Fact]
    public void Required_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            Required = true
        };
        Assert.True(item.Required);
    }

    [Fact]
    public void Required_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            Required = false
        };
        Assert.False(item.Required);
    }

    [Fact]
    public void ReadOnly_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.ReadOnly);
    }

    [Fact]
    public void ReadOnly_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            ReadOnly = true
        };
        Assert.True(item.ReadOnly);
    }

    [Fact]
    public void ReadOnly_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            ReadOnly = false
        };
        Assert.False(item.ReadOnly);
    }

    [Fact]
    public void ReadPermission_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.ReadPermission);
    }

    [Fact]
    public void ReadPermission_CanBeSet()
    {
        var item = new PropertyItem()
        {
            ReadPermission = "text"
        };
        Assert.Equal("text", item.ReadPermission);
    }

    [Fact]
    public void ReadPermission_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            ReadPermission = null
        };
        Assert.Null(item.ReadPermission);
    }

    [Fact]
    public void Insertable_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Insertable);
    }

    [Fact]
    public void Insertable_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            Insertable = true
        };
        Assert.True(item.Insertable);
    }

    [Fact]
    public void Insertable_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            Insertable = false
        };
        Assert.False(item.Insertable);
    }

    [Fact]
    public void Insertable_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Insertable = null
        };
        Assert.Null(item.Insertable);
    }

    [Fact]
    public void InsertPermission_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.InsertPermission);
    }

    [Fact]
    public void InsertPermission_CanBeSet()
    {
        var item = new PropertyItem()
        {
            InsertPermission = "text"
        };
        Assert.Equal("text", item.InsertPermission);
    }

    [Fact]
    public void InsertPermission_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            InsertPermission = null
        };
        Assert.Null(item.InsertPermission);
    }
    [Fact]
    public void HideOnInsert_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.HideOnInsert);
    }

    [Fact]
    public void HideOnInsert_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            HideOnInsert = true
        };
        Assert.True(item.HideOnInsert);
    }

    [Fact]
    public void HideOnInsert_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            HideOnInsert = false
        };
        Assert.False(item.HideOnInsert);
    }

    [Fact]
    public void HideOnInsert_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            HideOnInsert = null
        };
        Assert.Null(item.HideOnInsert);
    }

    [Fact]
    public void Updatable_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Updatable);
    }

    [Fact]
    public void Updatable_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            Updatable = true
        };
        Assert.True(item.Updatable);
    }

    [Fact]
    public void Updatable_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            Updatable = false
        };
        Assert.False(item.Updatable);
    }

    [Fact]
    public void Updatable_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Updatable = null
        };
        Assert.Null(item.Updatable);
    }

    [Fact]
    public void UpdatePermission_IsNull_ByDefault()
    {
        var item = new PropertyItem()
        {
            UpdatePermission = null
        };
        Assert.Null(item.UpdatePermission);
    }

    [Fact]
    public void UpdatePermission_CanBeSet()
    {
        var item = new PropertyItem()
        {
            UpdatePermission = "text"
        };
        Assert.Equal("text", item.UpdatePermission);
    }

    [Fact]
    public void UpdatePermission_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            UpdatePermission = null
        };
        Assert.Null(item.UpdatePermission);
    }

    [Fact]
    public void HideOnUpdate_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.HideOnUpdate);
    }

    [Fact]
    public void HideOnUpdate_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            HideOnUpdate = true
        };
        Assert.True(item.HideOnUpdate);
    }

    [Fact]
    public void HideOnUpdate_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            HideOnUpdate = false
        };
        Assert.False(item.HideOnUpdate);
    }

    [Fact]
    public void HideOnUpdate_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            HideOnUpdate = null
        };
        Assert.Null(item.HideOnUpdate);
    }

    [Fact]
    public void SkipOnSave_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.SkipOnSave);
    }

    [Fact]
    public void SkipOnSave_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            SkipOnSave = true
        };
        Assert.True(item.SkipOnSave);
    }

    [Fact]
    public void Oneway_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            SkipOnSave = false
        };
        Assert.False(item.SkipOnSave);
    }

    [Fact]
    public void Oneway_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            SkipOnSave = null
        };
        Assert.Null(item.SkipOnSave);
    }

    [Fact]
    public void DefaultValue_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.DefaultValue);
    }

    [Fact]
    public void DefaultValue_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            DefaultValue = true
        };
        Assert.True((bool)item.DefaultValue);
    }

    [Fact]
    public void DefaultValue_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            DefaultValue = false
        };
        Assert.False((bool)item.DefaultValue);
    }

    [Fact]
    public void DefaultValue_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            DefaultValue = null
        };
        Assert.Null(item.DefaultValue);
    }

    [Fact]
    public void DefaultValue_CanBeSet()
    {
        var item = new PropertyItem()
        {
            DefaultValue = "text"
        };
        Assert.Equal("text", item.DefaultValue);
    }

    [Fact]
    public void DefaultValue_CanBeSet_ToInt()
    {
        var item = new PropertyItem()
        {
            DefaultValue = 2
        };
        Assert.Equal(2, item.DefaultValue);
    }

    [Fact]
    public void DefaultValue_CanBeSet_ToEnum()
    {
        var item = new PropertyItem()
        {
            DefaultValue = IO.DeleteType.Delete
        };
        Assert.Equal(IO.DeleteType.Delete, item.DefaultValue);
    }

    [Fact]
    public void Localizable_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Localizable);
    }

    [Fact]
    public void Localizable_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            Localizable = true
        };
        Assert.True((bool)item.Localizable);
    }

    [Fact]
    public void Localizable_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            Localizable = false
        };
        Assert.False((bool)item.Localizable);
    }

    [Fact]
    public void Localizable_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Localizable = null
        };
        Assert.Null(item.Localizable);
    }

    [Fact]
    public void Visible_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Visible);
    }

    [Fact]
    public void Visible_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            Visible = true
        };
        Assert.True((bool)item.Visible);
    }

    [Fact]
    public void Visible_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            Visible = false
        };
        Assert.False((bool)item.Visible);
    }

    [Fact]
    public void Visible_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Visible = null
        };
        Assert.Null(item.Visible);
    }

    [Fact]
    public void AllowHide_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.AllowHide);
    }

    [Fact]
    public void AllowHide_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            AllowHide = true
        };
        Assert.True((bool)item.AllowHide);
    }

    [Fact]
    public void AllowHide_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            AllowHide = false
        };
        Assert.False((bool)item.AllowHide);
    }

    [Fact]
    public void AllowHide_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            AllowHide = null
        };
        Assert.Null(item.AllowHide);
    }

    [Fact]
    public void FormatterType_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.FormatterType);
    }

    [Fact]
    public void FormatterType_CanBeSet()
    {
        var item = new PropertyItem()
        {
            FormatterType = "sometext"
        };
        Assert.Equal("sometext", item.FormatterType);
    }

    [Fact]
    public void FormatterType_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            FormatterType = null
        };
        Assert.Null(item.FormatterType);
    }

    [Fact]
    public void DisplayFormat_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet()
    {
        var item = new PropertyItem()
        {
            DisplayFormat = "sometext"
        };
        Assert.Equal("sometext", item.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            DisplayFormat = null
        };
        Assert.Null(item.DisplayFormat);
    }

    [Fact]
    public void Alignment_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Alignment);
    }

    [Fact]
    public void Alignment_CanBeSet()
    {
        var item = new PropertyItem()
        {
            Alignment = "sometext"
        };
        Assert.Equal("sometext", item.Alignment);
    }

    [Fact]
    public void Aligment_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Alignment = null
        };
        Assert.Null(item.Alignment);
    }

    [Fact]
    public void Width_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Width);
    }

    [Fact]
    public void Width_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Width = null
        };
        Assert.Null(item.Width);
    }

    [Fact]
    public void Width_CanBeSet_ToInt()
    {
        var item = new PropertyItem()
        {
            Width = 1
        };
        Assert.Equal(1, item.Width);
    }

    [Fact]
    public void WidthSet_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.WidthSet);
    }

    [Fact]
    public void WidthSet_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            WidthSet = true
        };
        Assert.True((bool)item.WidthSet);
    }

    [Fact]
    public void WidthSet_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            WidthSet = false
        };
        Assert.False((bool)item.WidthSet);
    }

    [Fact]
    public void WidthSet_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            WidthSet = null
        };
        Assert.Null(item.WidthSet);
    }

    [Fact]
    public void MinWidth_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.MinWidth);
    }

    [Fact]
    public void MinWidth_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            MinWidth = null
        };
        Assert.Null(item.MinWidth);
    }

    [Fact]
    public void MinWidth_CanBeSet_ToInt()
    {
        var item = new PropertyItem()
        {
            MinWidth = 1
        };
        Assert.Equal(1, item.MinWidth);
    }

    [Fact]
    public void MaxWidth_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.MaxWidth);
    }

    [Fact]
    public void MaxWidth_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            MaxWidth = null
        };
        Assert.Null(item.MaxWidth);
    }

    [Fact]
    public void MaxWidth_CanBeSet_ToInt()
    {
        var item = new PropertyItem()
        {
            MaxWidth = 1
        };
        Assert.Equal(1, item.MaxWidth);
    }

    [Fact]
    public void LabelWidth_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.LabelWidth);
    }

    [Fact]
    public void LabelWidth_CanBeSet()
    {
        var item = new PropertyItem()
        {
            LabelWidth = "sometext"
        };
        Assert.Equal("sometext", item.LabelWidth);
    }

    [Fact]
    public void LabelWidth_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            LabelWidth = null
        };
        Assert.Null(item.LabelWidth);
    }

    [Fact]
    public void Resizable_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Resizable);
    }

    [Fact]
    public void Resizable_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            Resizable = true
        };
        Assert.True((bool)item.Resizable);
    }

    [Fact]
    public void Resizable_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            Resizable = false
        };
        Assert.False((bool)item.Resizable);
    }

    [Fact]
    public void Resizable_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Resizable = null
        };
        Assert.Null(item.Resizable);
    }

    [Fact]
    public void Sortable_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.Sortable);
    }

    [Fact]
    public void Sortable_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            Sortable = true
        };
        Assert.True((bool)item.Sortable);
    }

    [Fact]
    public void Sortable_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            Sortable = false
        };
        Assert.False((bool)item.Sortable);
    }

    [Fact]
    public void Sortable_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            Sortable = null
        };
        Assert.Null(item.Sortable);
    }

    [Fact]
    public void SortOrder_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.SortOrder);
    }

    [Fact]
    public void SortOrder_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            SortOrder = null
        };
        Assert.Null(item.SortOrder);
    }

    [Fact]
    public void SortOrder_CanBeSet_ToInt()
    {
        var item = new PropertyItem()
        {
            SortOrder = 1
        };
        Assert.Equal(1, item.SortOrder);
    }

    [Fact]
    public void GroupOrder_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.GroupOrder);
    }

    [Fact]
    public void GroupOrder_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            GroupOrder = null
        };
        Assert.Null(item.GroupOrder);
    }

    [Fact]
    public void GroupOrder_CanBeSet_ToInt()
    {
        var item = new PropertyItem()
        {
            GroupOrder = 1
        };
        Assert.Equal(1, item.GroupOrder);
    }

    [Fact]
    public void SummaryType_CanBeSet_ToEnum()
    {
        var item = new PropertyItem()
        {
            SummaryType = SummaryType.None
        };
        Assert.Equal(SummaryType.None, item.SummaryType);
    }

    [Fact]
    public void SummaryType_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            SummaryType = null
        };
        Assert.Null(item.SummaryType);
    }

    [Fact]
    public void SummaryType_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.EditLink);
    }

    [Fact]
    public void EditLink_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            EditLink = true
        };
        Assert.True((bool)item.EditLink);
    }

    [Fact]
    public void EditLink_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            EditLink = false
        };
        Assert.False((bool)item.EditLink);
    }

    [Fact]
    public void EditLink_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.EditLink);
    }

    [Fact]
    public void EditLink_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            EditLink = null
        };
        Assert.Null(item.EditLink);
    }

    [Fact]
    public void EditLinkItemType_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.EditLinkItemType);
    }

    [Fact]
    public void EditLinkItemType_CanBeSet()
    {
        var item = new PropertyItem()
        {
            EditLinkItemType = "sometext"
        };
        Assert.Equal("sometext", item.EditLinkItemType);
    }

    [Fact]
    public void EditLinkItemType_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            EditLinkItemType = null
        };
        Assert.Null(item.EditLinkItemType);
    }

    [Fact]
    public void EditLinkIdField_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.EditLinkIdField);
    }

    [Fact]
    public void EditLinkIdField_CanBeSet()
    {
        var item = new PropertyItem()
        {
            EditLinkIdField = "sometext"
        };
        Assert.Equal("sometext", item.EditLinkIdField);
    }

    [Fact]
    public void EditLinkIdField_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            EditLinkIdField = null
        };
        Assert.Null(item.EditLinkIdField);
    }

    [Fact]
    public void EditLinkCssClass_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.EditLinkCssClass);
    }

    [Fact]
    public void EditLinkCssClass_CanBeSet()
    {
        var item = new PropertyItem()
        {
            EditLinkCssClass = "sometext"
        };
        Assert.Equal("sometext", item.EditLinkCssClass);
    }

    [Fact]
    public void EditLinkCssClass_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            FilteringType = null
        };
        Assert.Null(item.FilteringType);
    }

    [Fact]
    public void FilteringType_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.FilteringType);
    }

    [Fact]
    public void FilteringType_CanBeSet()
    {
        var item = new PropertyItem()
        {
            FilteringType = "sometext"
        };
        Assert.Equal("sometext", item.FilteringType);
    }

    [Fact]
    public void FilteringType_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            FilteringType = null
        };
        Assert.Null(item.FilteringType);
    }

    [Fact]
    public void FilteringIdField_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.FilteringIdField);
    }

    [Fact]
    public void FilteringIdField_CanBeSet()
    {
        var item = new PropertyItem()
        {
            FilteringIdField = "sometext"
        };
        Assert.Equal("sometext", item.FilteringIdField);
    }

    [Fact]
    public void FilteringIdField_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            FilteringIdField = null
        };
        Assert.Null(item.FilteringIdField);
    }

    [Fact]
    public void FilterOnly_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.FilterOnly);
    }

    [Fact]
    public void FilterOnly_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            FilterOnly = true
        };
        Assert.True((bool)item.FilterOnly);
    }

    [Fact]
    public void FilterOnly_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            FilterOnly = false
        };
        Assert.False((bool)item.FilterOnly);
    }

    [Fact]
    public void FilterOnly_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            FilterOnly = null
        };
        Assert.Null(item.FilterOnly);
    }

    [Fact]
    public void NotFilterable_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.NotFilterable);
    }

    [Fact]
    public void NotFilterable_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            NotFilterable = true
        };
        Assert.True((bool)item.NotFilterable);
    }

    [Fact]
    public void NotFilterable_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            NotFilterable = false
        };
        Assert.False((bool)item.NotFilterable);
    }

    [Fact]
    public void NotFilterable_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            NotFilterable = null
        };
        Assert.Null(item.NotFilterable);
    }

    [Fact]
    public void QuickFilter_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.QuickFilter);
    }

    [Fact]
    public void QuickFilter_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            QuickFilter = true
        };
        Assert.True((bool)item.QuickFilter);
    }

    [Fact]
    public void QuickFilter_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            QuickFilter = false
        };
        Assert.False((bool)item.QuickFilter);
    }

    [Fact]
    public void QuickFilter_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            QuickFilter = null
        };
        Assert.Null(item.QuickFilter);
    }

    [Fact]
    public void QuickFilterSeparator_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.QuickFilterSeparator);
    }

    [Fact]
    public void QuickFilterSeparator_CanBeSet_ToTrue()
    {
        var item = new PropertyItem()
        {
            QuickFilterSeparator = true
        };
        Assert.True((bool)item.QuickFilterSeparator);
    }

    [Fact]
    public void QuickFilterSeparator_CanBeSet_ToFalse()
    {
        var item = new PropertyItem()
        {
            QuickFilterSeparator = false
        };
        Assert.False((bool)item.QuickFilterSeparator);
    }

    [Fact]
    public void QuickFilterSeparator_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            QuickFilterSeparator = null
        };
        Assert.Null(item.QuickFilterSeparator);
    }

    [Fact]
    public void QuickFilterCssClass_IsNull_ByDefault()
    {
        var item = new PropertyItem();
        Assert.Null(item.QuickFilterCssClass);
    }

    [Fact]
    public void QuickFilterCssClass_CanBeSet()
    {
        var item = new PropertyItem()
        {
            QuickFilterCssClass = "sometext"
        };
        Assert.Equal("sometext", item.QuickFilterCssClass);
    }

    [Fact]
    public void QuickFilterCssClass_CanBeSet_ToNull()
    {
        var item = new PropertyItem()
        {
            QuickFilterCssClass = null
        };
        Assert.Null(item.QuickFilterCssClass);
    }

    [Fact]
    public void EditorType_NotNullAndNotString_ReturnsTrue()
    {
        var item = new PropertyItem
        {
            EditorType = "EditorType"
        };

        bool result = item.ShouldSerializeEditorType();
        Assert.True(result);
    }

    [Fact]
    public void EditorType_Null_ReturnsFalse()
    {
        var item = new PropertyItem
        {
            EditorType = null
        };

        bool result = item.ShouldSerializeEditorType();
        Assert.False(result);
    }

    [Fact]
    public void EditorType_String_ReturnsFalse()
    {
        var item = new PropertyItem
        {
            EditorType = "String"
        };

        bool result = item.ShouldSerializeEditorType();
        Assert.False(result);
    }

    [Fact]
    public void FilteringType_NotNullAndNotString_ReturnsTrue()
    {
        var item = new PropertyItem()
        {
            FilteringType = "FilteringType"
        };
        bool result = item.ShouldSerializeFilteringType();
        Assert.True(result);
    }

    [Fact]
    public void FilteringType_IsNull_Return_False()
    {
        var item = new PropertyItem()
        {
            FilteringType = null
        };
        bool result = item.ShouldSerializeFilteringType();
        Assert.False(result);
    }

    [Fact]
    public void FilteringType_String_ReturnFalse()
    {
        var item = new PropertyItem()
        {
            FilteringType = "String"
        };
        bool result = item.ShouldSerializeFilteringType();
        Assert.False(result);
    }

    [Fact]
    public void ShouldSerializeEditorParams_WithNonEmptyDictionary_ReturnsTrue()
    {
        var item = new PropertyItem
        {
            EditorParams = new Dictionary<string, object>
            {
                { "key", "value" }
            }
        };

        bool result = item.ShouldSerializeEditorParams();
        Assert.True(result);
    }

    [Fact]
    public void ShouldSerializeEditorParams_WithNullDictionary_ReturnsFalse()
    {
        var item = new PropertyItem
        {
            EditorParams = null
        };

        bool result = item.ShouldSerializeEditorParams();
        Assert.False(result);
    }

    [Fact]
    public void ShouldSerializeEditorParams_WithEmptyDictionary_ReturnsFalse()
    {
        var item = new PropertyItem
        {
            EditorParams = []
        };

        bool result = item.ShouldSerializeEditorParams();
        Assert.False(result);
    }

    [Fact]
    public void FormatterParams_WithNonEmptyDictionary_ReturnsTrue()
    {
        var item = new PropertyItem
        {
            FormatterParams = new Dictionary<string, object>
            {
                { "key", "value" }
            }
        };

        bool result = item.ShouldSerializeFormatterParams();
        Assert.True(result);
    }

    [Fact]
    public void FormatterParams_WithNullDictionary_ReturnsFalse()
    {
        var item = new PropertyItem
        {
            FormatterParams = null
        };

        bool result = item.ShouldSerializeFormatterParams();
        Assert.False(result);
    }

    [Fact]
    public void FormatterParams_WithEmptyDictionary_ReturnsFalse()
    {
        var item = new PropertyItem
        {
            FormatterParams = []
        };

        bool result = item.ShouldSerializeFormatterParams();
        Assert.False(result);
    }

    [Fact]
    public void FilteringParams_WithNonEmptyDictionary_ReturnsTrue()
    {
        var item = new PropertyItem
        {
            FilteringParams = new Dictionary<string, object>
            {
                { "key", "value" }
            }
        };

        bool result = item.ShouldSerializeFilteringParams();
        Assert.True(result);
    }

    [Fact]
    public void FilteringParams_WithNullDictionary_ReturnsFalse()
    {
        var item = new PropertyItem
        {
            FilteringParams = null
        };

        bool result = item.ShouldSerializeFilteringParams();
        Assert.False(result);
    }

    [Fact]
    public void FilteringParams_WithEmptyDictionary_ReturnsFalse()
    {
        var item = new PropertyItem
        {
            FilteringParams = []
        };

        bool result = item.ShouldSerializeFilteringParams();
        Assert.False(result);
    }

    [Fact]
    public void QuickFilterParams_WithNonEmptyDictionary_ReturnsTrue()
    {
        var item = new PropertyItem
        {
            QuickFilterParams = new Dictionary<string, object>
            {
                { "key", "value" }
            }
        };

        bool result = item.ShouldSerializeQuickFilterParams();
        Assert.True(result);
    }

    [Fact]
    public void QuickFilterParams_WithNullDictionary_ReturnsFalse()
    {
        var item = new PropertyItem
        {
            QuickFilterParams = null
        };

        bool result = item.ShouldSerializeQuickFilterParams();
        Assert.False(result);
    }

    [Fact]
    public void QuickFilterParams_WithEmptyDictionary_ReturnsFalse()
    {
        var item = new PropertyItem
        {
            QuickFilterParams = []
        };

        bool result = item.ShouldSerializeQuickFilterParams();
        Assert.False(result);
    }
}