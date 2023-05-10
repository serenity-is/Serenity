
namespace Serenity.Web;

public static partial class WebTexts
{
    public static partial class Controls
    {
        public static class CheckTreeEditor
        {
            public static readonly LocalText SelectAll = "Select All";
        }

        public static class ColumnPickerDialog
        {
            public static readonly LocalText HiddenColumns = "Hidden Columns";
            public static readonly LocalText HideHint = "hide";
            public static readonly LocalText RestoreDefaults = "Restore Defaults";
            public static readonly LocalText ShowHint = "show";
            public static readonly LocalText Title = "Column Picker";
            public static readonly LocalText VisibleColumns = "Visible Columns";
        }

        public static class DateTimeEditor
        {
            public static readonly LocalText SetToNow = "set to now";
        }

        public static class DataGrid
        {
            public static readonly LocalText NewButton = "New";
        }

        public static class EntityDialog
        {
            public static readonly LocalText DeleteConfirmation = "Delete record?";
            public static readonly LocalText UndeleteButton = "Undelete";
            public static readonly LocalText UndeleteConfirmation = "Undelete record?";
            public static readonly LocalText CloneButton = "Clone";
            public static readonly LocalText SaveSuccessMessage = "Save success";
            public static readonly LocalText SaveButton = "Save";
            public static readonly LocalText UpdateButton = "Update";
            public static readonly LocalText ApplyChangesButton = "Apply Changes";
            public static readonly LocalText DeleteButton = "Delete";
            public static readonly LocalText EditButton = "Edit";
            public static readonly LocalText LocalizationButton = "Localization";
            public static readonly LocalText LocalizationBack = "Back to Form";
            public static readonly LocalText LocalizationConfirmation = "Save changes to translations?";
            public static readonly LocalText NewRecordTitle = "New {0}";
            public static readonly LocalText EditRecordTitle = "Edit {0}{1}";
            public static readonly LocalText ViewRecordTitle = "View {0}{1}";
        }

        public static class EntityGrid
        {
            public static readonly LocalText NewButton = "New {0}";
            public static readonly LocalText RefreshButton = "Refresh";
            public static readonly LocalText IncludeDeletedToggle = "display inactive records";
        }

        public static class ImageUpload
        {
            public static readonly LocalText AddFileButton = "Select File";
            public static readonly LocalText DeleteButtonHint = "Remove";
            public static readonly LocalText ColorboxCurrent = "image {current} / {total}";
            public static readonly LocalText ColorboxPrior = "prior";
            public static readonly LocalText ColorboxNext = "next";
            public static readonly LocalText ColorboxClose = "close";
        }

        public static class Pager
        {
            public static readonly LocalText Page = "Page";
            public static readonly LocalText PageStatus = "Showing {from} to {to} of {total} total records";
            public static readonly LocalText NoRowStatus = "No records";
            public static readonly LocalText LoadingStatus = "Please wait, loading data...";
            public static readonly LocalText DefaultLoadError = "An error occurred while loading data!";
        }

        public static class PropertyGrid
        {
            public static readonly LocalText DefaultCategory = "Properties";
            public static readonly LocalText RequiredHint = "this field is required";
        }

        public static class QuickSearch
        {
            public static readonly LocalText Placeholder = "search...";
            public static readonly LocalText Hint = "enter the text to search for...";
            public static readonly LocalText FieldSelection = "select the field to search on";
        }

        public static class SelectEditor
        {
            public static readonly LocalText EmptyItemText = "--select--";
            public static readonly LocalText InplaceAdd = "Define New";
            public static readonly LocalText InplaceEdit = "Edit";
            public static readonly LocalText ClickToDefine = "*** Click to define a new one ***";
            public static readonly LocalText NoResultsClickToDefine = "*** No results. Click to define a new one ***";
        }
    }
}