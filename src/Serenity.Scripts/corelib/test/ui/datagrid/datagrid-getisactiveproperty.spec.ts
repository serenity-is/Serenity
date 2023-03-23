import { IsActivePropertyAttribute } from "@/decorators";
import { addAttribute } from "@/q";
import { DataGrid } from "@/ui/datagrid/datagrid";
import { jqueryPath, loadExternalScripts } from "../../testutil"

loadExternalScripts(window, jqueryPath);

function getIsActiveProperty(grid: DataGrid<any, any>): string {
    return grid["getIsActiveProperty"]();
}

describe('DataGrid.getIsActiveProperty', () => {
    it('returns empty by default', () => {
        class DefaultGrid extends DataGrid<any, any> {
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("");
    });

    it('can be set via attribute', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getIsActiveProperty() { return "subClassIsActive" };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("subClassIsActive");
    });

    it('can be set via attribute', () => {
        class AttrGrid extends DataGrid<any, any> {
        }
        addAttribute(AttrGrid, new IsActivePropertyAttribute("attrIsActive"));

        var grid = new AttrGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("attrIsActive");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly isActiveProperty = "activeForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("activeForTestRow");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly isActiveProperty = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("");
    });
});