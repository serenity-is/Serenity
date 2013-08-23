
namespace Serenity.Data
{
    public partial class SqlQuery
    {
        /// <summary>
        ///   SQLSelect nesnesi içindeki işlemlerde kullanılan string sabitleri</summary>
        private static class Consts
        {
            public const string Comma = ", ";
            public const string DeclareCmd = "DECLARE @Value{0} SQL_VARIANT\n";
            public const string AssignCmd = "@Value{0} = {1}";
            public const string Greater = "(({0} IS NOT NULL AND @Value{1} IS NULL) OR ({0} > @Value{1}))";
            public const string LessThan = "(({0} IS NULL AND @Value{1} IS NOT NULL) OR ({0} < @Value{1}))";
            public const string Equality = "(({0} IS NULL AND @Value{1} IS NULL) OR ({0} = @Value{1}))";
        }       
    }

}