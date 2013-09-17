using Serenity.Services;
using System;

namespace Serenity.Reporting
{
    public class ReportDesignListRequest : ListRequest
    {
        public string ReportKey { get; set; }
    }
}