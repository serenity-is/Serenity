#if ISSOURCEGENERATOR
using Microsoft.CodeAnalysis.CSharp.Syntax;
#endif

namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    private static string? GetAttributeKeyViaConstuctorArgument(CustomAttribute attr)
    {
        if (attr.ConstructorArguments() is { Count: 1 } args &&
            args[0] is { } arg &&
            arg.Value is string value &&
            arg.Type?.FullNameOf() == "System.String")
            return value;

        return null;
    }

    private static string? GetAttributeKeyViaKeyConstant(TypeReference attributeType)
    {
        return attributeType.Resolve().FieldsOf().FirstOrDefault(x =>
            x.IsStatic &&
            x.IsPublic() &&
            x.Name == "Key" &&
            x.HasConstant() &&
            x.Constant() is string &&
            x.DeclaringType().FullNameOf() == attributeType.FullNameOf())?.Constant() as string;
    }

    private string? GetAttributeKeyViaBaseCtorCall(TypeReference attributeType)
    {
#if ISSOURCEGENERATOR
        foreach (var constructor in attributeType.Resolve().MethodsOf().Where(x => x.IsConstructor()))
        {
            foreach (var syntaxReference in constructor.DeclaringSyntaxReferences)
            {
                if (syntaxReference.GetSyntax(cancellationToken) is not ConstructorDeclarationSyntax
                    { Initializer: { } initializer } constructorSyntax)
                    continue;

                var semanticModel = Compilation.GetSemanticModel(constructorSyntax.SyntaxTree);
                foreach (var argument in initializer.ArgumentList.Arguments.Reverse())
                {
                    var constant = semanticModel.GetConstantValue(argument.Expression, cancellationToken);
                    if (constant.HasValue && constant.Value is string key)
                        return key;
                }
            }
        }
#else
        if (attributeType.Resolve().MethodsOf()
                    .Where(x => x.IsConstructor())
                    .SelectMany(m => m.Body.Instructions
                        .Where(i => i.OpCode == Mono.Cecil.Cil.OpCodes.Call &&
                            (i.Operand is Mono.Cecil.MethodReference) &&
                            (i.Operand as Mono.Cecil.MethodReference)!.Resolve().IsConstructor &&
                            i.Previous.OpCode == Mono.Cecil.Cil.OpCodes.Ldstr &&
                            i.Previous.Operand is string)
                        .Select(x => x.Previous.Operand as string)).FirstOrDefault() is string key)
            return key;
#endif
        return null;
    }
}