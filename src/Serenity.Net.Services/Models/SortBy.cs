namespace Serenity.Services
{
    [JsonConverter(typeof(JsonSortByConverter))]
    public class SortBy
    {
        public SortBy()
        {
        }

        public SortBy(string field)
        {
            Field = field;
        }

        public SortBy(string field, bool descending)
        {
            Field = field;
            Descending = descending;
        }

        public string Field { get; set; }
        public bool Descending { get; set; }
    }
}
