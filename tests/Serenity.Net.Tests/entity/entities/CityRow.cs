﻿namespace Serenity.Tests.Entities;

[TableName("Cities")]
public class CityRow : Row<CityRow.RowFields>, IIdRow, INameRow
{
    [NotNull, Identity, IdProperty]
    public int? CityId { get => fields.CityId[this]; set => fields.CityId[this] = value; }

    [NotNull, NameProperty]
    public string CityName { get => fields.CityName[this]; set => fields.CityName[this] = value; }

    [ForeignKey("Countries", "CountryId"), LeftJoin("jCountry")]
    public int? CountryId { get => fields.CountryId[this]; set => fields.CountryId[this] = value; }

    [Expression("jCountry.CountryName")]
    public string CountryName { get => fields.CountryName[this]; set => fields.CountryName[this] = value; }

    [Expression("jCity.CityName + ' / ' + jCountry.CountryName")]
    public string FullName { get => fields.FullName[this]; set => fields.FullName[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field CityId;
        public StringField CityName;
        public Int32Field CountryId;
        public StringField CountryName;
        public StringField FullName;
    }
}