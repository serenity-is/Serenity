using System.Collections.Generic;

namespace Serenity.Data
{
    public interface ISqlQueryExtensible
    {
        IList<object> IntoRows { get; }
        void IntoRowSelection(object into);
        object FirstIntoRow { get; }
        IList<SqlQuery.Column> Columns { get; }
        int GetSelectIntoIndex(IField field);
    }
}
