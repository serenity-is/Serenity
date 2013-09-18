using System.Collections;
using System.Collections.Generic;

namespace Serenity.Reporting
{
    public abstract class BaseReport : IReport, IReportWithAdditionalData
    {
        public abstract IEnumerable GetData();

        public virtual IDictionary<string, IEnumerable> GetAdditionalData()
        {
            return null;
        }
    }
}