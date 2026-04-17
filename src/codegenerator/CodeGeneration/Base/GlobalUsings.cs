global using Serenity.Reflection;
#if ISSOURCEGENERATOR
global using CustomAttribute = Microsoft.CodeAnalysis.AttributeData;
global using FieldDefinition = Microsoft.CodeAnalysis.IFieldSymbol;
global using MethodDefinition = Microsoft.CodeAnalysis.IMethodSymbol;
global using PropertyDefinition = Microsoft.CodeAnalysis.IPropertySymbol;
global using TypeDefinition = Microsoft.CodeAnalysis.ITypeSymbol;
global using TypeReference = Microsoft.CodeAnalysis.ITypeSymbol;
global using GenericInstanceType = Microsoft.CodeAnalysis.INamedTypeSymbol;
global using MethodKind = Microsoft.CodeAnalysis.MethodKind;
global using TypeKind = Microsoft.CodeAnalysis.TypeKind;
global using INamedTypeSymbol = Microsoft.CodeAnalysis.INamedTypeSymbol;
global using SymbolEqualityComparer = Microsoft.CodeAnalysis.SymbolEqualityComparer;
global using SymbolFilter = Microsoft.CodeAnalysis.SymbolFilter;
#else
global using CustomAttribute = Mono.Cecil.CustomAttribute;
global using FieldDefinition = Mono.Cecil.FieldDefinition;
global using MethodDefinition = Mono.Cecil.MethodDefinition;
global using TypeReference = Mono.Cecil.TypeReference;
global using TypeDefinition = Mono.Cecil.TypeDefinition;
global using ParameterDefinition = Mono.Cecil.ParameterDefinition;
global using PropertyDefinition = Mono.Cecil.PropertyDefinition;
global using GenericInstanceType = Mono.Cecil.GenericInstanceType;
#endif