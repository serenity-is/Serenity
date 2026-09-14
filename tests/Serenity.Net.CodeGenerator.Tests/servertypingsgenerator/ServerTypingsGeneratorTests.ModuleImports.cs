namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void DetermineModulesRoot_Uses_Modules_Folder_When_It_Has_Files()
        {
            var fs = new MockFileSystem();
            fs.CreateDirectory("/app/Modules");
            fs.WriteAllText("/app/Modules/Some.ts", "");
            var generator = CreateGenerator();

            var modulesDir = generator.DetermineModulesRoot(fs, "/app/My.Web.csproj", "My");

            Assert.EndsWith("Modules", modulesDir.Replace('\\', '/'));
            Assert.Equal("Modules", generator.ModulesPathFolder);
        }

        [Fact]
        public void DetermineModulesRoot_Uses_ProjectFileName_Folder_As_Fallback()
        {
            var fs = new MockFileSystem();
            fs.CreateDirectory("/app/My.Web.csproj");
            var generator = CreateGenerator();

            var modulesDir = generator.DetermineModulesRoot(fs, "/app/My.Web.csproj", "My");

            Assert.EndsWith("My.Web.csproj", modulesDir.Replace('\\', '/'));
            Assert.Equal("My.Web.csproj", generator.ModulesPathFolder);
        }

        [Fact]
        public void DetermineModulesRoot_Uses_RootNamespace_Folder_As_Fallback()
        {
            var fs = new MockFileSystem();
            fs.CreateDirectory("/app/My");
            var generator = CreateGenerator();

            var modulesDir = generator.DetermineModulesRoot(fs, "/app/My.Web.csproj", "My");

            Assert.EndsWith("My", modulesDir.Replace('\\', '/'));
            Assert.Equal("My", generator.ModulesPathFolder);
        }
    }
}
