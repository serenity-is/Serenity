namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing6_FromAs()
        {
            var p = new Alias("p");
            var c = new Alias("c");
            var o = new Alias("o");

            return new SqlQuery()
                .Select("p.Firstname")
                .Select("p.Surname")
                .Select("c.CityName")
                .Select("o.CountryName")
                .FromAs("People", p)
                .FromAs("City", c)
                .FromAs("Country", o)
                .OrderBy("p.Age")
                .ToString();
        }
    }
}