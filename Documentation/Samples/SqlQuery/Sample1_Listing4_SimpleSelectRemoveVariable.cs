namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing4_SimpleSelectRemoveVariable()
        {
            return new SqlQuery()
                .Select("Firstname")
                .Select("Surname")
                .From("People")
                .OrderBy("Age")
                .ToString();
        }
    }
}