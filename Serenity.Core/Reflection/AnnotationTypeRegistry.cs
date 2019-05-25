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
                                if (type.Namespace == ns.Substring(0, ns.Length - 2) ||
                                    type.Namespace.StartsWith(ns.Substring(0, ns.Length - 1), StringComparison.OrdinalIgnoreCase))
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

        /// <summary>
        /// Gets the annotated type information for given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// Annotated type information
        /// </returns>
        public IAnnotatedType GetAnnotatedType(Type type)
        {
            return new AnnotatedType(type, GetAnnotationTypesFor(type));
        }

        private class AnnotatedType : IAnnotatedType
        {
            private Dictionary<string, List<PropertyInfo>> annotationsByName;

            public AnnotatedType(Type type, IEnumerable<Type> annotationTypes)
            {
                annotationsByName = new Dictionary<string, List<PropertyInfo>>();

                foreach (var annotationType in annotationTypes)
                {
                    foreach (var annotationProperty in annotationType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        List<PropertyInfo> list;
                        if (!annotationsByName.TryGetValue(annotationProperty.Name, out list))
                            annotationsByName[annotationProperty.Name] = list = new List<PropertyInfo>();

                        list.Add(annotationProperty);
                    }
                }
            }

            public IPropertyInfo GetAnnotatedProperty(PropertyInfo property)
            {
                List<PropertyInfo> list;
                if (!annotationsByName.TryGetValue(property.Name, out list))
                    return new WrappedProperty(property);

                return new AnnotatedProperty(property, list);
            }
        }

        private class AnnotatedProperty : IPropertyInfo
        {
            private PropertyInfo property;
            private IEnumerable<PropertyInfo> annotations;

            public AnnotatedProperty(PropertyInfo property, IEnumerable<PropertyInfo> annotations)
            {
                this.property = property;
                this.annotations = annotations;
            }

            public string Name
            {
                get { return property.Name; }
            }

            public Type PropertyType
            {
                get { return property.PropertyType; }
            }

            public TAttr GetAttribute<TAttr>() where TAttr : Attribute
            {
                var attr = property.GetCustomAttribute<TAttr>();
                if (attr != null)
                    return attr;

                foreach (var annotation in annotations)
                {
                    attr = annotation.GetCustomAttribute<TAttr>();
                    if (attr != null)
                        return attr;
                }

                return null;
            }

            public IEnumerable<TAttr> GetAttributes<TAttr>() where TAttr : Attribute
            {
                foreach (var attr in property.GetCustomAttributes<TAttr>())
                    yield return attr;

                foreach (var annotation in annotations)
                {
                    foreach (var attr in annotation.GetCustomAttributes<TAttr>())
                        yield return attr;
                }

                yield break;
            }
        }
    }

}