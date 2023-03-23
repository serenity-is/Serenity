import { IsActivePropertyAttribute } from "@/decorators";
import { addAttribute } from "@/q";
import { EntityGrid } from "@/ui/datagrid/entitygrid";
import { jqueryPath, loadExternalScripts } from "../../testutil"

loadExternalScripts(window, jqueryPath);

function getIsActiveProperty(grid: EntityGrid<any, any>): string {
    return grid["getIsActiveProperty"]();
}

describe('EntityGrid.getIsActiveProperty', () => {
    it('returns empty by default', () => {
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getIsActiveProperty() { return "subClassIsActive" };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("subClassIsActive");
    });

    it('can be set via attribute', () => {
        class AttrGrid extends EntityGrid<any, any> {
        }
        addAttribute(AttrGrid, new IsActivePropertyAttribute("attrIsActive"));

        var grid = new AttrGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("attrIsActive");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly isActiveProperty = "activeForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("activeForTestRow");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly isActiveProperty = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("");
    });
});