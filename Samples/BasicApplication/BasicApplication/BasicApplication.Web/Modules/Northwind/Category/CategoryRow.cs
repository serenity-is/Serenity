
namespace BasicApplication.Northwind.Entities
{
    using Newtonsoft.Json;
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using Serenity.Data.Mapping;
    using System;
    using System.ComponentModel;
    using System.IO;

    [ConnectionKey("Default"), DisplayName("Categories"), InstanceName("Category")]
    [ReadPermission("Northwind")]
    [ModifyPermission("Northwind")]
    [JsonConverter(typeof(JsonRowConverter))]
    [LookupScript("Northwind.Category")]
    public sealed class CategoryRow : Row, IIdRow, INameRow
    {
        [DisplayName("Category Id"), Identity]
        public Int32? CategoryID
        {
            get { return Fields.CategoryID[this]; }
            set { Fields.CategoryID[this] = value; }
        }

        [DisplayName("Category Name"), Size(15), NotNull, QuickSearch]
        public String CategoryName
        {
            get { return Fields.CategoryName[this]; }
            set { Fields.CategoryName[this] = value; }
        }

        [DisplayName("Description"), QuickSearch]
        public String Description
        {
            get { return Fields.Description[this]; }
            set { Fields.Description[this] = value; }
        }

        [DisplayName("Picture")]
        public Stream Picture
        {
            get { return Fields.Picture[this]; }
            set { Fields.Picture[this] = value; }
        }

        IIdField IIdRow.IdField
        {
            get { return Fields.CategoryID; }
        }

        StringField INameRow.NameField
        {
            get { return Fields.CategoryName; }
        }

        public static readonly RowFields Fields = new RowFields().Init();

        public CategoryRow()
            : base(Fields)
        {
        }

        public class RowFields : RowFieldsBase
        {
            public readonly Int32Field CategoryID;
            public readonly StringField CategoryName;
            public readonly StringField Description;
            public readonly StreamField Picture;
            public RowFields()
                : base("Categories")
            {
                LocalTextPrefix = "Northwind.Category";
            }
        }
    }
}