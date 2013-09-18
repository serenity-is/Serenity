using System;
using System.Runtime.CompilerServices;
using Serenity.Services;

namespace Serenity.Reporting
{
    public class ReportRetrieveRequest : ServiceRequest
    {
        public string ReportKey { get; set; }
    }
}