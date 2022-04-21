namespace Serenity.Tests.Services;

public partial class CriteriaFieldExpressionReplacerTests
{
    private const string ReadPermission = "Test:ReadPermission";
    private const string ServiceLookupPermission = "TestService:LookupPermission";
    private const string ExtraReadPermission = "Test:ExtraSpecialField";
    
    [ReadPermission(ReadPermission)]
    [ServiceLookupPermission(ServiceLookupPermission)]
    private class TestRow : Row<TestRow.RowFields>, IIdRow
    {
        [IdProperty]
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

        [LookupInclude]
        public string LookupIncludeField
        {
            get => fields.LookupIncludeField[this];
            set => fields.LookupIncludeField[this] = value;
        }

        public string NormalField
        {
            get => fields.NormalField[this];
            set => fields.NormalField[this] = value;
        }
        
        [ReadPermission(ExtraReadPermission)]
        public string ExtraReadPermissionField
        {
            get => fields.ExtraReadPermissionField[this];
            set => fields.ExtraReadPermissionField[this] = value;
        }
        
        [ReadPermission(ExtraReadPermission), LookupInclude]
        public string ExtraReadPermissionWithLookupIncludeField
        {
            get => fields.ExtraReadPermissionWithLookupIncludeField[this];
            set => fields.ExtraReadPermissionWithLookupIncludeField[this] = value;
        }
        
        [SetFieldFlags(FieldFlags.DenyFiltering)]
        public string DenyFilteringField
        {
            get => fields.DenyFilteringField[this];
            set => fields.DenyFilteringField[this] = value;
        }

        [NotMapped]
        public string NotMappedField
        {
            get => fields.NotMappedField[this];
            set => fields.NotMappedField[this] = value;
        }
        
        [MinSelectLevel(SelectLevel.Never)]
        public string MinSelectLevelNeverField
        {
            get => fields.MinSelectLevelNeverField[this];
            set => fields.MinSelectLevelNeverField[this] = value;
        }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Name;
            public StringField LookupIncludeField;
            public StringField NormalField;
            public StringField ExtraReadPermissionField;
            public StringField ExtraReadPermissionWithLookupIncludeField;
            public StringField DenyFilteringField;
            public StringField NotMappedField;
            public StringField MinSelectLevelNeverField;
#pragma warning restore CS0649
        }
    }
}