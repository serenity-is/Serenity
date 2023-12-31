namespace Serenity.TypeScript;

internal class NamespaceDeclaration(NodeArray<IModifierLike> modifiers, Identifier name, IModuleBody body, NodeFlags flags)
    : ModuleDeclaration(modifiers, name, body, flags), INamespaceBody
{
}
