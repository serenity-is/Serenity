namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing1_SimpleSelectFromOrderBy()
        {
            var query = new SqlQuery();
            query.Select("Firstname");
            query.Select("Surname");
            query.From("People");
            query.OrderBy("Age");
            
            return query.ToString();
        }
    }
}