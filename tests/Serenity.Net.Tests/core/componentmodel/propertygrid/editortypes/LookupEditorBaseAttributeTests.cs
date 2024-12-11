namespace Serenity.ComponentModel;

public class LookupEditorBaseAttributeTests
{
    public class MyLookupEditorBaseAttribute : LookupEditorBaseAttribute
    {
        public MyLookupEditorBaseAttribute()
            : base("text")
        {
        }

        [Fact]
        public void LookupKey_CanBeSet()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                LookupKey = "lookupKey"
            };
            Assert.Equal("lookupKey", attribute.LookupKey);
        }

        [Fact]
        public void AutoComplete_CanBeSet_ToTrue()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                AutoComplete = true
            };
            Assert.True(attribute.AutoComplete);
        }

        [Fact]
        public void AutoComplete_CanBeSet_ToFalse()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                AutoComplete = false
            };
            Assert.False(attribute.AutoComplete);
        }

        [Fact]
        public void InplaceAddPermission_CanBeSet()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                InplaceAddPermission = "text"
            };
            Assert.Equal("text", attribute.InplaceAddPermission);
        }

        [Fact]
        public void InplaceAddPermission_CanBeSet_ToNull()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                InplaceAddPermission = null
            };
            Assert.Null(attribute.InplaceAddPermission);
        }

        [Fact]
        public void DialogType_CanBeSet()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                DialogType = "text"
            };
            Assert.Equal("text", attribute.DialogType);
        }

        [Fact]
        public void DialogType_CanBeSet_ToNull()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                DialogType = null
            };
            Assert.Null(attribute.DialogType);
        }

        [Fact]
        public void CascadeFrom_CanBeSet()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                CascadeFrom = "text"
            };
            Assert.Equal("text", attribute.CascadeFrom);
        }

        [Fact]
        public void CascadeFrom_CanBeSet_ToNull()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                CascadeFrom = null
            };
            Assert.Null(attribute.CascadeFrom);
        }

        [Fact]
        public void CascadeField_CanBeSet()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                CascadeField = "text"
            };
            Assert.Equal("text", attribute.CascadeField);
        }

        [Fact]
        public void CascadeField_CanBeSet_ToNull()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                CascadeField = null
            };
            Assert.Null(attribute.CascadeField);
        }

        [Fact]
        public void CascadeValue_CanBeSet()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                CascadeValue = "text"
            };
            Assert.Equal("text", attribute.CascadeValue);
        }

        [Fact]
        public void CascadeValue_CanBeSet_ToNull()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                CascadeValue = null
            };
            Assert.Null(attribute.CascadeValue);
        }

        [Fact]
        public void FilterField_CanBeSet()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                FilterField = "text"
            };
            Assert.Equal("text", attribute.FilterField);
        }

        [Fact]
        public void FilterField_CanBeSet_ToNull()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                FilterField = null
            };
            Assert.Null(attribute.FilterField);
        }

        [Fact]
        public void FilterValue_CanBeSet()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                FilterValue = "text"
            };
            Assert.Equal("text", attribute.FilterValue);
        }

        [Fact]
        public void FilterValue_CanBeSet_ToNull()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                FilterValue = null
            };
            Assert.Null(attribute.FilterValue);
        }

        [Fact]
        public void MinimumResultsForSearch_CanBe_ToInt()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                MinimumResultsForSearch = 2
            };
            Assert.Equal(2, attribute.MinimumResultsForSearch);
        }

        [Fact]
        public void Multiple_CanBeSet_ToTrue()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                Multiple = true
            };
            Assert.True(attribute.Multiple);
        }

        [Fact]
        public void Multiple_CanBeSet_ToFalse()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                Multiple = false
            };
            Assert.False(attribute.Multiple);
        }

        [Fact]
        public void OpenDialogAsPanel_CanBeSet_ToTrue()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                OpenDialogAsPanel = false
            };
            Assert.False(attribute.OpenDialogAsPanel);
        }

        [Fact]
        public void OpenDialogAsPanel_CanBeSet_ToFalse()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                OpenDialogAsPanel = false
            };
            Assert.False(attribute.OpenDialogAsPanel);
        }

        [Fact]
        public void Async_CanBeSet_ToTrue()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                Async = false
            };
            Assert.False(attribute.Async);
        }

        [Fact]
        public void Async_CanBeSet_ToFalse()
        {
            var attribute = new MyLookupEditorBaseAttribute()
            {
                Async = false
            };
            Assert.False(attribute.Async);
        }
    }
}