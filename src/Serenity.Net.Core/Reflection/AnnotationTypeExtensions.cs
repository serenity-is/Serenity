namespace Serenity.Reflection;

/// <summary>
/// Extension methods for annotation types
/// </summary>
public static class AnnotationTypeExtensions
{
    /// <summary>
    /// Gets the annotated type information for given type.
    /// </summary>
    /// <param name="annotationTypes">Annotation types</param>
    /// <returns>
    /// Annotated type information
    /// </returns>
    public static IAnnotatedType GetAnnotatedType(this IEnumerable<Type> annotationTypes)
    {
        return new AnnotatedType(annotationTypes);
    }

    private class AnnotatedType : IAnnotatedType
    {
        private readonly Dictionary<string, List<PropertyInfo>> annotationsByName;

        public AnnotatedType(IEnumerable<Type> annotationTypes)
        {
            annotationsByName = new Dictionary<string, List<PropertyInfo>>();

            foreach (var annotationType in annotationTypes)
            {
                foreach (var annotationProperty in annotationType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!annotationsByName.TryGetValue(annotationProperty.Name, out List<PropertyInfo> list))
                        annotationsByName[annotationProperty.Name] = list = new List<PropertyInfo>();

                    list.Add(annotationProperty);
                }
            }
        }

        public IPropertyInfo GetAnnotatedProperty(PropertyInfo property)
        {
            if (!annotationsByName.TryGetValue(property.Name, out List<PropertyInfo> list))
                return new WrappedProperty(property);

            return new AnnotatedProperty(property, list);
        }
    }

    private class AnnotatedProperty : IPropertyInfo
    {
        private readonly PropertyInfo property;
        private readonly IEnumerable<PropertyInfo> annotations;

        public AnnotatedProperty(PropertyInfo property, IEnumerable<PropertyInfo> annotations)
        {
            this.property = property;
            this.annotations = annotations;
        }

        public string Name => property.Name;

        public Type PropertyType => property.PropertyType;

        public TAttr? GetAttribute<TAttr>() where TAttr : Attribute
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