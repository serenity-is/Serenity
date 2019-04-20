using System;

namespace Serenity.Reporting
{
    public class ReportAttribute : Attribute
    {
        public ReportAttribute(string reportKey = null)
        {
            this.ReportKey = reportKey;
        }

        public string ReportKey { get; private set; }
    }
}