
namespace Serenity.Reporting
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [Imported, Serializable, PreserveMemberCase]
    public class ReportExecuteRequest : ServiceRequest
    {
        public string ExportType;
        public string ReportKey;
        public string DesignId;
        public object Parameters;
    }

    [Imported, Serializable, PreserveMemberCase]
    public class ReportRetrieveRequest : ServiceRequest
    {
        public string ReportKey;
    }

    [Imported, Serializable, PreserveMemberCase]
    public class ReportRetrieveResponse : ServiceResponse
    {
        public string ReportKey;
        public List<PropertyItem> Properties;
        public string Title;
        public object InitialSettings;
        public List<ReportDesignItem> Designs;
    }
}