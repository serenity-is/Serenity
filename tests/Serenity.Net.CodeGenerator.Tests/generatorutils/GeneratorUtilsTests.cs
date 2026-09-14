namespace Serenity.Reflection;

public class GeneratorUtilsTests
{
    private static void AddFile(MockFileSystem fileSystem, string path, string content)
    {
        var directory = fileSystem.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            fileSystem.CreateDirectory(directory);
        fileSystem.WriteAllText(path, content);
    }

    private class TestAttribute : Attribute
    {
    }

    private class DerivedTestAttribute : TestAttribute
    {
    }

    private class BaseClass
    {
    }

    private class ChildClass : BaseClass
    {
    }

    [DerivedTest]
    private class DecoratedClass
    {
    }

    private class NotDecoratedClass
    {
    }

    private class GenericBase<T>
    {
    }

    private class DerivedFromGeneric : GenericBase<int>
    {
    }

    private class NonGeneric
    {
    }

    [Fact]
    public void IsEqualOrSubclassOf_ReturnsTrue_ForSameType()
    {
        Assert.True(GeneratorUtils.IsEqualOrSubclassOf(typeof(BaseClass), typeof(BaseClass).FullName));
    }

    [Fact]
    public void IsEqualOrSubclassOf_ReturnsTrue_ForSubclass()
    {
        Assert.True(GeneratorUtils.IsEqualOrSubclassOf(typeof(ChildClass), typeof(BaseClass).FullName));
    }

    [Fact]
    public void IsEqualOrSubclassOf_ReturnsFalse_ForUnrelatedType()
    {
        Assert.False(GeneratorUtils.IsEqualOrSubclassOf(typeof(ChildClass), typeof(string).FullName));
    }

    [Fact]
    public void IsSubclassOf_ReturnsFalse_ForNullType()
    {
        Assert.False(GeneratorUtils.IsSubclassOf(null, typeof(BaseClass).FullName));
    }

    [Fact]
    public void IsSubclassOf_ReturnsTrue_ForIndirectBase()
    {
        Assert.True(GeneratorUtils.IsSubclassOf(typeof(ChildClass), typeof(object).FullName));
    }

    [Fact]
    public void IsSubclassOf_ReturnsFalse_ForSameType()
    {
        Assert.False(GeneratorUtils.IsSubclassOf(typeof(BaseClass), typeof(BaseClass).FullName));
    }

    [Fact]
    public void GetAttribute_ReturnsNull_WhenNotDecorated()
    {
        Assert.Null(GeneratorUtils.GetAttribute(typeof(NotDecoratedClass), typeof(TestAttribute).FullName));
    }

    [Fact]
    public void GetAttribute_ReturnsDerivedAttribute()
    {
        var attribute = GeneratorUtils.GetAttribute(typeof(DecoratedClass), typeof(TestAttribute).FullName);

        Assert.NotNull(attribute);
        Assert.IsType<DerivedTestAttribute>(attribute);
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(int))]
    [InlineData(typeof(long))]
    [InlineData(typeof(short))]
    [InlineData(typeof(double))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(TimeSpan))]
    public void IsSimpleType_ReturnsTrue_ForSimpleTypes(Type type)
    {
        Assert.True(GeneratorUtils.IsSimpleType(type));
    }

    [Theory]
    [InlineData(typeof(object))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(int?))]
    [InlineData(typeof(decimal?))]
    public void IsSimpleType_ReturnsFalse_ForOtherTypes(Type type)
    {
        Assert.False(GeneratorUtils.IsSimpleType(type));
    }

    [Fact]
    public void GetFirstDerivedOfGenericType_ReturnsType_WhenItselfGeneric()
    {
        Assert.True(GeneratorUtils.GetFirstDerivedOfGenericType(
            typeof(List<int>), typeof(List<>), out var derivedType));
        Assert.Equal(typeof(List<int>), derivedType);
    }

    [Fact]
    public void GetFirstDerivedOfGenericType_ReturnsBase_WhenInheritedGeneric()
    {
        Assert.True(GeneratorUtils.GetFirstDerivedOfGenericType(
            typeof(DerivedFromGeneric), typeof(GenericBase<>), out var derivedType));
        Assert.Equal(typeof(GenericBase<int>), derivedType);
    }

    [Fact]
    public void GetFirstDerivedOfGenericType_ReturnsFalse_WhenNotGeneric()
    {
        Assert.False(GeneratorUtils.GetFirstDerivedOfGenericType(
            typeof(NonGeneric), typeof(GenericBase<>), out var derivedType));
        Assert.Null(derivedType);
    }

    [Fact]
    public void GetAssemblyToPackageMappings_ReturnsEmpty_WhenPackageJsonMissing()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory("/proj/app");

        var result = GeneratorUtils.GetAssemblyToPackageMappings(fileSystem, "/proj/app/My.Web.csproj");

        Assert.Empty(result);
    }

    [Fact]
    public void GetAssemblyToPackageMappings_MapsDependenciesFromPackageJson()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory("/proj/app");
        AddFile(fileSystem, "/proj/app/package.json", /*lang=json*/ """
            {
                "dependencies": {
                    "pkg-a": "file://./libs/a",
                    "pkg-b": "workspace:*",
                    "pkg-c": "file:///home/user/node_modules/.dotnet/ProjC",
                    "pkg-d": "1.2.3",
                    "pkg-e": null
                },
                "devDependencies": {
                    "pkg-f": "../shared/f"
                }
            }
            """);
        AddFile(fileSystem, "/proj/app/libs/a/A.Lib.csproj", "");
        AddFile(fileSystem, "/proj/app/node_modules/pkg-b/B.Lib.csproj", "");
        AddFile(fileSystem, "/proj/shared/f/F.Lib.csproj", "");

        var result = GeneratorUtils.GetAssemblyToPackageMappings(fileSystem, "/proj/app/My.Web.csproj");

        Assert.Equal("pkg-a", result["A.Lib"]);
        Assert.Equal("pkg-b", result["B.Lib"]);
        Assert.Equal("pkg-c", result["ProjC"]);
        Assert.Equal("pkg-f", result["F.Lib"]);
        Assert.DoesNotContain("pkg-d", result.Values);
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void GetAssemblyToPackageMappings_SkipsDotNetPathsWithSubDirectories()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory("/proj/app");
        AddFile(fileSystem, "/proj/app/package.json", /*lang=json*/ """
            {
                "dependencies": {
                    "pkg": "file:///home/user/node_modules/.dotnet/Some/Sub"
                }
            }
            """);

        var result = GeneratorUtils.GetAssemblyToPackageMappings(fileSystem, "/proj/app/My.Web.csproj");

        Assert.Empty(result);
    }

    [Fact]
    public void GetAssemblyToPackageMappings_SkipsFoldersWithMultipleProjects()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory("/proj/app/libs/multi");
        AddFile(fileSystem, "/proj/app/package.json", /*lang=json*/ """
            {
                "dependencies": {
                    "pkg": "./libs/multi"
                }
            }
            """);
        AddFile(fileSystem, "/proj/app/libs/multi/One.csproj", "");
        AddFile(fileSystem, "/proj/app/libs/multi/Two.csproj", "");

        var result = GeneratorUtils.GetAssemblyToPackageMappings(fileSystem, "/proj/app/My.Web.csproj");

        Assert.Empty(result);
    }
}
