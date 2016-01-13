using System;

namespace Serenity.Reporting
{
    public class ReportDesignAttribute : Attribute
    {
        public ReportDesignAttribute(string design)
        {
            this.Design = design;
        }

        public string Design { get; private set; }
    }
}