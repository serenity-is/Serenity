using Serenity.CodeGeneration;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        private static ServerTypingsGenerator CreateGenerator()
        {
            var generator = new ServerTypingsGenerator(new MockGeneratorFileSystem(),
                typeof(ServerTypingsGeneratorTests).Assembly.Location);
            generator.RootNamespaces.Add("ServerTypingsTest");
            return generator;
        }

        private static ServerTypingsGenerator CreateGeneratorModules()
        {
            var generator = new ServerTypingsGenerator(new MockGeneratorFileSystem(),
                typeof(ServerTypingsGeneratorTests).Assembly.Location);
            generator.RootNamespaces.Add("ServerTypingsTest");
            generator.ModuleTypings = true;
            generator.NamespaceTypings = false;
            return generator;
        }

    }
}
