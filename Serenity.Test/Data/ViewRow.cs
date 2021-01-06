using Serenity.Data;
using Serenity.Data.Mapping;
using System;
using System.ComponentModel;

namespace Serenity.Test.Data
{
    [TableName("ViewTable")]
    public class ViewRow : Row
    {
        [Identity]
        public Int32? ID
        {
            get { return Fields.ID[this]; }
            set { Fields.ID[this] = value; }
        }

        [NotNull]
        public String Firstname
        {
            get { return Fields.Firstname[this]; }
            set { Fields.Firstname[this] = value; }
        }

        [NotNull]
        public String Surname
        {
            get { return Fields.Surname[this]; }
            set { Fields.Surname[this] = value; }
        }

        [Expression("(T0.Name + ' ' + T0.Surname)")]
        public String FullName
        {
            get { return Fields.FullName[this]; }
            set { Fields.FullName[this] = value; }
        }

        [ForeignKey("Districts", "DistrictID"), LeftJoin("d")]
        public Int32? DistrictID
        {
            get { return Fields.DistrictID[this]; }
            set { Fields.DistrictID[this] = value; }
        }

        [DisplayName("District"), Expression("d.Name")]
        public String District
        {
            get { return Fields.District[this]; }
            set { Fields.District[this] = value; }
        }

        [DisplayName("City"), Expression("d.CityID")]
        [ForeignKey("Cities", "CityID"), LeftJoin("c")]
        public Int32? CityID
        {
            get { return Fields.CityID[this]; }
            set { Fields.CityID[this] = value; }
        }

        [DisplayName("City"), Expression("c.Name")]
        public String City
        {
            get { return Fields.City[this]; }
            set { Fields.City[this] = value; }
        }

        [DisplayName("Country ID"), Expression("c.CountryID")]
        [ForeignKey("Countries", "CountryID"), LeftJoin("o")]
        public Int32? CountryID
        {
            get { return Fields.CountryID[this]; }
            set { Fields.CountryID[this] = value; }
        }

        [DisplayName("Country"), Expression("o.Name")]
        public String Country
        {
            get { return Fields.Country[this]; }
            set { Fields.Country[this] = value; }
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID;
            public StringField Firstname;
            public StringField Surname;
            public StringField FullName;
            public Int32Field DistrictID;
            public StringField District;
            public Int32Field CityID;
            public StringField City;
            public Int32Field CountryID;
            public StringField Country;

            public RowFields()
            {
            }
        }

        public static RowFields Fields = new RowFields().Init();

        public ViewRow()
            : base(Fields)
        {
        }
    }
}