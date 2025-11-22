
namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    protected virtual void EnqueueInitialTypes()
    {
        var visitedForAnnotations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

#if ISSOURCEGENERATOR
        if (!string.IsNullOrEmpty(Compilation.AssemblyName))
            assemblyNames.Add(Compilation.AssemblyName);

        var types = Compilation.GetSymbolsWithName(s => true, SymbolFilter.Type).OfType<TypeReference>().ToArray();

        var collector = new ExportedTypesCollector(cancellationToken);
        collector.VisitNamespace(Compilation.GlobalNamespace);
        foreach (var expType in collector.GetPublicTypes())
            ScanAnnotationTypeAttributes(expType);

        EnqueueInitialTypesFrom(types);
#else
        foreach (var assembly in Assemblies)
        {
            foreach (var module in assembly.Modules)
            {
                TypeDefinition[] asmModuleTypes;
                try
                {
                    var typeFilter = TypeFilter ?? (type => true);
                    asmModuleTypes = module.Types
                        .Where(typeFilter)
                        .ToArray();
                }
                catch
                {
                    // skip assemblies that doesn't like to list its types (e.g. some SignalR reported in #2340)
                    continue;
                }

                if (module.HasAssemblyReferences)
                {
                    foreach (var refAsm in module.AssemblyReferences)
                    {
                        if (!visitedForAnnotations.Contains(refAsm.Name))
                        {
                            visitedForAnnotations.Add(refAsm.Name);

                            if (SkipPackages.ForAnnotations(refAsm.Name))
                                continue;

                            if (Assemblies.Any(x => string.Equals(x.Name.Name,
                                refAsm.Name, StringComparison.OrdinalIgnoreCase)))
                                continue;

                            try
                            {

                                var refDef = module.AssemblyResolver.Resolve(refAsm);
                                if (refDef != null)
                                {
                                    foreach (var refMod in refDef.Modules)
                                    {
                                        foreach (var refType in refMod.GetTypes())
                                        {
                                            ScanAnnotationTypeAttributes(refType);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                EnqueueInitialTypesFrom(asmModuleTypes);
            }
        }
#endif
    }

    protected virtual bool ShouldExcludeInitialType(TypeDefinition type)
    {
        if (type.IsAbstract &&
             TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "ScriptIncludeAttribute") == null)
            return true;

#if ISSOURCEGENERATOR
        if (type.ContainingType != null && type.DeclaredAccessibility !=  
            Microsoft.CodeAnalysis.Accessibility.Public)
            return true;
#endif

        if (TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "TransformIgnoreAttribute") != null)
            return true;

        return false;
    }

    private const string MvcNamespace = "Microsoft.AspNetCore.Mvc";
    private const string ControllerBase = "ControllerBase";
    private const string ServicesNS = "Serenity.Services";
    private const string ServiceEndpoint = "ServiceEndpoint";

    private bool IsServiceEndpoint(TypeReference[] baseTypes, TypeDefinition type)
    {
        return TypingsUtils.Contains(baseTypes, ServicesNS, ServiceEndpoint) ||
            (TypingsUtils.Contains(baseTypes, MvcNamespace, ControllerBase) && // controllers in .Endpoints namespace
             type.NamespaceOf()?.EndsWith(".Endpoints", StringComparison.Ordinal) == true);
    }

    private CustomAttribute GetColumnsScriptAttribute(TypeDefinition type)
    {
        return TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "ColumnsScriptAttribute", emptyTypes);
    }

    private CustomAttribute GetFormScriptAttribute(TypeDefinition type)
    {
        return TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "FormScriptAttribute", emptyTypes);
    }

    private CustomAttribute GetNestedPermissionKeysAttribute(TypeDefinition type)
    {
        return TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "NestedPermissionKeysAttribute", emptyTypes);
    }

    protected static bool IsRowType(TypeReference[] baseTypes, TypeDefinition type)
    {
        return TypingsUtils.Contains(baseTypes, "Serenity.Data", "Row") ||
            TypingsUtils.Contains(baseTypes, "Serenity.Data", "Row`1");
    }

    protected static bool IsServiceRequest(TypeReference[] baseTypes, TypeDefinition type)
    {
        return TypingsUtils.Contains(baseTypes, ServicesNS, "ServiceRequest");
    }
    
    protected static bool IsServiceResponse(TypeReference[] baseTypes, TypeDefinition type)
    {
        return TypingsUtils.Contains(baseTypes, ServicesNS, "ServiceResponse");
    }   

    protected virtual bool ShouldIncludeInitialType(TypeDefinition type)
    {
        var baseTypes = TypingsUtils.EnumerateBaseClasses(type).ToArray();

        return IsServiceRequest(baseTypes, type) ||
            IsServiceResponse(baseTypes, type) ||
            IsRowType(baseTypes, type) ||
            IsServiceEndpoint(baseTypes, type) ||
            GetColumnsScriptAttribute(type) != null ||
            GetFormScriptAttribute(type) != null ||
            GetNestedPermissionKeysAttribute(type) != null ||
            TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "ScriptIncludeAttribute", emptyTypes) != null;
    }

    protected virtual void EnqueueInitialTypesFrom(TypeDefinition[] types)
    {
        foreach (var fromType in types)
        {
            PreVisitType(fromType);
            ScanAnnotationTypeAttributes(fromType);

            if (ShouldExcludeInitialType(fromType))
                continue;

            if (ShouldIncludeInitialType(fromType))
            {
                EnqueueType(fromType);
                continue;
            }

            if (TypingsUtils.GetAttr(fromType, "Serenity.ComponentModel", "LookupScriptAttribute", emptyTypes) != null)
                lookupScripts.Add(fromType);
        }
    }

    protected virtual bool EnqueueType(TypeDefinition type)
    {
        if (visited.Contains(type.FullNameOf()))
            return false;

        visited.Add(type.FullNameOf());
        generateQueue.Enqueue(type);
        return true;
    }

    private void EnqueueMemberType(TypeReference memberType)
    {
        if (memberType == null)
            return;

        var enumType = TypingsUtils.GetEnumTypeFrom(memberType);
        if (enumType != null)
            EnqueueType(enumType);
    }

    protected virtual void EnqueueTypeMembers(TypeDefinition type)
    {
        foreach (var field in type.FieldsOf())
            if (!field.IsStatic && field.IsPublic())
                EnqueueMemberType(field.FieldType());

        foreach (var property in type.PropertiesOf())
        {
            if (!TypingsUtils.IsPublicInstanceProperty(property))
                continue;

            EnqueueMemberType(property.PropertyType());
        }
    }

}