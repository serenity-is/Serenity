using Serenity.Reflection;
using System;
using System.Collections.Concurrent;

namespace Serenity.Data
{
    public class DefaultRowFieldsProvider : IRowFieldsProvider
    {
        private readonly IAnnotationTypeRegistry annotationRegistry;
        private readonly ConcurrentDictionary<Type, RowFieldsBase> byType;

        public DefaultRowFieldsProvider(IAnnotationTypeRegistry annotationRegistry = null)
        {
            this.annotationRegistry = annotationRegistry;
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
            fields.Initialize(annotations);
            return fields;
        }
    }
}