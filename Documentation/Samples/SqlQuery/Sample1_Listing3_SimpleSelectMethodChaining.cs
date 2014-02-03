namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing3_SimpleSelectMethodChaining()
        {
            var query = new SqlQuery()
                .Select("Firstname")
                .Select("Surname")
                .From("People")
                .OrderBy("Age");

            return query.ToString();
        }
    }
}