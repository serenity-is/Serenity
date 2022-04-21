namespace Serenity.Tests.Services;

public partial class ListRequestHandlerTests
{
    private const string ReadPermission = "Test:ReadPermission";
    private const string ServiceLookupPermission = "TestService:LookupPermission";
    private const string ExtraSpecialFieldPermission = "Test:ExtraSpecialField";
    
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
        
        [ReadPermission(ExtraSpecialFieldPermission)]
        public string ExtraSpecialField
        {
            get => fields.ExtraSpecialField[this];
            set => fields.ExtraSpecialField[this] = value;
        }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Name;
            public StringField LookupIncludeField;
            public StringField NormalField;
            public StringField ExtraSpecialField;
#pragma warning restore CS0649
        }
    }
}