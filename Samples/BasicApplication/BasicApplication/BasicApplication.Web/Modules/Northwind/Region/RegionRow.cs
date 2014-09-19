
namespace BasicApplication.Northwind.Entities
{
    using Newtonsoft.Json;
    using Serenity;
    using Serenity.Data;
    using Serenity.Data.Mapping;
    using System;
    using System.IO;
    using System.ComponentModel;
    using Serenity.ComponentModel;

    [ConnectionKey("Default"), DisplayName("Region"), InstanceName("Region"), TwoLevelCached]
    [ReadPermission("Northwind")]
    [ModifyPermission("Northwind")]
    [JsonConverter(typeof(JsonRowConverter))]
    [LookupScript("Northwind.Region")]
    public sealed class RegionRow : Row, IIdRow, INameRow
    {
        [DisplayName("Region Id"), PrimaryKey, NotNull, Updatable(false), QuickSearch]
        public Int32? RegionID
        {
            get { return Fields.RegionID[this]; }
            set { Fields.RegionID[this] = value; }
        }

        [DisplayName("Region Description"), Size(50), NotNull, QuickSearch]
        public String RegionDescription
        {
            get { return Fields.RegionDescription[this]; }
            set { Fields.RegionDescription[this] = value; }
        }

        IIdField IIdRow.IdField
        {
            get { return Fields.RegionID; }
        }

        StringField INameRow.NameField
        {
            get { return Fields.RegionDescription; }
        }

        public static readonly RowFields Fields = new RowFields().Init();

        public RegionRow()
            : base(Fields)
        {
        }

        public class RowFields : RowFieldsBase
        {
            public readonly Int32Field RegionID;
            public readonly StringField RegionDescription;
            public RowFields()
                : base("Region")
            {
                LocalTextPrefix = "Northwind.Region";
            }
        }
    }
}