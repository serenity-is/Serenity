namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing5_CrossJoin()
        {
            return new SqlQuery()
                .Select("Firstname")
                .Select("Surname")
                .From("People")
                .From("City")
                .From("Country")
                .OrderBy("p.Age")
                .ToString();
        }
    }
}