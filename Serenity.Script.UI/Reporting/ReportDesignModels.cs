
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
    public class ReportDesignEntity
    {
        public string ReportDesignId;
        public string ReportKey;
        public string ReportDesign;
        public string Filename;
    }
}