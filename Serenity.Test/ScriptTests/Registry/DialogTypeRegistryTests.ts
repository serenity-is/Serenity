namespace DummyRoot {
    export namespace SomeModule {
        @Serenity.Decorators.registerClass()
        export class SomeDialog extends Serenity.PropertyDialog<any, any> {
        }

        @Serenity.Decorators.registerClass()
        export class SomeDialogWithoutDialogSuffix extends Serenity.PropertyDialog<any, any> {
        }
    }
}

namespace Serenity.Test {
    let assert: Assert = QUnit.assert;
    QUnit.module('Registry');

    QUnit.test('DialogTypeRegistry tests', function () {
        Q.Config.rootNamespaces.push("DummyRoot");
        try {
            assert.equal(DummyRoot.SomeModule.SomeDialog, Serenity.DialogTypeRegistry.get("DummyRoot.SomeModule.SomeDialog"),
                "should find dialog class with full namespace");

            assert.equal(DummyRoot.SomeModule.SomeDialog, Serenity.DialogTypeRegistry.get("SomeModule.SomeDialog"),
                "should find dialog class with module name only");

            assert.equal(DummyRoot.SomeModule.SomeDialog, Serenity.DialogTypeRegistry.get("SomeModule.Some"),
                "should find dialog class without specifying Dialog suffix");

            assert.equal(DummyRoot.SomeModule.SomeDialogWithoutDialogSuffix, Serenity.DialogTypeRegistry.get("SomeModule.SomeDialogWithoutDialogSuffix"),
                "should find dialog class that doesn't have Dialog suffix");

            assert.throws(() => Serenity.DialogTypeRegistry.get("SomeDialog"), "SomeDialog dialog class is not found! Make sure there is a dialog class with this name, it is under your project root namespace, and your namespace parts start with capital letters, e.g. MyProject.Pascal.Cased namespace. If you got this error from an editor with InplaceAdd option check that lookup key and dialog type name matches (case sensitive, excluding Dialog suffix). You need to change lookup key or specify DialogType property in LookupEditor attribute if that's not the case.",
                "shouldn't find dialog class without module name");

            assert.throws(() => Serenity.DialogTypeRegistry.get("Some"), "Some dialog class is not found! Make sure there is a dialog class with this name, it is under your project root namespace, and your namespace parts start with capital letters, e.g. MyProject.Pascal.Cased namespace. If you got this error from an editor with InplaceAdd option check that lookup key and dialog type name matches (case sensitive, excluding Dialog suffix). You need to change lookup key or specify DialogType property in LookupEditor attribute if that's not the case.",
                "shouldn't find dialog class without module name and suffix");

            assert.throws(() => Serenity.DialogTypeRegistry.get("SomeDialogWithoutDialogSuffix"), "SomeDialogWithoutDialogSuffix dialog class is not found! Make sure there is a dialog class with this name, it is under your project root namespace, and your namespace parts start with capital letters, e.g. MyProject.Pascal.Cased namespace. If you got this error from an editor with InplaceAdd option check that lookup key and dialog type name matches (case sensitive, excluding Dialog suffix). You need to change lookup key or specify DialogType property in LookupEditor attribute if that's not the case.",
                "shouldn't find dialog class that doesn't have dialog suffix without module");
        }
        finally {
            Q.Config.rootNamespaces = Q.Config.rootNamespaces.filter(x => x != "DummyRoot");
        }

    });
}