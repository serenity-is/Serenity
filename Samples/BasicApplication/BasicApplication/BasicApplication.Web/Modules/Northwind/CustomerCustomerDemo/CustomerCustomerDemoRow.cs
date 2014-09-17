
namespace BasicApplication.Northwind.Entities
{
    using Newtonsoft.Json;
    using Serenity;
    using Serenity.Data;
    using Serenity.Data.Mapping;
    using System;
    using System.IO;
    using System.ComponentModel;

    [ConnectionKey("Default"), DisplayName("CustomerCustomerDemo"), InstanceName("CustomerCustomerDemo")]
    [ReadPermission("Northwind")]
    [ModifyPermission("Northwind")]
    [JsonConverter(typeof(JsonRowConverter))]
    public sealed class CustomerCustomerDemoRow : Row, IIdRow, INameRow
    {
        [DisplayName("Id"), Identity]
        public Int32? ID
        {
            get { return Fields.ID[this]; }
            set { Fields.ID[this] = value; }
        }

        [DisplayName("Customer Id"), Size(5), PrimaryKey, ForeignKey("Customers", "CustomerID"), LeftJoin("jCustomer"), QuickSearch]
        public String CustomerID
        {
            get { return Fields.CustomerID[this]; }
            set { Fields.CustomerID[this] = value; }
        }

        [DisplayName("Customer Type Id"), Size(10), PrimaryKey, ForeignKey("CustomerDemographics", "CustomerTypeID"), LeftJoin("jCustomerType")]
        public String CustomerTypeID
        {
            get { return Fields.CustomerTypeID[this]; }
            set { Fields.CustomerTypeID[this] = value; }
        }

        [DisplayName("Customer Company Name"), Expression("jCustomer.CompanyName")]
        public String CustomerCompanyName
        {
            get { return Fields.CustomerCompanyName[this]; }
            set { Fields.CustomerCompanyName[this] = value; }
        }

        [DisplayName("Customer Contact Name"), Expression("jCustomer.ContactName")]
        public String CustomerContactName
        {
            get { return Fields.CustomerContactName[this]; }
            set { Fields.CustomerContactName[this] = value; }
        }

        [DisplayName("Customer Contact Title"), Expression("jCustomer.ContactTitle")]
        public String CustomerContactTitle
        {
            get { return Fields.CustomerContactTitle[this]; }
            set { Fields.CustomerContactTitle[this] = value; }
        }

        [DisplayName("Customer Address"), Expression("jCustomer.Address")]
        public String CustomerAddress
        {
            get { return Fields.CustomerAddress[this]; }
            set { Fields.CustomerAddress[this] = value; }
        }

        [DisplayName("Customer City"), Expression("jCustomer.City")]
        public String CustomerCity
        {
            get { return Fields.CustomerCity[this]; }
            set { Fields.CustomerCity[this] = value; }
        }

        [DisplayName("Customer Region"), Expression("jCustomer.Region")]
        public String CustomerRegion
        {
            get { return Fields.CustomerRegion[this]; }
            set { Fields.CustomerRegion[this] = value; }
        }

        [DisplayName("Customer Postal Code"), Expression("jCustomer.PostalCode")]
        public String CustomerPostalCode
        {
            get { return Fields.CustomerPostalCode[this]; }
            set { Fields.CustomerPostalCode[this] = value; }
        }

        [DisplayName("Customer Country"), Expression("jCustomer.Country")]
        public String CustomerCountry
        {
            get { return Fields.CustomerCountry[this]; }
            set { Fields.CustomerCountry[this] = value; }
        }

        [DisplayName("Customer Phone"), Expression("jCustomer.Phone")]
        public String CustomerPhone
        {
            get { return Fields.CustomerPhone[this]; }
            set { Fields.CustomerPhone[this] = value; }
        }

        [DisplayName("Customer Fax"), Expression("jCustomer.Fax")]
        public String CustomerFax
        {
            get { return Fields.CustomerFax[this]; }
            set { Fields.CustomerFax[this] = value; }
        }

        [DisplayName("Customer Type Customer Desc"), Expression("jCustomerType.CustomerDesc")]
        public String CustomerTypeCustomerDesc
        {
            get { return Fields.CustomerTypeCustomerDesc[this]; }
            set { Fields.CustomerTypeCustomerDesc[this] = value; }
        }

        IIdField IIdRow.IdField
        {
            get { return Fields.ID; }
        }

        StringField INameRow.NameField
        {
            get { return Fields.CustomerID; }
        }

        public static readonly RowFields Fields = new RowFields().Init();

        public CustomerCustomerDemoRow()
            : base(Fields)
        {
        }

        public class RowFields : RowFieldsBase
        {
            public readonly Int32Field ID;
            public readonly StringField CustomerID;
            public readonly StringField CustomerTypeID;

            public readonly StringField CustomerCompanyName;
            public readonly StringField CustomerContactName;
            public readonly StringField CustomerContactTitle;
            public readonly StringField CustomerAddress;
            public readonly StringField CustomerCity;
            public readonly StringField CustomerRegion;
            public readonly StringField CustomerPostalCode;
            public readonly StringField CustomerCountry;
            public readonly StringField CustomerPhone;
            public readonly StringField CustomerFax;

            public readonly StringField CustomerTypeCustomerDesc;

            public RowFields()
                : base("CustomerCustomerDemo")
            {
                LocalTextPrefix = "Northwind.CustomerCustomerDemo";
            }
        }
    }
}