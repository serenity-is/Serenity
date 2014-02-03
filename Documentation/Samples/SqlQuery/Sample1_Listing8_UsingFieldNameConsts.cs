namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        const string Firstname = "Firstname";
        const string Surname = "Surname";
        const string Age = "Age";
        const string CityName = "CityName";
        const string CountryName = "CountryName";

        public static string Sample1_Listing8_UsingFieldNameConsts()
        {
            var p = new Alias("People", "p");
            var c = new Alias("City", "c");
            var o = new Alias("Country", "o");

            return new SqlQuery()
                .Select(p + Firstname)
                .Select(p + Surname)
                .Select(c + CityName)
                .Select(o + CountryName)
                .From(p)
                .From(c)
                .From(o)
                .OrderBy(p + Age)
                .ToString();
        }
    }
}