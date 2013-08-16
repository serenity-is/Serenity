using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serenity.Data
{
    public interface IStagingRow : IIdRow
    {
        Int64Field StagingBaseIdField { get; }
        Int64Field StagingStatusIdField { get; }
        StringField StagingNoteField { get; }
    }
}
