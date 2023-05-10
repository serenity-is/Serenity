namespace Serenity.Reflection;

/// <summary>
/// Default annotation type registry
/// </summary>
/// <seealso cref="IAnnotationTypeRegistry" />
public class AnnotationTypeRegistry : IAnnotationTypeRegistry
{
    private readonly IEnumerable<Type> annotationTypes;

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="typeSource">Type source</param>
    public AnnotationTypeRegistry(ITypeSource typeSource)
    {
        annotationTypes = (typeSource ?? throw new ArgumentNullException(nameof(typeSource)))
            .GetTypesWithAttribute(typeof(AnnotationTypeAttribute));
    }

    /// <summary>
    /// Gets the annotation types for given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public IEnumerable<Type> GetAnnotationTypesFor(Type type)
    {
        var list = new List<Type>();

        foreach (var annotationType in annotationTypes)
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