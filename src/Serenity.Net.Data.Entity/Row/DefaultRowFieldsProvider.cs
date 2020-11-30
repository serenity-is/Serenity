using Serenity.Reflection;
using System;
using System.Collections.Concurrent;

namespace Serenity.Data
{
    public class DefaultRowFieldsProvider : IRowFieldsProvider
    {
        private readonly IAnnotationTypeRegistry annotationRegistry;
        private readonly IConnectionStrings connectionStrings;
        private readonly ConcurrentDictionary<Type, RowFieldsBase> byType;

        public DefaultRowFieldsProvider(IAnnotationTypeRegistry annotationRegistry = null, 
            IConnectionStrings connectionStrings = null)
        {
            this.annotationRegistry = annotationRegistry;
            this.connectionStrings = connectionStrings;
            byType = new ConcurrentDictionary<Type, RowFieldsBase>();
        }

        public RowFieldsBase Resolve(Type fieldsType)
        {
            return byType.GetOrAdd(fieldsType, CreateType);
        }

        private RowFieldsBase CreateType(Type fieldsType)
        {
            var fields = (RowFieldsBase)Activator.CreateInstance(fieldsType);
            IAnnotatedType annotations = null;
            if (annotationRegistry != null)
            {
                annotations = annotationRegistry.GetAnnotationTypesFor(fieldsType)
                    .GetAnnotatedType();
            }

            var dialect = connectionStrings?.TryGetConnectionString(fields.ConnectionKey)?
                .Dialect ?? SqlSettings.DefaultDialect;
            fields.Initialize(annotations, dialect);
            return fields;
        }
    }
}