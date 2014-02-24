using Newtonsoft.Json;

namespace Serenity.Data
{
    public class BasicFilter : BasicFilterBase
    {
        public string Field { get; set; }
        public FilterOp Operator { get; set; }
        public string Value { get; set; }
        public string Value2 { get; set; }
        public string[] Values { get; set; }

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                if (Field.IsNullOrEmpty())
                    return false;

                if (Operator < FilterOp.True || Operator > FilterOp.NotIN)
                    return false;

                bool isIn = Operator == FilterOp.IN || Operator == FilterOp.NotIN;

                if ((!isIn && Values != null) ||
                    (isIn && (Values == null || Values.Length == 0)))
                    return false;

                return true;
            }
        }
    }
}
