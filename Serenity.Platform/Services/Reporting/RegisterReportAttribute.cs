using System;

namespace Serenity.Reporting
{
    public class RegisterReportAttribute : Attribute
    {
        public RegisterReportAttribute(string reportKey = null)
        {
            this.ReportKey = reportKey;
        }

        public string ReportKey { get; private set; }
    }
}