
namespace Serenity.Reporting
{
    using System;
    using System.Runtime.CompilerServices;

    [Imported, Serializable, PreserveMemberCase]
    public class ReportDesignListRequest : ServiceRequest
    {
        public string ReportKey;
    }

    [Imported, Serializable, PreserveMemberCase]
    public class ReportDesignItem
    {
        public string DesignId;
        public string ReportKey;
    }
}