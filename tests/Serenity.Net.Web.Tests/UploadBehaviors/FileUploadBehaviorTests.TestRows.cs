namespace Serenity.Tests.Web;

public partial class FileUploadBehaviorTests
{
    public class TestRow : Row<TestRow.RowFields>
    {
        [Identity]
        public int? Id
        {
            get => fields.Id[this];
            set => fields.Id[this] = value;
        }

        [ImageUploadEditor]
        public string StringFieldImageUploadEditor
        {
            get => fields.StringFieldImageUploadEditor[this];
            set => fields.StringFieldImageUploadEditor[this] = value;
        }

        [ImageUploadEditor(DisableDefaultBehavior = true)]
        public string ImageUploadEditorDisableDefaultBehavior
        {
            get => fields.ImageUploadEditorDisableDefaultBehavior[this];
            set => fields.ImageUploadEditorDisableDefaultBehavior[this] = value;
        }
        
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField StringFieldImageUploadEditor;
            public StringField ImageUploadEditorDisableDefaultBehavior;
        }
    }

    [TableName("dbo.[Test]")]
    public class TestIIdRow : Row<TestIIdRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id
        {
            get => fields.Id[this];
            set => fields.Id[this] = value;
        }

        [NameProperty]
        public string Name
        {
            get => fields.Name[this];
            set => fields.Name[this] = value;
        }

        [ImageUploadEditor]
        public string StringFieldImageUploadEditor
        {
            get => fields.StringFieldImageUploadEditor[this];
            set => fields.StringFieldImageUploadEditor[this] = value;
        }

        [ImageUploadEditor]
        public int? IntegerFieldImageUploadEditor
        {
            get => fields.IntegerFieldImageUploadEditor[this];
            set => fields.IntegerFieldImageUploadEditor[this] = value;
        }

        [Expression("(SELECT 'TEST')")]
        public string StringFieldExpression
        {
            get => fields.StringFieldExpression[this];
            set => fields.StringFieldExpression[this] = value;
        }
        
        [Expression("(SELECT 123)")]
        public int? IntegerFieldExpression
        {
            get => fields.IntegerFieldExpression[this];
            set => fields.IntegerFieldExpression[this] = value;
        }

        [ImageUploadEditor(FilenameFormat = "Test/|StringFieldExpression|")]
        public string ImageUploadEditorReplaceField
        {
            get => fields.ImageUploadEditorReplaceField[this];
            set => fields.ImageUploadEditorReplaceField[this] = value;
        }

        [ImageUploadEditor(FilenameFormat = "Test/||")]
        public string ImageUploadEditorInvalidReplaceField
        {
            get => fields.ImageUploadEditorInvalidReplaceField[this];
            set => fields.ImageUploadEditorInvalidReplaceField[this] = value;
        }
        
        [ImageUploadEditor(FilenameFormat = "Test/|ThisFieldDoesntExist|")]
        public string ImageUploadEditorReplaceFieldNoField
        {
            get => fields.ImageUploadEditorReplaceFieldNoField[this];
            set => fields.ImageUploadEditorReplaceFieldNoField[this] = value;
        }

        [ImageUploadEditor(OriginalNameProperty = nameof(RowFields.Name))]
        public string ImageUploadEditorOriginalName
        {
            get => fields.ImageUploadEditorOriginalName[this];
            set => fields.ImageUploadEditorOriginalName[this] = value;
        }

        [ImageUploadEditor(OriginalNameProperty = nameof(RowFields.Id))]
        public string ImageUploadEditorOriginalNameIntegerField
        {
            get => fields.ImageUploadEditorOriginalNameIntegerField[this];
            set => fields.ImageUploadEditorOriginalNameIntegerField[this] = value;
        }

        [ImageUploadEditor(OriginalNameProperty = "ThisFieldDoesntExist")]
        public string ImageUploadEditorOriginalNameNoField
        {
            get => fields.ImageUploadEditorOriginalNameNoField[this];
            set => fields.ImageUploadEditorOriginalNameNoField[this] = value;
        }

        [ImageUploadEditor(CopyToHistory = true)]
        public string ImageUploadEditorCopyToHistory
        {
            get => fields.ImageUploadEditorCopyToHistory[this];
            set => fields.ImageUploadEditorCopyToHistory[this] = value;
        }

        public string Empty
        {
            get => fields.Empty[this];
            set => fields.Empty[this] = value;
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public StringField StringFieldImageUploadEditor;
            public Int32Field IntegerFieldImageUploadEditor;
            public StringField StringFieldExpression;
            public Int32Field IntegerFieldExpression;
            public StringField ImageUploadEditorReplaceField;
            public StringField ImageUploadEditorInvalidReplaceField;
            public StringField ImageUploadEditorReplaceFieldNoField;
            public StringField ImageUploadEditorOriginalName;
            public StringField ImageUploadEditorOriginalNameIntegerField;
            public StringField ImageUploadEditorOriginalNameNoField;
            public StringField ImageUploadEditorCopyToHistory;
            public StringField Empty;
        }
    }

    public class TestIIsActiveDeletedRowRow : Row<TestIIsActiveDeletedRowRow.RowFields>, IIdRow, IIsActiveDeletedRow
    {
        [Identity]
        public int? Id
        {
            get => fields.Id[this];
            set => fields.Id[this] = value;
        }

        [ImageUploadEditor]
        public string StringFieldImageUploadEditor
        {
            get => fields.StringFieldImageUploadEditor[this];
            set => fields.StringFieldImageUploadEditor[this] = value;
        }

        public short? IsActive
        {
            get => fields.IsActive[this];
            set => fields.IsActive[this] = value;
        }

        public Int16Field IsActiveField => fields.IsActive;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField StringFieldImageUploadEditor;
            public Int16Field IsActive;
        }
    }

    public class TestIIsDeletedRow : Row<TestIIsDeletedRow.RowFields>, IIdRow, IIsDeletedRow
    {
        [Identity]
        public int? Id
        {
            get => fields.Id[this];
            set => fields.Id[this] = value;
        }

        [ImageUploadEditor]
        public string StringFieldImageUploadEditor
        {
            get => fields.StringFieldImageUploadEditor[this];
            set => fields.StringFieldImageUploadEditor[this] = value;
        }

        public bool? IsDeleted
        {
            get => fields.IsDeleted[this];
            set => fields.IsDeleted[this] = value;
        }

        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField StringFieldImageUploadEditor;
            public BooleanField IsDeleted;
        }
    }

    public class TestIDeleteLogRow : Row<TestIDeleteLogRow.RowFields>, IIdRow, IDeleteLogRow
    {
        [Identity]
        public int? Id
        {
            get => fields.Id[this];
            set => fields.Id[this] = value;
        }

        [ImageUploadEditor]
        public string StringFieldImageUploadEditor
        {
            get => fields.StringFieldImageUploadEditor[this];
            set => fields.StringFieldImageUploadEditor[this] = value;
        }

        public int? DeleteUserId
        {
            get => fields.DeleteUserId[this];
            set => fields.DeleteUserId[this] = value;
        }

        public DateTime? DeleteDate
        {
            get => fields.DeleteDate[this];
            set => fields.DeleteDate[this] = value;
        }

        public Field DeleteUserIdField => fields.DeleteUserId;
        public DateTimeField DeleteDateField => fields.DeleteDate;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField StringFieldImageUploadEditor;
            public Int32Field DeleteUserId;
            public DateTimeField DeleteDate;
        }
    }
}