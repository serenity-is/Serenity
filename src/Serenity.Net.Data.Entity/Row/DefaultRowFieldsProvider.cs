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
            return byType.GetOrAdd(fieldsType, (t) =>
            {
                var fields = (RowFieldsBase)Activator.CreateInstance(t);
                IAnnotatedType annotations = null;
                if (annotationRegistry != null)
                {
                    annotations = annotationRegistry.GetAnnotationTypesFor(t)
                        .GetAnnotatedType();
                }
                fields.Initialize(annotations);
                return fields;
            });
        }
    }
}