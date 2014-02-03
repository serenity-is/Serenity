namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing2_SimpleSelectReordered()
        {
            var query = new SqlQuery();
            query.OrderBy("Age");
            query.Select("Surname");
            query.From("People");
            query.Select("Firstname");

            return query.ToString();
        }
    }
}