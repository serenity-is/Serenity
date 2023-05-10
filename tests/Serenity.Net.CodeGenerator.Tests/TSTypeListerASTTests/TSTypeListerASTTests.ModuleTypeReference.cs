using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class TSTypeListerASTTests
{
    [Fact]
    public void Resolves_Imported_Type_From_Serenity_Module()
    {
        var fileSystem = new MockGeneratorFileSystem();
        fileSystem.CreateDirectory(root);
        string myDialog = root + "Modules/Test/MyDialog.ts";
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(myDialog));
        fileSystem.WriteAllText(myDialog, @"
import { EntityDialog } from '@serenity-is/corelib';

export class MyDialog extends EntityDialog {
}
");
        string corelibPackage = root + "node_modules/@serenity-is/corelib/package.json";
        string serenityIndexTS = root + "node_modules/@serenity-is/corelib/src/serenity/index.ts";
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(serenityIndexTS));

        fileSystem.WriteAllText(corelibPackage, @"{
    ""types"": ""src/index.ts"",
    ""typesVersions"": {
        ""*"": {
            ""serenity"": [ ""src/serenity/index.ts"" ]
        }
    }
}");

        fileSystem.WriteAllText(serenityIndexTS, @"
export class Dummy {
}
");

        var tl = new TSTypeListerAST(fileSystem, tsConfigDir: root, tsConfig: new TSConfig
        {
        });
        tl.AddInputFile(myDialog);

        var types = tl.ExtractTypes();

        var myDialogType = Assert.Single(types, x => x.FullName == "/Modules/Test/MyDialog:MyDialog");
        Assert.Equal("@serenity-is/corelib:EntityDialog", myDialogType.BaseType);
    }
}
