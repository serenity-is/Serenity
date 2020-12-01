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

        internal DefaultRowFieldsProvider()
        {
            byType = new ConcurrentDictionary<Type, RowFieldsBase>();
        }

        public DefaultRowFieldsProvider(IServiceProvider serviceProvider)
            : this()
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public RowFieldsBase Resolve(Type fieldsType)
        {
            return byType.GetOrAdd(fieldsType, CreateType);
        }

        private RowFieldsBase CreateType(Type fieldsType)
        {
            RowFieldsBase fields;
            if (serviceProvider == null)
            {
                fields = (RowFieldsBase)Activator.CreateInstance(fieldsType);
                fields.Initialize(annotations: null, SqlSettings.DefaultDialect);
                return fields;
            }

            var annotationRegistry = serviceProvider.GetService<IAnnotationTypeRegistry>();
            var connectionStrings = serviceProvider.GetService<IConnectionStrings>();
            fields = (RowFieldsBase)ActivatorUtilities.CreateInstance(serviceProvider, fieldsType);

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