using System;

namespace Serenity.Reporting
{
    public class ReportDesignAttribute : Attribute
    {
        public ReportDesignAttribute(string design)
        {
            Design = design;
        }

        public string Design { get; private set; }
    }
}