using Microsoft.Extensions.DependencyInjection;
using Serenity.Reflection;
using System;
using System.Collections.Concurrent;

namespace Serenity.Data
{
    public class DefaultRowFieldsProvider : IRowFieldsProvider
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ConcurrentDictionary<Type, RowFieldsBase> byType;

        public DefaultRowFieldsProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public RowFieldsBase Resolve(Type fieldsType)
        {
            return byType.GetOrAdd(fieldsType, CreateType);
        }

        private RowFieldsBase CreateType(Type fieldsType)
        {
            var annotationRegistry = serviceProvider.GetService<IAnnotationTypeRegistry>();
            var connectionStrings = serviceProvider.GetService<IConnectionStrings>();
            var fields = (RowFieldsBase)ActivatorUtilities.CreateInstance(serviceProvider, fieldsType);

            IAnnotatedType annotations = null;
            if (annotationRegistry != null &&
                fieldsType.IsNested &&
                typeof(IRow).IsAssignableFrom(fieldsType.DeclaringType))
            {
                annotations = annotationRegistry.GetAnnotationTypesFor(fieldsType.DeclaringType)
                    .GetAnnotatedType();
            }

            var dialect = connectionStrings?.TryGetConnectionString(fields.ConnectionKey)?
                .Dialect ?? SqlSettings.DefaultDialect;

            fields.Initialize(annotations, dialect);

            return fields;
        }
    }
}