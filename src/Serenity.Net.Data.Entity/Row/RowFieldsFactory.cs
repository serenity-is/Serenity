using Serenity.Reflection;
using System;
using System.Collections.Concurrent;

namespace Serenity.Data
{
    public class RowFieldsFactory : IRowFieldsFactory
    {
        private readonly IAnnotationTypeRegistry annotationRegistry;
        private readonly ConcurrentDictionary<Type, RowFieldsBase> byType;

        private static object sync = new object();
        private static IRowFieldsFactory current;

        public static IRowFieldsFactory Current
        {
            get
            {
                lock (sync)
                    return current;
            }
        }

        public static IRowFieldsFactory SetCurrent(IRowFieldsFactory factory)
        {
            lock (sync)
            {
                var old = current;
                current = factory;
                return old;
            }
        }

        /// <summary>
        /// Runs operation in locked context, useful for testing only
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="factory"></param>
        public static void Scoped(Action operation, IRowFieldsFactory factory)
        {
            lock (sync)
            {
                var old = current;
                try
                {
                    current = factory;
                    operation();
                }
                finally
                {
                    current = old;
                }
            }
        }

        static RowFieldsFactory()
        {
            current = new RowFieldsFactory(annotationRegistry: null);
        }

        public RowFieldsFactory(IAnnotationTypeRegistry annotationRegistry = null)
        {
            this.annotationRegistry = annotationRegistry;
            byType = new ConcurrentDictionary<Type, RowFieldsBase>();
        }

        public RowFieldsBase GetFields(Type type)
        {
            return byType.GetOrAdd(type, (t) =>
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