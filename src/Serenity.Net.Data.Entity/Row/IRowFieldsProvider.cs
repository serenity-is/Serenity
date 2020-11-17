using System;

namespace Serenity.Data
{
    public interface IRowFieldsProvider
    {
        RowFieldsBase Resolve(Type fieldsType);
    }
}