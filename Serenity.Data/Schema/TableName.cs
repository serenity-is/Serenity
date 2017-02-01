namespace Serenity.Data.Schema
{
    public class TableName
    {
        public string Schema { get; set; }
        public string Table { get; set; }
        public bool IsView { get; set; }

        public string Tablename
        {
            get
            {
                return Schema.IsEmptyOrNull() ? Table : Schema + "." + Table;
            }
        }
    }
}