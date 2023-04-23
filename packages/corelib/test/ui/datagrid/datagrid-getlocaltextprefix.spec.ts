import { LocalTextPrefixAttribute } from "@/decorators";
import { addAttribute } from "@/q";
import { DataGrid } from "@/ui/datagrid/datagrid";
import $ from "jquery";

function getLocalTextPrefix(grid: DataGrid<any, any>): string {
    return grid["getLocalTextPrefix"]();
}

describe('DataGrid.getLocalTextPrefix', () => {
    it('returns undefined by default', () => {
        class DefaultGrid extends DataGrid<any, any> {
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBeUndefined();
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getLocalTextPrefix() { return "subClassPrefix" };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("subClassPrefix");
    });

    it('can be set via attribute', () => {
        class AttrGrid extends DataGrid<any, any> {
        }
        addAttribute(AttrGrid, new LocalTextPrefixAttribute("attrPrefix"));

        var grid = new AttrGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("attrPrefix");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("prefixForTestRow");
    });

    it("returns undefined if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBeUndefined();
    });
});