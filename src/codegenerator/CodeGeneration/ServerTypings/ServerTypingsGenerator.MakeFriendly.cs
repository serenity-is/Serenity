namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    protected virtual string MakeFriendlyName(TypeReference type, string codeNamespace)
    {
        if (type.IsGenericInstance())
        {
            var name = type.Name;
            var idx = name.IndexOf('`', StringComparison.Ordinal);
            if (idx >= 0)
                name = name[..idx];

            sb.Append(name);
            sb.Append('<');

            int i = 0;
#if ISSOURCEGENERATOR
            var nt = (INamedTypeSymbol)type;
            foreach (var argument in nt.TypeArguments)
#else
            foreach (var argument in (type as GenericInstanceType).GenericArguments)
#endif
            {
                if (i++ > 0)
                    sb.Append(", ");

                HandleMemberType(argument, codeNamespace);
            }

            sb.Append('>');

            return name + "`" +
#if ISSOURCEGENERATOR
                nt.TypeArguments.Length;
#else
                (type as GenericInstanceType).GenericArguments.Count;
#endif
        }
#if ISSOURCEGENERATOR
        else if (type is INamedTypeSymbol nt2 && nt2.TypeParameters.Length > 0)
#else
        else if (type.HasGenericParameters)
#endif
        {
            var name = type.Name;
            var idx = name.IndexOf('`', StringComparison.Ordinal);
            if (idx >= 0)
                name = name[..idx];

            sb.Append(name);
            sb.Append('<');

            int i = 0;
#if ISSOURCEGENERATOR
            foreach (var argument in nt2.TypeParameters)
#else
            foreach (var argument in type.GenericParameters)
#endif
            {
                if (i++ > 0)
                    sb.Append(", ");

                sb.Append(argument.Name);
            }

            sb.Append('>');

            return name + "`" +
#if ISSOURCEGENERATOR
                nt2.TypeParameters.Length;
#else
                type.GenericParameters.Count;
#endif
        }
        else
        {
            sb.Append(type.Name);
            return type.Name;
        }
    }

    public virtual string ShortenFullName(string ns, string name, string codeNamespace,
        string containingAssembly)
    {
        var nonGeneric = name;
        var genericIdx = nonGeneric.IndexOf('`', StringComparison.Ordinal);
        if (genericIdx >= 0)
            nonGeneric = nonGeneric[..genericIdx];

        if (string.IsNullOrEmpty(containingAssembly) ||
            !assemblyNames.Contains(containingAssembly))
        {
            ExternalType moduleType = TryFindModuleType(ns?.Length > 0 ? (ns + "." + name) : name, containingAssembly);
            if (moduleType != null)
                return AddExternalImport(moduleType.Module, nonGeneric);
        }

        var filename = GetTypingFileNameFor(ns, nonGeneric);
        return AddModuleImport(filename, nonGeneric, external: false);
    }

    protected virtual void MakeFriendlyReference(TypeReference type, string codeNamespace)
    {
        var fullName = ShortenFullName(ScriptNamespaceFor(type), type.Name, codeNamespace, GetAssemblyNameFor(type));

        if (type.IsGenericInstance())
        {
            var idx = fullName.IndexOf('`', StringComparison.Ordinal);
            if (idx >= 0)
                fullName = fullName[..idx];

            sb.Append(fullName);
            sb.Append('<');

            int i = 0;
#if ISSOURCEGENERATOR
            foreach (var argument in (type as INamedTypeSymbol).TypeArguments)
#else
            foreach (var argument in (type as GenericInstanceType).GenericArguments)
#endif
            {
                if (i++ > 0)
                    sb.Append(", ");

                HandleMemberType(argument, codeNamespace);
            }

            sb.Append('>');
            return;
        }

        sb.Append(fullName);
    }

    protected virtual string ScriptNamespaceFor(TypeReference type)
    {
        var ns = type.NamespaceOf() ?? "";
        if (string.IsNullOrEmpty(ns) && type.IsNested())
            ns = type.DeclaringType().NamespaceOf();

        if (ns.EndsWith(".Entities", StringComparison.Ordinal))
            return ns[..^".Entities".Length];

        if (ns.EndsWith(".Endpoints", StringComparison.Ordinal))
            return ns[..^".Endpoints".Length];

        if (ns.EndsWith(".Forms", StringComparison.Ordinal))
            return ns[..^".Forms".Length];

        if (ns.EndsWith(".Columns", StringComparison.Ordinal))
            return ns[..^".Columns".Length];

        return ns;
    }
}