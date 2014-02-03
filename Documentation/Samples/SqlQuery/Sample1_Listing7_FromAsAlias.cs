namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing7_FromAsAlias()
        {
            var p = new Alias("People", "p");
            var c = new Alias("City", "c");
            var o = new Alias("Country", "o");

            return new SqlQuery()
                .Select(p + "Firstname")
                .Select(p + "Surname")
                .Select(c + "CityName")
                .Select(o + "CountryName")
                .From(p)
                .From(c)
                .From(o)
                .OrderBy(p + "Age")
                .ToString();
        }
    }
}