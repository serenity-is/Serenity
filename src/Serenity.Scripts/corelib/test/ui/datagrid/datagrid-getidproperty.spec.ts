import { IdPropertyAttribute } from "@/decorators";
import { addAttribute } from "@/q";
import { DataGrid } from "@/ui/datagrid/datagrid";
import $ from "jquery";

function getIdProperty(grid: DataGrid<any, any>): string {
    return grid["getIdProperty"]();
}

describe('DataGrid.getIdProperty', () => {
    it('returns ID by default', () => {
        class DefaultGrid extends DataGrid<any, any> {
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("ID");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getIdProperty() { return "subClassId" };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("subClassId");
    });

    it('can be set via attribute', () => {
        class AttrGrid extends DataGrid<any, any> {
        }
        addAttribute(AttrGrid, new IdPropertyAttribute("attrId"));

        var grid = new AttrGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("attrId");
    });

    it('returns value from getRowDefition()', () => {
        class TestRow {
            static readonly idProperty = "idForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("idForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly idProperty = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("");
    });
});