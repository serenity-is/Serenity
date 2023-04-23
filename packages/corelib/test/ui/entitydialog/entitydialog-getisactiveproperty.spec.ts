import { IsActivePropertyAttribute } from "@/decorators";
import { addAttribute } from "@/q";
import { EntityDialog } from "@/ui/dialogs/entitydialog";

function getIsActiveProperty(dialog: EntityDialog<any, any>): string {
    return dialog["getIsActiveProperty"]();
}

function mockPropertyItemsData() {
    return {
        items: [],
        additionalItems: []
    }
}

describe('EntityDialog.getIsActiveProperty', () => {
    it('returns empty string by default', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var Dialog = new DefaultDialog($('<div/>'));
        expect(getIsActiveProperty(Dialog)).toBe("");
    });

    it('can be overridden in subclass', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getIsActiveProperty() { return "subClassIsActive" };
        }

        var Dialog = new SubClassDialog($('<div/>'));
        expect(getIsActiveProperty(Dialog)).toBe("subClassIsActive");
    });

    it('can be set via attribute', () => {
        class AttrDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }
        addAttribute(AttrDialog, new IsActivePropertyAttribute("attrIsActive"));

        var Dialog = new AttrDialog($('<div/>'));
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

        var Dialog = new TestRowDialog($('<div/>'));
        expect(getIsActiveProperty(Dialog)).toBe("isActiveForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly isActiveProperty = undefined;
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog($('<div/>'));
        expect(getIsActiveProperty(dialog)).toBe("");
    });
});