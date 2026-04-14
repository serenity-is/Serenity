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
            annotationsByName = [];

            foreach (var annotationType in annotationTypes)
            {
                foreach (var annotationProperty in annotationType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!annotationsByName.TryGetValue(annotationProperty.Name, out List<PropertyInfo> list))
                        annotationsByName[annotationProperty.Name] = list = [];

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

    private class AnnotatedProperty(PropertyInfo property, IEnumerable<PropertyInfo> annotations) : IPropertyInfo
    {
        private readonly WrappedProperty property = new(property);
        private readonly IEnumerable<WrappedProperty> annotations = [.. annotations.Select(a => new WrappedProperty(a))];

        public string Name => property.Name;

        public Type PropertyType => property.PropertyType;

        public TAttr? GetAttribute<TAttr>(AttributeOrigin origin) where TAttr : Attribute
        {
            var attr = property.GetAttribute<TAttr>(origin);
            if (attr != null)
                return attr;

            if (origin.HasFlag(AttributeOrigin.Annotation))
            {
                foreach (var annotation in annotations)
                {
                    attr = annotation.GetAttribute<TAttr>();
                    if (attr != null)
                        return attr;
                }
            }

            return null;
        }

        public IEnumerable<TAttr> GetAttributes<TAttr>(AttributeOrigin origin) where TAttr : Attribute
        {
            foreach (var attr in property.GetAttributes<TAttr>(origin))
                yield return attr;

            if (!origin.HasFlag(AttributeOrigin.Annotation))
                yield break;

            foreach (var annotation in annotations)
            {
                foreach (var attr in annotation.GetAttributes<TAttr>(origin))
                    yield return attr;
            }

            yield break;
        }
    }
}