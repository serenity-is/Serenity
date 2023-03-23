import { LocalTextPrefixAttribute, Decorators } from "@/decorators";
import { addAttribute } from "@/q";
import { EntityDialog } from "@/ui/dialogs/entitydialog";
import { jqueryPath, jqueryValidatePath, loadExternalScripts, toastrPath } from "../../testutil"

loadExternalScripts(window, jqueryPath, jqueryValidatePath, toastrPath);

function getLocalTextPrefix(dialog: EntityDialog<any, any>): string {
    return dialog["getLocalTextPrefix"]();
}

function mockPropertyItemsData() {
    return {
        items: [],
        additionalItems: []
    }
}

describe('EntityDialog.getLocalTextPrefix', () => {
    it('returns class identifier by default', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog($('<div/>'));
        expect(getLocalTextPrefix(dialog)).toBe("Default");
    });

    it('returns class identifier based on typeName property', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            static readonly __typeName = 'MyProject.TestModule.DefaultDialog';
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog($('<div/>'));
        expect(getLocalTextPrefix(dialog)).toBe("TestModule.Default");
    });

    it('returns class identifier based on registration name', () => {
        @Decorators.registerClass('MyProject.MyModule.Some.DefaultDialog')
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog($('<div/>'));
        expect(getLocalTextPrefix(dialog)).toBe("MyModule.Some.Default");
    });

    it('can be overridden in subclass', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getLocalTextPrefix() { return "subClassPrefix" };
        }

        var dialog = new SubClassDialog($('<div/>'));
        expect(getLocalTextPrefix(dialog)).toBe("subClassPrefix");
    });

    it('can be set via attribute', () => {
        class AttrDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }
        addAttribute(AttrDialog, new LocalTextPrefixAttribute("attrPrefix"));

        var dialog = new AttrDialog($('<div/>'));
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

        var dialog = new TestRowDialog($('<div/>'));
        expect(getLocalTextPrefix(dialog)).toBe("prefixForTestRow");
    });

    it("returns undefined if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix = undefined;
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog($('<div/>'));
        expect(getLocalTextPrefix(dialog)).toBeUndefined();
    });
});