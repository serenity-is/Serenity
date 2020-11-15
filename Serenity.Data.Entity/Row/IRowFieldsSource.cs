using System;

namespace Serenity.Data
{
    public interface IRowFieldsSource
    {
        RowFieldsBase GetFields(Type type);
    }
}