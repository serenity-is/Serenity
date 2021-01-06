using Serenity.ComponentModel;
using Serenity.Services;
using System.Collections.Generic;

namespace Serenity.Reporting
{
    public class ReportRetrieveResponse : ServiceResponse
    {
        public string ReportKey { get; set; }
        public string Title { get; set; }
        public List<PropertyItem> Properties { get; set; }
        public object InitialSettings { get; set; }
        public bool IsDataOnlyReport { get; set; }
    }
}