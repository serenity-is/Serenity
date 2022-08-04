using Serenity.CodeGeneration;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        private static ServerTypingsGenerator CreateGenerator()
        {
            var generator = new ServerTypingsGenerator(new PhysicalGeneratorFileSystem(),
                typeof(ServerTypingsGeneratorTests).Assembly.Location);
            generator.RootNamespaces.Add("ServerTypingsTest");
            return generator;
        }
    }
}
