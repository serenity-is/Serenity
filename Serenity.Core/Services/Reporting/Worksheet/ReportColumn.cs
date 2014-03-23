using Serenity.Data;
using System;

namespace Serenity.Reporting
{
    public class ReportColumn
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public double? Width { get; set; }
        public Type DataType { get; set; }
        public string Format { get; set; }
        public bool WrapText { get; set; }
    }
}