
namespace Serenity.Data
{
    public class SqlServer2012Dialect : SqlServer2008Dialect
    {
        public static new readonly ISqlDialect Instance = new SqlServer2012Dialect();

        public override bool CanUseOffsetFetch
        {
            get
            {
                return true;
            }
        }

        public override string OffsetFormat
        {
            get
            {
                return " OFFSET {0} ROWS";
            }
        }

        public override string OffsetFetchFormat
        {
            get
            {
                return " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY";
            }
        }
    }
}