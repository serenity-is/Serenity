using Serenity.CodeGenerator;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Serenity.Tests.CodeGenerator
{
    public class RestoreCommandTests
    {
        [Fact]
        public void Throws_ArgumentNull_When_FileSystem_IsNull()
        {
            var fileSystem = new MockFileSystem();
            var projectSystem = new MockBuildProjectSystem(fileSystem);
            Assert.Throws<ArgumentNullException>(() => new RestoreCommand(null, projectSystem));
        }

        [Fact]
        public void Throws_ArgumentNull_When_ProjectSystem_IsNull()
        {
            var fileSystem = new MockFileSystem();
            var projectSystem = new MockBuildProjectSystem(fileSystem);
            Assert.Throws<ArgumentNullException>(() => new RestoreCommand(fileSystem, null));
        }

        [Fact]
        public void Throws_ArgumentNull_When_CsProj_IsNull()
        {
            var fileSystem = new MockFileSystem();
            var projectSystem = new MockBuildProjectSystem(fileSystem);
            var command = new RestoreCommand(fileSystem, projectSystem);
            Assert.Throws<ArgumentNullException>(() => command.Run(null));
        }

        [Fact]
        public void Returns_ProjectNotFound_ExitCode_When_CsProj_Not_Found()
        {
            var fileSystem = new MockFileSystem();
            var projectSystem = new MockBuildProjectSystem(fileSystem);
            var command = new RestoreCommand(fileSystem, projectSystem);
            var exitCode = command.Run("nonexisting.csproj");
            Assert.Equal(ExitCodes.ProjectNotFound, exitCode);
        }

        [Fact]
        public void Returns_Cant_Determine_Packages_Dir_When_Nuget_Packages_Dir_Not_Found()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                ["Test.csproj"] = "A"
            });

            var projectSystem = new MockBuildProjectSystem(fileSystem);
            var command = new RestoreCommand(fileSystem, projectSystem);
            var exitCode = command.Run("Test.csproj");
            Assert.Equal(ExitCodes.CantDeterminePackagesDir, exitCode);
        }
    }
}
