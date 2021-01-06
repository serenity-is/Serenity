using System.Collections.Generic;

namespace Serenity.Reporting
{
    public abstract class BaseReport : IReport, IReportWithAdditionalData
    {
        public abstract object GetData();

        public virtual IDictionary<string, object> GetAdditionalData()
        {
            return null;
        }
    }
}