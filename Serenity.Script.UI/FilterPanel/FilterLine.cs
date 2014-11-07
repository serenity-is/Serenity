using Serenity.Data;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable]
    public class FilterLine
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public bool IsOr { get; set; }
        public bool LeftParen { get; set; }
        public bool RightParen { get; set; }
        public string ValidationError { get; set; }
        public BaseCriteria Criteria { get; set; }
        public string DisplayText { get; set; }
        public object State { get; set; }
    }
}