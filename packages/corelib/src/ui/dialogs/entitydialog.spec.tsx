import { PropertyItemsData, addCustomAttribute, classTypeInfo } from "../../base";
import { IdPropertyAttribute, IsActivePropertyAttribute, LocalTextPrefixAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { EntityDialog } from "./entitydialog";

function getIdProperty(dialog: EntityDialog<any, any>): string {
    return dialog["getIdProperty"]();
}

function mockPropertyItemsData(): PropertyItemsData {
    return {
        items: [],
        additionalItems: []
    }
}

describe('EntityDialog.getIdProperty', () => {
    it('returns ID by default', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var Dialog = new DefaultDialog({});
        expect(getIdProperty(Dialog)).toBe("ID");
    });

    it('can be overridden in subclass', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getIdProperty() { return "subClassId" };
        }

        var Dialog = new SubClassDialog({});
        expect(getIdProperty(Dialog)).toBe("subClassId");
    });

    it('can be set via attribute', () => {
        class AttrDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }
        addCustomAttribute(AttrDialog, new IdPropertyAttribute("attrId"));

        var Dialog = new AttrDialog({});
        expect(getIdProperty(Dialog)).toBe("attrId");
    });

    it('returns value from getRowDefition()', () => {
        class TestRow {
            static readonly idProperty = "idForTestRow";
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var Dialog = new TestRowDialog({});
        expect(getIdProperty(Dialog)).toBe("idForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly idProperty: string = undefined;
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog({});
        expect(getIdProperty(dialog)).toBe("");
    });
});

function getIsActiveProperty(dialog: EntityDialog<any, any>): string {
    return dialog["getIsActiveProperty"]();
}

describe('EntityDialog.getIsActiveProperty', () => {
    it('returns empty string by default', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var Dialog = new DefaultDialog({});
        expect(getIsActiveProperty(Dialog)).toBe("");
    });

    it('can be overridden in subclass', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getIsActiveProperty() { return "subClassIsActive" };
        }

        var Dialog = new SubClassDialog({});
        expect(getIsActiveProperty(Dialog)).toBe("subClassIsActive");
    });

    it('can be set via attribute', () => {
        class AttrDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }
        addCustomAttribute(AttrDialog, new IsActivePropertyAttribute("attrIsActive"));

        var Dialog = new AttrDialog({});
        expect(getIsActiveProperty(Dialog)).toBe("attrIsActive");
    });

    it('returns value from getRowDefition()', () => {
        class TestRow {
            static readonly isActiveProperty = "isActiveForTestRow";
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var Dialog = new TestRowDialog({});
        expect(getIsActiveProperty(Dialog)).toBe("isActiveForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly isActiveProperty: string = undefined;
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog({});
        expect(getIsActiveProperty(dialog)).toBe("");
    });
});

function getLocalTextDbPrefix(dialog: EntityDialog<any, any>): string {
    return dialog["getLocalTextDbPrefix"]();
}

describe('EntityDialog.getLocalTextDbPrefix', () => {
    it('returns class identifier by default', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.Default.");
    });

    it('returns class identifier based on typeInfo property', () => {
        @Decorators.registerType()
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            static override readonly typeInfo = Decorators.classType('MyProject.TestModule.DefaultDialog');
        }

        var dialog = new DefaultDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.TestModule.Default.");
    });

    it('returns class identifier based on registration name', () => {
        @Decorators.registerClass('MyProject.MyModule.Some.DefaultDialog')
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.MyModule.Some.Default.");
    });

    it('can be overridden in subclass via getLocalTextDbPrefix', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getLocalTextDbPrefix() { return "My.Prefix." };
        }

        var dialog = new SubClassDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("My.Prefix.");
    });

    it('can be overridden in subclass via getLocalTextPrefix', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getLocalTextPrefix() { return "MySubClassPrefix" };
        }

        var dialog = new SubClassDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.MySubClassPrefix.");
    });

    it('can be set via attribute', () => {
        class AttrDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }
        addCustomAttribute(AttrDialog, new LocalTextPrefixAttribute("attrPrefix"));

        var dialog = new AttrDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.attrPrefix.");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.prefixForTestRow.");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("");
    });
});

function getLocalTextPrefix(dialog: EntityDialog<any, any>): string {
    return dialog["getLocalTextPrefix"]();
}

describe('EntityDialog.getLocalTextPrefix', () => {
    it('returns class identifier by default', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog({});
        expect(getLocalTextPrefix(dialog)).toBe("Default");
    });

    it('returns class identifier based on typeInfo property', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            static readonly typeInfo = classTypeInfo('MyProject.TestModule.DefaultDialog');
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog({});
        expect(getLocalTextPrefix(dialog)).toBe("TestModule.Default");
    });

    it('returns class identifier based on registration name', () => {
        @Decorators.registerClass('MyProject.MyModule.Some.DefaultDialog')
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog({});
        expect(getLocalTextPrefix(dialog)).toBe("MyModule.Some.Default");
    });

    it('can be overridden in subclass', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getLocalTextPrefix() { return "subClassPrefix" };
        }

        var dialog = new SubClassDialog({});
        expect(getLocalTextPrefix(dialog)).toBe("subClassPrefix");
    });

    it('can be set via attribute', () => {
        class AttrDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }
        addCustomAttribute(AttrDialog, new LocalTextPrefixAttribute("attrPrefix"));

        var dialog = new AttrDialog({});
        expect(getLocalTextPrefix(dialog)).toBe("attrPrefix");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog({});
        expect(getLocalTextPrefix(dialog)).toBe("prefixForTestRow");
    });

    it("returns undefined if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog({});
        expect(getLocalTextPrefix(dialog)).toBeUndefined();
    });
});

describe("EntityDialog.destroy", () => {

    class MockDialog extends EntityDialog<any, any> {
        getPropertyItemsData() { return mockPropertyItemsData() };
    }

    it("calls destroy on propertyGrid", () => {
        const dialog = new MockDialog();
        const destroy = jest.fn();
        dialog["propertyGrid"] = { destroy } as any;
        dialog.destroy();
        expect(destroy).toHaveBeenCalledTimes(1);
        expect(dialog["propertyGrid"] == null).toBe(true);
    });

    it("calls destroy on localizer", () => {
        const dialog = new MockDialog();
        const destroy = jest.fn();
        dialog["localizer"] = { destroy } as any;
        dialog.destroy();
        expect(destroy).toHaveBeenCalledTimes(1);
        expect(dialog["localizer"] == null).toBe(true);
    });

    it("clears toolbar and button references", () => {
        const dialog = new MockDialog();
        const props = ["toolbar", "editButton", "cloneButton", "saveAndCloseButton", "applyChangesButton", "deleteButton", "undeleteButton"];
        props.forEach(p => (dialog as any)[p] = "test" as any);
        dialog.destroy();
        props.forEach(p => expect((dialog as any)[p] == null).toBe(true));
    });

    it("calls super.destroy", () => {
        const dialog = new MockDialog();
        dialog.destroy();
        expect(dialog.domNode == null).toBe(true);
    });

});