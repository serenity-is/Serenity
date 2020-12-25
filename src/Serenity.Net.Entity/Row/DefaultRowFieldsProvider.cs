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
        private readonly ConcurrentDictionary<(Type type, string alias), 
            RowFieldsBase> byTypeAndAlias;

        public DefaultRowFieldsProvider(IServiceProvider serviceProvider)
        {
            byType = new ConcurrentDictionary<Type, RowFieldsBase>();
            byTypeAndAlias = new ConcurrentDictionary<(Type, string), RowFieldsBase>();
            this.serviceProvider = serviceProvider ?? 
                throw new ArgumentNullException(nameof(serviceProvider));
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

            if (alias != null)
                fields.ReplaceAliasWith(alias);
            fields.LockAlias();

            return fields;
        }
    }
}