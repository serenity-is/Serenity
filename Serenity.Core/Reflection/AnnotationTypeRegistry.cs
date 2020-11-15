using Serenity.ComponentModel;
using Serenity.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.Reflection
{
    /// <summary>
    /// Default annotation type registry
    /// </summary>
    /// <seealso cref="Serenity.Reflection.IAnnotationTypeRegistry" />
    public class AnnotationTypeRegistry : IAnnotationTypeRegistry
    {
        private Type[] annotationTypes;

        /// <summary>
        /// Gets the annotation types.
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<Type> GetAnnotationTypes()
        {
            var annotationTypes = this.annotationTypes;
            if (annotationTypes != null)
                return annotationTypes;

            var list = new List<Type>();

            foreach (var assembly in ExtensibilityHelper.SelfAssemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetCustomAttribute<AnnotationTypeAttribute>() != null)
                        list.Add(type);
                }
            }

            this.annotationTypes = annotationTypes = list.ToArray();
            return annotationTypes;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.annotationTypes = null;
        }

        /// <summary>
        /// Gets the annotation types for given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IEnumerable<Type> GetAnnotationTypesFor(Type type)
        {
            var list = new List<Type>();

            foreach (var annotationType in GetAnnotationTypes())
            {
                var annotationMatch = false;

                foreach (var attr in annotationType.GetCustomAttributes<AnnotationTypeAttribute>())
                {
                    if (attr.AnnotatedType.IsSubclassOf(typeof(Attribute)))
                    {
                        if (type.GetCustomAttribute(attr.AnnotatedType) == null)
                            continue;
                    }
                    else if (attr.Inherited || attr.AnnotatedType.IsInterface)
                    {
                        if (!attr.AnnotatedType.IsAssignableFrom(type))
                            continue;
                    }
                    else if (type != attr.AnnotatedType)
                        continue;

                    if (attr.Namespaces != null && attr.Namespaces.Length > 0)
                    {
                        bool namespaceMatch = false;
                        foreach (var ns in attr.Namespaces)
                        {
                            if (type.Namespace == ns)
                            {
                                namespaceMatch = true;
                                break;
                            }

                            if (ns.Length > 2 &&
                                ns.EndsWith(".*", StringComparison.OrdinalIgnoreCase) &&
                                type.Namespace != null)
                            {
                                if (type.Namespace == ns[0..^2] ||
                                    type.Namespace.StartsWith(ns[0..^1], StringComparison.OrdinalIgnoreCase))
                                {
                                    namespaceMatch = true;
                                    break;
                                }
                            }
                        }

                        if (!namespaceMatch)
                            continue;
                    }

                    if (attr.Properties != null &&
                        attr.Properties.Length > 0 &&
                        attr.Properties.Any(name => type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public) == null))
                        continue;

                    annotationMatch = true;
                    break;
                }

                if (annotationMatch)
                    list.Add(annotationType);
            }

            return list;
        }
    }

}