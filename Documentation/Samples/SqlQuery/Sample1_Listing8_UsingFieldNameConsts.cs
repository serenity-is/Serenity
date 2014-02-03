namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        const string People = "People";
        const string Firstname = "Firstname";
        const string Surname = "Surname";
        const string Age = "Age";
        const string CityName = "CityName";
        const string CountryName = "CountryName";

        public static string Sample1_Listing8_UsingFieldNameConsts()
        {
            var p = new Alias("p");
            var c = new Alias("c");
            var o = new Alias("o");

            return new SqlQuery()
                .Select(p[Firstname])
                .Select(p[Surname])
                .Select(c[CityName])
                .Select(o[CountryName])
                .FromAs("People", p)
                .FromAs("City", c)
                .FromAs("Country", o)
                .OrderBy(p[Age])
                .ToString();
        }
    }
}