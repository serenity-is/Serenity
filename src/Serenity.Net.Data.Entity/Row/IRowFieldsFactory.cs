using System;

namespace Serenity.Data
{
    public interface IRowFieldsFactory
    {
        RowFieldsBase GetFields(Type type);
    }
}