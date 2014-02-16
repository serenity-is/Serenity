using Serenity.Services;
using System;

namespace Serenity.Reporting
{
    public class ReportExecuteRequest : ServiceRequest
    {
        public string ReportKey { get; set; }
        public object Parameters { get; set; }
        public string DesignId { get; set; }
        public ReportExportType? ExportType { get; set; }
    }
}