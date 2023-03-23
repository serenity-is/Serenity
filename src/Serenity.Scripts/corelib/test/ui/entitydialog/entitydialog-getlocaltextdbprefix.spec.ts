import { Decorators, LocalTextPrefixAttribute } from "@/decorators";
import { addAttribute } from "@/q";
import { EntityDialog } from "@/ui/dialogs/entitydialog";
import { jqueryPath, jqueryValidatePath, loadExternalScripts, toastrPath } from "../../testutil"

loadExternalScripts(window, jqueryPath, jqueryValidatePath, toastrPath);

function getLocalTextDbPrefix(dialog: EntityDialog<any, any>): string {
    return dialog["getLocalTextDbPrefix"]();
}

function mockPropertyItemsData() {
    return {
        items: [],
        additionalItems: []
    }
}

describe('EntityDialog.getLocalTextDbPrefix', () => {
    it('returns class identifier by default', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog($('<div/>'));
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.Default.");
    });

    it('returns class identifier based on typeName property', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            static readonly __typeName = 'MyProject.TestModule.DefaultDialog';
        }

        var dialog = new DefaultDialog($('<div/>'));
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.TestModule.Default.");
    });

    it('returns class identifier based on registration name', () => {
        @Decorators.registerClass('MyProject.MyModule.Some.DefaultDialog')
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog($('<div/>'));
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.MyModule.Some.Default.");
    });

    it('can be overridden in subclass via getLocalTextDbPrefix', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getLocalTextDbPrefix() { return "My.Prefix." };
        }

        var dialog = new SubClassDialog($('<div/>'));
        expect(getLocalTextDbPrefix(dialog)).toBe("My.Prefix.");
    });

    it('can be overridden in subclass via getLocalTextPrefix', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getLocalTextPrefix() { return "MySubClassPrefix" };
        }

        var dialog = new SubClassDialog($('<div/>'));
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.MySubClassPrefix.");
    });

    it('can be set via attribute', () => {
        class AttrDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }
        addAttribute(AttrDialog, new LocalTextPrefixAttribute("attrPrefix"));

        var dialog = new AttrDialog($('<div/>'));
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

        var dialog = new TestRowDialog($('<div/>'));
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.prefixForTestRow.");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix = undefined;
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog($('<div/>'));
        expect(getLocalTextDbPrefix(dialog)).toBe("");
    });
});