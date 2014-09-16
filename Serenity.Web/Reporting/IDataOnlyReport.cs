using System.Collections;
using System.Collections.Generic;

namespace Serenity.Reporting
{
    public interface IDataOnlyReport : IReport
    {
        List<ReportColumn> GetColumnList();
    }
}