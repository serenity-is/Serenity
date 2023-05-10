using Mono.Cecil;
using Serenity.CodeGeneration;

namespace Serenity.Tests.CodeGenerator;

public partial class ServerTypingsGeneratorTests
{
    public static bool InTestNamespace(TypeDefinition type)
    {
        return type.Namespace == "ServerTypingsTest" ||
            type.Namespace.StartsWith("ServerTypingsTest.", StringComparison.Ordinal);
    }

    private static ServerTypingsGenerator CreateGenerator(params Type[] types)
    {
        var generator = new ServerTypingsGenerator(new MockGeneratorFileSystem(),
            typeof(ServerTypingsGeneratorTests).Assembly.Location);
        generator.RootNamespaces.Add("ServerTypingsTest");
        generator.TypeFilter = type =>
        {
            return types.Length == 0 ? InTestNamespace(type) :
                types.Any(x => type.FullName == x.FullName);
        };
        return generator;
    }

    private static ServerTypingsGenerator CreateGeneratorModules(params Type[] types)
    {
        var generator = new ServerTypingsGenerator(new MockGeneratorFileSystem(),
            typeof(ServerTypingsGeneratorTests).Assembly.Location);
        generator.RootNamespaces.Add("ServerTypingsTest");
        generator.TypeFilter = type =>
        {
            return types.Length == 0 ? InTestNamespace(type) :
                types.Any(x => type.FullName == x.FullName);
        };
        generator.ModuleTypings = true;
        generator.ModuleReExports = false;
        generator.NamespaceTypings = false;
        return generator;
    }

}
