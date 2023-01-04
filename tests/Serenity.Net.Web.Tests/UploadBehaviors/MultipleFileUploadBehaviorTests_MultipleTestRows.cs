namespace Serenity.Tests.Web;

public partial class MultipleFileUploadBehaviorTests
{
    public class MultipleTestRow : Row<MultipleTestRow.RowFields>
    {
        [Identity]
        public int? Id
        {
            get => fields.Id[this];
            set => fields.Id[this] = value;
        }

        [MultipleImageUploadEditor]
        public string StringFieldImageUploadEditor
        {
            get => fields.StringFieldImageUploadEditor[this];
            set => fields.StringFieldImageUploadEditor[this] = value;
        }

        [MultipleImageUploadEditor(DisableDefaultBehavior = true)]
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

    class MultipleCustomEditorImageUploadEditorAttribute : BaseUploadEditorAttribute
    {
        public override bool IsMultiple => true;

        public MultipleCustomEditorImageUploadEditorAttribute(string editorType)
            : base(editorType)
        {
        }
    }

    public class MultipleTestIIdRow : Row<MultipleTestIIdRow.RowFields>, IIdRow
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

        [MultipleImageUploadEditor]
        public string StringFieldImageUploadEditor
        {
            get => fields.StringFieldImageUploadEditor[this];
            set => fields.StringFieldImageUploadEditor[this] = value;
        }

        [MultipleImageUploadEditor]
        public int? IntegerFieldImageUploadEditor
        {
            get => fields.IntegerFieldImageUploadEditor[this];
            set => fields.IntegerFieldImageUploadEditor[this] = value;
        }

        [MultipleCustomEditorImageUploadEditor("MultipleImageUpload")]
        public string ImageUploadEditorCorrectEditorType
        {
            get => fields.ImageUploadEditorCorrectEditorType[this];
            set => fields.ImageUploadEditorCorrectEditorType[this] = value;
        }

        [Expression("(SELECT 'TEST')")]
        public string StringFieldExpression
        {
            get => fields.StringFieldExpression[this];
            set => fields.StringFieldExpression[this] = value;
        }

        [MultipleImageUploadEditor(FilenameFormat = "Test/|StringFieldExpression|")]
        public string ImageUploadEditorReplaceField
        {
            get => fields.ImageUploadEditorReplaceField[this];
            set => fields.ImageUploadEditorReplaceField[this] = value;
        }

        [MultipleImageUploadEditor(FilenameFormat = "Test/||")]
        public string ImageUploadEditorInvalidReplaceField
        {
            get => fields.ImageUploadEditorInvalidReplaceField[this];
            set => fields.ImageUploadEditorInvalidReplaceField[this] = value;
        }

        [MultipleImageUploadEditor(FilenameFormat = "Test/|ThisFieldDoesntExist|")]
        public string ImageUploadEditorReplaceFieldNoField
        {
            get => fields.ImageUploadEditorReplaceFieldNoField[this];
            set => fields.ImageUploadEditorReplaceFieldNoField[this] = value;
        }

        [MultipleImageUploadEditor(OriginalNameProperty = nameof(RowFields.Name))]
        public string ImageUploadEditorOriginalName
        {
            get => fields.ImageUploadEditorOriginalName[this];
            set => fields.ImageUploadEditorOriginalName[this] = value;
        }

        [MultipleImageUploadEditor(CopyToHistory = true)]
        public string ImageUploadEditorCopyToHistory
        {
            get => fields.ImageUploadEditorCopyToHistory[this];
            set => fields.ImageUploadEditorCopyToHistory[this] = value;
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public StringField StringFieldImageUploadEditor;
            public Int32Field IntegerFieldImageUploadEditor;
            public StringField ImageUploadEditorCorrectEditorType;
            public StringField StringFieldExpression;
            public StringField ImageUploadEditorReplaceField;
            public StringField ImageUploadEditorInvalidReplaceField;
            public StringField ImageUploadEditorReplaceFieldNoField;
            public StringField ImageUploadEditorOriginalName;
            public StringField ImageUploadEditorCopyToHistory;
        }
    }

    public class MultipleTestIIsActiveDeletedRowRow : Row<MultipleTestIIsActiveDeletedRowRow.RowFields>, IIdRow, IIsActiveDeletedRow
    {
        [Identity]
        public int? Id
        {
            get => fields.Id[this];
            set => fields.Id[this] = value;
        }

        [MultipleImageUploadEditor]
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

    public class MultipleTestIIsDeletedRow : Row<MultipleTestIIsDeletedRow.RowFields>, IIdRow, IIsDeletedRow
    {
        [Identity]
        public int? Id
        {
            get => fields.Id[this];
            set => fields.Id[this] = value;
        }

        [MultipleImageUploadEditor]
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

    public class MultipleTestIDeleteLogRow : Row<MultipleTestIDeleteLogRow.RowFields>, IIdRow, IDeleteLogRow
    {
        [Identity]
        public int? Id
        {
            get => fields.Id[this];
            set => fields.Id[this] = value;
        }

        [MultipleImageUploadEditor]
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