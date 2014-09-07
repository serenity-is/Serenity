using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public interface ISqlQueryExtensible
    {
        Action<SqlQuery, string> EnsureJoinsInExpression { get; set; }
        IList<object> IntoRows { get; }
        void IntoRowSelection(object into);
        object FirstIntoRow { get; }
        IDictionary<string, string> Aliases { get; }
        IList<SqlQuery.Column> Columns { get; }
        int GetSelectIntoIndex(IField field);
    }
}
