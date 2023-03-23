import { IdPropertyAttribute } from "@/decorators";
import { addAttribute } from "@/q";
import { EntityGrid } from "@/ui/datagrid/entitygrid";
import { jqueryPath, loadExternalScripts } from "../../testutil"

loadExternalScripts(window, jqueryPath);

function getIdProperty(grid: EntityGrid<any, any>): string {
    return grid["getIdProperty"]();
}

describe('EntityGrid.getIdProperty', () => {
    it('returns ID by default', () => {
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("ID");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getIdProperty() { return "subClassId" };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("subClassId");
    });

    it('can be overridden in subclass', () => {
        class AttrGrid extends EntityGrid<any, any> {
        }
        addAttribute(AttrGrid, new IdPropertyAttribute("attrId"));

        var grid = new AttrGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("attrId");
    });

    it('returns value from getRowDefition()', () => {
        class TestRow {
            static readonly idProperty = "idForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("idForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly idProperty = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("");
    });
});