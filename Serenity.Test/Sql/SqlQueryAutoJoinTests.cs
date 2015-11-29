using Serenity.Data;
using Serenity.Testing;
using System.Linq;
using Xunit;

namespace Serenity.Test.Data
{
    public partial class SqlQueryAutoJoinTests
    {
        [Fact]
        public void NoAutomaticJoinsIfFromNotUsed()
        {
            var fld = RowMappingTests.ComplexRow.Fields;

            var query = new SqlQuery()
                    .Select(fld.CountryName);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT c.Name AS CountryName"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void DoesAutomaticJoinsIfFromFieldsUsed()
        {
            var fld = RowMappingTests.ComplexRow.Fields;

            var query = new SqlQuery()
                    .From(fld)
                    .Select(fld.CountryName);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT c.Name AS CountryName FROM ComplexTable T0 " +
                    "LEFT JOIN TheCountryTable c ON (c.TheCountryID = T0.CountryID)"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void DoesAutomaticJoinsIfFromRowUsed()
        {
            var fld = RowMappingTests.ComplexRow.Fields;

            var query = new SqlQuery()
                    .From(new RowMappingTests.ComplexRow())
                    .Select(fld.CountryName);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT c.Name AS CountryName FROM ComplexTable T0 " +
                    "LEFT JOIN TheCountryTable c ON (c.TheCountryID = T0.CountryID)"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void DoesAutomaticJoinProperlyWithAliasedFields()
        {
            var x = RowMappingTests.ComplexRow.Fields.As("x");

            var query = new SqlQuery()
                    .From(x)
                    .Select(x.CountryName);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT x_c.Name AS CountryName FROM ComplexTable x " +
                    "LEFT JOIN TheCountryTable x_c ON (x_c.TheCountryID = x.CountryID)"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void DoesAutomaticJoinProperlyWithAliasedAndJoinedFieldsAndSelect()
        {
            var x = RowMappingTests.ComplexRow.Fields.As("x");
            var y = RowMappingTests.ComplexRow.Fields.As("y");

            var query = new SqlQuery()
                    .From(x)
                    .LeftJoin(y, y.ID == x.ID)
                    .Select(x.CountryName, "CountryNameX")
                    .Select(y.CountryName, "CountryNameY");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT " + 
                        "x_c.Name AS CountryNameX, " +
                        "y_c.Name AS CountryNameY " +
                    "FROM ComplexTable x " +
                    "LEFT JOIN ComplexTable y ON (y.ComplexID = x.ComplexID) " +
                    "LEFT JOIN TheCountryTable x_c ON (x_c.TheCountryID = x.CountryID) " +
                    "LEFT JOIN TheCountryTable y_c ON (y_c.TheCountryID = y.CountryID)"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void DoesAutomaticJoinProperlyWithViewRowIfOnlyCountryIsSelected()
        {
            var fld = ViewRow.Fields;

            var query = new SqlQuery()
                    .From(fld)
                    .Select(fld.Country);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT " +
                        "o.Name AS Country " +
                    "FROM ViewTable T0 " +
                    "LEFT JOIN Districts d ON (d.DistrictID = T0.DistrictID) " +
                    "LEFT JOIN Cities c ON (c.CityID = d.CityID) " +
                    "LEFT JOIN Countries o ON (o.CountryID = c.CountryID) "),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void DoesAutomaticJoinProperlyWithViewRowIfOnlyCityIsSelected()
        {
            var fld = ViewRow.Fields;

            var query = new SqlQuery()
                    .From(fld)
                    .Select(fld.City);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT " +
                        "c.Name AS City " +
                    "FROM ViewTable T0 " +
                    "LEFT JOIN Districts d ON (d.DistrictID = T0.DistrictID) " +
                    "LEFT JOIN Cities c ON (c.CityID = d.CityID) "),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void DoesAutomaticJoinProperlyWithViewRowIfOnlyDistrictIsSelected()
        {
            var fld = ViewRow.Fields;

            var query = new SqlQuery()
                    .From(fld)
                    .Select(fld.District);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT " +
                        "d.Name AS District " +
                    "FROM ViewTable T0 " +
                    "LEFT JOIN Districts d ON (d.DistrictID = T0.DistrictID) "),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }


        [Fact]
        public void DoesAutomaticJoinProperlyWithViewRowIfAllNamesAreSelectedInReverseOrder()
        {

            var fld = ViewRow.Fields;

            var query = new SqlQuery()
                    .From(fld)
                    .Select(fld.Country)
                    .Select(fld.City)
                    .Select(fld.District);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT " +
                        "o.Name AS Country, " +
                        "c.Name AS City, " +
                        "d.Name AS District " +
                    "FROM ViewTable T0 " +
                    "LEFT JOIN Districts d ON (d.DistrictID = T0.DistrictID) " +
                    "LEFT JOIN Cities c ON (c.CityID = d.CityID) " +
                    "LEFT JOIN Countries o ON (o.CountryID = c.CountryID) "),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void DoesAutomaticJoinProperlyWithViewRowIfAllNamesAreSelectedInJoinOrder()
        {
            var fld = ViewRow.Fields;

            var query = new SqlQuery()
                    .From(fld)
                    .Select(fld.District)
                    .Select(fld.City)
                    .Select(fld.Country);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT " +
                        "d.Name AS District, " +
                        "c.Name AS City, " +
                        "o.Name AS Country " +
                    "FROM ViewTable T0 " +
                    "LEFT JOIN Districts d ON (d.DistrictID = T0.DistrictID) " +
                    "LEFT JOIN Cities c ON (c.CityID = d.CityID) " +
                    "LEFT JOIN Countries o ON (o.CountryID = c.CountryID) "),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void DoesAutomaticJoinProperlyWithViewRowIfAreSelectedInJoinOrderAliased()
        {
            var vw = ViewRow.Fields.As("vw");

            var query = new SqlQuery()
                    .From(vw)
                    .Select(vw.District)
                    .Select(vw.City)
                    .Select(vw.Country);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT " +
                        "vw_d.Name AS District, " +
                        "vw_c.Name AS City, " +
                        "vw_o.Name AS Country " +
                    "FROM ViewTable vw " +
                    "LEFT JOIN Districts vw_d ON (vw_d.DistrictID = vw.DistrictID) " +
                    "LEFT JOIN Cities vw_c ON (vw_c.CityID = vw_d.CityID) " +
                    "LEFT JOIN Countries vw_o ON (vw_o.CountryID = vw_c.CountryID) "),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }
    }
}