using System;
using System.Collections.Concurrent;

namespace Serenity.Data
{
    public class FallbackRowFieldsProvider : IRowFieldsProvider
    {
        public static FallbackRowFieldsProvider Instance = new FallbackRowFieldsProvider();
        private readonly ConcurrentDictionary<Type, RowFieldsBase> byType;

        private FallbackRowFieldsProvider()
        {
            byType = new ConcurrentDictionary<Type, RowFieldsBase>();
        }

        public RowFieldsBase Resolve(Type fieldsType)
        {
            return byType.GetOrAdd(fieldsType, CreateType);
        }

        private RowFieldsBase CreateType(Type fieldsType)
        {
            var fields = (RowFieldsBase)Activator.CreateInstance(fieldsType);
            fields.Initialize(annotations: null, SqlSettings.DefaultDialect);
            return fields;
        }
    }
}