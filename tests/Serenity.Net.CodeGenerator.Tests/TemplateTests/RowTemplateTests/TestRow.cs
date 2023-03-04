using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Data.Mapping;
using System.ComponentModel;

namespace TestNamespaceX.TestModule
{
    [ConnectionKey("TestConnection"), Module("TestModule"), TableName("[test].[Customer]")]
    [DisplayName("Customer"), InstanceName("Customer")]
    [ReadPermission("TestPermission")]
    [ModifyPermission("TestPermission")]
    public sealed class CustomerRow : Row<CustomerRow.RowFields>, IIdRow, INameRow
    {
        [DisplayName("Test Id"), Identity, IdProperty]
        public int? CustomerId
        {
            get => fields.CustomerId[this];
            set => fields.CustomerId[this] = value;
        }

        [DisplayName("Test Name"), Size(50), NotNull, QuickSearch, NameProperty]
        public string CustomerName
        {
            get => fields.CustomerName[this];
            set => fields.CustomerName[this] = value;
        }

        [DisplayName("City"), ForeignKey("[test].[City]", "CityId"), LeftJoin("jCity"), TextualField("CityCityName")]
        public int? CityId
        {
            get => fields.CityId[this];
            set => fields.CityId[this] = value;
        }

        [DisplayName("City City Name"), Expression("jCity.[CityName]")]
        public string CityCityName
        {
            get => fields.CityCityName[this];
            set => fields.CityCityName[this] = value;
        }

        [DisplayName("City Country Id"), Expression("jCity.[CountryId]")]
        public int? CityCountryId
        {
            get => fields.CityCountryId[this];
            set => fields.CityCountryId[this] = value;
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field CustomerId;
            public StringField CustomerName;
            public Int32Field CityId;

            public StringField CityCityName;
            public Int32Field CityCountryId;
        }
    }
}