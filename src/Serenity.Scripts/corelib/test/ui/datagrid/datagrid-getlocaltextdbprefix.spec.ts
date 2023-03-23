import { LocalTextPrefixAttribute } from "@/decorators";
import { addAttribute } from "@/q";
import { DataGrid } from "@/ui/datagrid/datagrid";
import { jqueryPath, loadExternalScripts } from "../../testutil"

loadExternalScripts(window, jqueryPath);

function getLocalTextDbPrefix(grid: DataGrid<any, any>): string {
    return grid["getLocalTextDbPrefix"]();
}

describe('DataGrid.getLocalTextDbPrefix', () => {
    it('returns empty by default', () => {
        class DefaultGrid extends DataGrid<any, any> {
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("");
    });

    it('can be overridden in subclass via getLocalTextDbPrefix', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getLocalTextDbPrefix() { return "My.Prefix." };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("My.Prefix.");
    });

    it('can be overridden in subclass via getLocalTextPrefix', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getLocalTextPrefix() { return "MySubClassPrefix" };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.MySubClassPrefix.");
    });

    it('can be set via attribute', () => {
        class AttrGrid extends DataGrid<any, any> {
        }
        addAttribute(AttrGrid, new LocalTextPrefixAttribute("attrPrefix"));

        var grid = new AttrGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.attrPrefix.");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.prefixForTestRow.");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("");
    });
});