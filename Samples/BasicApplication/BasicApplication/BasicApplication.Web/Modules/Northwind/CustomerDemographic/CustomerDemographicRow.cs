
namespace BasicApplication.Northwind.Entities
{
    using Newtonsoft.Json;
    using Serenity;
    using Serenity.Data;
    using Serenity.Data.Mapping;
    using System;
    using System.IO;
    using System.ComponentModel;

    [ConnectionKey("Default"), DisplayName("CustomerDemographics"), InstanceName("CustomerDemographics")]
    [ReadPermission("Northwind")]
    [ModifyPermission("Northwind")]
    [JsonConverter(typeof(JsonRowConverter))]
    public sealed class CustomerDemographicRow : Row, IIdRow, INameRow
    {
        [DisplayName("Id"), Identity]
        public Int32? ID
        {
            get { return Fields.ID[this]; }
            set { Fields.ID[this] = value; }
        }

        [DisplayName("Customer Type Id"), Size(10), PrimaryKey, QuickSearch]
        public String CustomerTypeID
        {
            get { return Fields.CustomerTypeID[this]; }
            set { Fields.CustomerTypeID[this] = value; }
        }

        [DisplayName("Customer Desc")]
        public String CustomerDesc
        {
            get { return Fields.CustomerDesc[this]; }
            set { Fields.CustomerDesc[this] = value; }
        }

        IIdField IIdRow.IdField
        {
            get { return Fields.ID; }
        }

        StringField INameRow.NameField
        {
            get { return Fields.CustomerTypeID; }
        }

        public static readonly RowFields Fields = new RowFields().Init();

        public CustomerDemographicRow()
            : base(Fields)
        {
        }

        public class RowFields : RowFieldsBase
        {
            public readonly Int32Field ID;
            public readonly StringField CustomerTypeID;
            public readonly StringField CustomerDesc;
            public RowFields()
                : base("CustomerDemographics")
            {
                LocalTextPrefix = "Northwind.CustomerDemographic";
            }
        }
    }
}