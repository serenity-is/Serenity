namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    protected class AnnotationTypeInfo
    {
        public TypeDefinition AnnotationType { get; private set; }
        public List<AttributeInfo> Attributes { get; private set; }
        public Dictionary<string, PropertyDefinition> PropertyByName { get; private set; }

        public AnnotationTypeInfo(TypeDefinition annotationType)
        {
            AnnotationType = annotationType;
            PropertyByName = [];
            Attributes = [];

            foreach (var property in annotationType.PropertiesOf())
                if (TypingsUtils.IsPublicInstanceProperty(property))
                    PropertyByName[property.Name] = property;
        }

        public class AttributeInfo
        {
            public TypeDefinition AnnotatedType { get; set; }
            public bool Inherited { get; set; }
            public string[] Namespaces { get; set; }
            public string[] Properties { get; set; }
        }
    }

    protected List<AnnotationTypeInfo> GetAnnotationTypesFor(TypeDefinition type)
    {
        var list = new List<AnnotationTypeInfo>();
        TypeReference[] baseClasses = null;
        foreach (var annotationType in annotationTypes)
        {
            var annotationMatch = false;

            foreach (var attr in annotationType.Attributes)
            {
                baseClasses ??= TypingsUtils.EnumerateBaseClasses(type).ToArray();

                if (TypingsUtils.IsOrSubClassOf(attr.AnnotatedType, "System", "Attribute"))
                {
                    if (TypingsUtils.GetAttr(type, attr.AnnotatedType.NamespaceOf(),
                        attr.AnnotatedType.Name, baseClasses) == null)
                        continue;
                }
                else if (attr.Inherited ||
#if ISSOURCEGENERATOR
                    attr.AnnotatedType.TypeKind == TypeKind.Interface)
#else
                    attr.AnnotatedType.IsInterface)
#endif
                {
                    if (!TypingsUtils.IsAssignableFrom(attr.AnnotatedType, type))
                        continue;
                }
#if ISSOURCEGENERATOR
                else if (!type.Equals(attr.AnnotatedType, SymbolEqualityComparer.Default))
#else
                else if (type != attr.AnnotatedType)
#endif
                    continue;

                if (attr.Namespaces != null && attr.Namespaces.Length > 0)
                {
                    bool namespaceMatch = false;
                    foreach (var ns in attr.Namespaces)
                    {
                        if (type.NamespaceOf() == ns)
                        {
                            namespaceMatch = true;
                            break;
                        }

                        if (ns.Length > 2 &&
                            ns.EndsWith(".*", StringComparison.OrdinalIgnoreCase) &&
                            type.NamespaceOf() != null)
                        {
                            if (type.NamespaceOf() == ns[0..^2] ||
                                type.NamespaceOf().StartsWith(ns[0..^1], StringComparison.OrdinalIgnoreCase))
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
                    attr.Properties.Any(name => !type.PropertiesOf().Any(p =>
                        p.Name == name && TypingsUtils.IsPublicInstanceProperty(p))))
                    continue;

                annotationMatch = true;
                break;
            }

            if (annotationMatch)
                list.Add(annotationType);
        }

        return list;
    }


    protected virtual void ScanAnnotationTypeAttributes(TypeDefinition fromType)
    {
        var annotationTypeAttrs = TypingsUtils.GetAttrs(
            fromType.GetAttributes(),
            "Serenity.ComponentModel", "AnnotationTypeAttribute", null);

        if (!annotationTypeAttrs.Any())
            return;

        var typeInfo = new AnnotationTypeInfo(fromType);
        foreach (var attr in annotationTypeAttrs)
        {
            var attrInfo = new AnnotationTypeInfo.AttributeInfo();
            if (attr.ConstructorArguments()?.FirstOrDefault(x =>
                x.Type.FullNameOf() == "System.Type").Value is not TypeReference annotatedType)
                continue;

            attrInfo.AnnotatedType = annotatedType.Resolve();

            if (attr.HasProperties())
            {
                attrInfo.Inherited = attr.GetProperties().FirstOrDefault(x =>
                    x.Name() == "Inherited").ArgumentValue() as bool? ?? true;

                attrInfo.Namespaces = attr.GetProperties().FirstOrDefault(x =>
                    x.Name() == "Namespaces").ArgumentValue() as string[];

                attrInfo.Properties = attr.GetProperties().FirstOrDefault(x =>
                    x.Name() == "Properties").ArgumentValue() as string[];
            }
            else
                attrInfo.Inherited = true;
            typeInfo.Attributes.Add(attrInfo);
        }

        if (typeInfo.Attributes.Count > 0)
            annotationTypes.Add(typeInfo);
    }
}
