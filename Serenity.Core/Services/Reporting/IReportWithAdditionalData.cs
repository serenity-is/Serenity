using System.Collections;
using System.Collections.Generic;

namespace Serenity.Reporting
{
    public interface IReportWithAdditionalData
    {
        IDictionary<string, IEnumerable> GetAdditionalData();
    }
}