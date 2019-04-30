
namespace Serenity.Data
{
    public class Oracle12cDialect : OracleDialect
    {
        public static new readonly ISqlDialect Instance = new Oracle12cDialect();

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