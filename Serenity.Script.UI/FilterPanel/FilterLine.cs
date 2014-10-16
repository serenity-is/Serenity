
namespace Serenity
{
    public class FilterLine
    {
        public IFilterField Field { get; set; }
        public string Operator { get; set; }
        public bool IsOr { get; set; }
        public bool LeftParen { get; set; }
        public bool RightParen { get; set; }
        public string ValidationError { get; set; }
        public object Value { get; set; }
        public string DisplayText { get; set; }
    }
}