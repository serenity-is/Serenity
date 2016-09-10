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
    let assert: QUnitAssert = QUnit.assert;
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

            assert.throws(() => Serenity.DialogTypeRegistry.get("SomeDialog"), "SomeDialog dialog class is not found!",
                "shouldn't find dialog class without module name");

            assert.throws(() => Serenity.DialogTypeRegistry.get("Some"), "SomeDialog dialog class is not found!",
                "shouldn't find dialog class without module name and suffix");

            assert.throws(() => Serenity.DialogTypeRegistry.get("SomeDialogWithoutDialogSuffix"), "SomeDialogWithoutDialogSuffixDialog dialog class is not found!",
                "shouldn't find dialog class that doesn't have dialog suffix without module");
        }
        finally {
            Q.Config.rootNamespaces = Q.Config.rootNamespaces.filter(x => x != "DummyRoot");
        }

    });
}