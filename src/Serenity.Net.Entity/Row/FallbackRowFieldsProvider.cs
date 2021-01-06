using System;
using System.Collections.Concurrent;

namespace Serenity.Data
{
    public class FallbackRowFieldsProvider : IRowFieldsProvider
    {
        public static FallbackRowFieldsProvider Instance = new FallbackRowFieldsProvider();
        private readonly ConcurrentDictionary<Type, RowFieldsBase> byType;
        private readonly ConcurrentDictionary<(Type type, string alias),
            RowFieldsBase> byTypeAndAlias;

        private FallbackRowFieldsProvider()
        {
            byType = new ConcurrentDictionary<Type, RowFieldsBase>();
            byTypeAndAlias = new ConcurrentDictionary<(Type, string), RowFieldsBase>();
        }

        public RowFieldsBase Resolve(Type fieldsType)
        {
            return byType.GetOrAdd(fieldsType, CreateType);
        }

        public RowFieldsBase ResolveWithAlias(Type fieldsType, string alias)
        {
            if (string.IsNullOrEmpty(alias))
                throw new ArgumentNullException(nameof(alias));

            return byTypeAndAlias.GetOrAdd((fieldsType, alias),
                tuple => CreateType(tuple.type, tuple.alias));
        }

        private RowFieldsBase CreateType(Type fieldsType)
        {
            return CreateType(fieldsType, null);
        }

        private RowFieldsBase CreateType(Type fieldsType, string alias)
        {
            var fields = (RowFieldsBase)Activator.CreateInstance(fieldsType);
            fields.Initialize(annotations: null, SqlSettings.DefaultDialect);

            if (alias != null)
                fields.ReplaceAliasWith(alias);
            fields.LockAlias();

            return fields;
        }
    }
}