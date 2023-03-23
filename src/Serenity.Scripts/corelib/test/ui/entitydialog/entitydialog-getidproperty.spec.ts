import { IdPropertyAttribute } from "@/decorators";
import { addAttribute } from "@/q";
import { EntityDialog } from "@/ui/dialogs/entitydialog";
import { jqueryPath, jqueryValidatePath, loadExternalScripts, toastrPath } from "../../testutil"

loadExternalScripts(window, jqueryPath, jqueryValidatePath, toastrPath);

function getIdProperty(dialog: EntityDialog<any, any>): string {
    return dialog["getIdProperty"]();
}

function mockPropertyItemsData() {
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

        var Dialog = new DefaultDialog($('<div/>'));
        expect(getIdProperty(Dialog)).toBe("ID");
    });

    it('can be overridden in subclass', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getIdProperty() { return "subClassId" };
        }

        var Dialog = new SubClassDialog($('<div/>'));
        expect(getIdProperty(Dialog)).toBe("subClassId");
    });

    it('can be set via attribute', () => {
        class AttrDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }
        addAttribute(AttrDialog, new IdPropertyAttribute("attrId"));

        var Dialog = new AttrDialog($('<div/>'));
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

        var Dialog = new TestRowDialog($('<div/>'));
        expect(getIdProperty(Dialog)).toBe("idForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly idProperty = undefined;
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog($('<div/>'));
        expect(getIdProperty(dialog)).toBe("");
    });
});