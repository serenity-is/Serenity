namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing6_From()
        {
            return new SqlQuery()
                .Select("p.Firstname")
                .Select("p.Surname")
                .Select("p.CityName")
                .Select("p.CountryName")
                .From("Person p")
                .From("City c")
                .From("Country o")
                .OrderBy("p.Age")
                .ToString();
        }
    }
}