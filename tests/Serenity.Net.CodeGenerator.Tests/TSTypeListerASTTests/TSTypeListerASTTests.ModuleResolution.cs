using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class TSTypeListerASTTests
{
    const string root = "/root/";

    [Fact]
    public void Resolves_Relative_Module_In_Same_Dir()
    {
        var fileSystem = new MockGeneratorFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "a.d.ts", @"
import { B1 } from './b';
");

        fileSystem.WriteAllText(root + "b.d.ts", @"
export class B1 {
}
");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: root, tsConfig: null);
        tl.AddInputFile(root + "a.d.ts");

        var types = tl.ExtractTypes();

        Assert.Single(types, x => x.FullName == "/b:B1");
    }

    [Fact]
    public void Resolves_Relative_Slash_Module_In_Same_Dir()
    {
        var fileSystem = new MockGeneratorFileSystem();
        fileSystem.CreateDirectory(root);
        string fileA = root + "x/y/a.d.ts";
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(fileA));
        fileSystem.WriteAllText(fileA, @"
import { B1 } from '/z/b';
");

        string fileB = root + "z/b.d.ts";
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(fileB));
        fileSystem.WriteAllText(fileB, @"
export class B1 {
}
");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: root, tsConfig: null);
        tl.AddInputFile(fileA);

        var types = tl.ExtractTypes();

        Assert.Single(types, x => x.FullName == "/z/b:B1");
    }

    [Fact]
    public void Resolves_Module_With_Aliased_Asterisk_Path()
    {
        var fileSystem = new MockGeneratorFileSystem();
        fileSystem.CreateDirectory(root);
        string fileA = root + "x/y/a.d.ts";
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(fileA));
        fileSystem.WriteAllText(fileA, @"
import { B1 } from 'my/b';
");

        string fileB = root + "zzz/b.d.ts";
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(fileB));
        fileSystem.WriteAllText(fileB, @"
export class B1 {
}
");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: root, tsConfig: new TSConfig
        {
            CompilerOptions = new()
            {
                BaseUrl = ".",
                Paths = new()
                {
                    ["my/*"] = new[] { "./zzz/*" }
                }
            }
        });
        tl.AddInputFile(fileA);

        var types = tl.ExtractTypes();

        Assert.Single(types, x => x.FullName == "/zzz/b:B1");
    }

    [Fact]
    public void Resolves_Module_With_Aliased_Asterisk_Path_Multiple()
    {
        var fileSystem = new MockGeneratorFileSystem();
        fileSystem.CreateDirectory(root);
        string fileA = root + "x/y/a.d.ts";
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(fileA));
        fileSystem.WriteAllText(fileA, @"
import { B1 } from 'my/b';
");

        string fileB = root + "zzz/b.d.ts";
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(fileB));
        fileSystem.WriteAllText(fileB, @"
export class B1 {
}
");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: root, tsConfig: new TSConfig
        {
            CompilerOptions = new()
            {
                BaseUrl = ".",
                Paths = new()
                {
                    ["my/*"] = new[] { "./nnn", "./zzz/*" }
                }
            }
        });
        tl.AddInputFile(fileA);

        var types = tl.ExtractTypes();

        Assert.Single(types, x => x.FullName == "/zzz/b:B1");
    }

    [Fact]
    public void Resolves_Node_Module_In_TsConfig_Dir()
    {
        var fileSystem = new MockGeneratorFileSystem();
        fileSystem.CreateDirectory(root);
        string fileA = root + "x/y/a.d.ts";
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(fileA));
        fileSystem.WriteAllText(fileA, @"
import { B1 } from 'module-a';
");

        string moduleAPackage = root + "node_modules/module-a/package.json";
        string moduleAIndexTS = root + "node_modules/module-a/src/index.ts";
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(moduleAIndexTS));

        fileSystem.WriteAllText(moduleAPackage, @"{
    ""types"": ""src/index.ts""
}");

        fileSystem.WriteAllText(moduleAIndexTS, @"
export class A1 {
}
");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: root, tsConfig: new TSConfig
        {
        });
        tl.AddInputFile(fileA);

        var types = tl.ExtractTypes();

        Assert.Single(types, x => x.FullName == "module-a:A1");
    }
}
