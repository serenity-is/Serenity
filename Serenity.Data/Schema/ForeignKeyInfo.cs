namespace Serenity.Data.Schema
{
    public class ForeignKeyInfo
    {
        public string FKName { get; set; }
        public string FKColumn { get; set; }
        public string PKSchema { get; set; }
        public string PKTable { get; set; }
        public string PKColumn { get; set; }
    }
}