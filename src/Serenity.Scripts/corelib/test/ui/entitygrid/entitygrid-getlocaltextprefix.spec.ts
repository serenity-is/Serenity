import { LocalTextPrefixAttribute, Decorators } from "@/decorators";
import { addAttribute } from "@/q";
import { EntityGrid } from "@/ui/datagrid/entitygrid";
import { jqueryPath, loadExternalScripts } from "../../testutil"

loadExternalScripts(window, jqueryPath);

function getLocalTextPrefix(grid: EntityGrid<any, any>): string {
    return grid["getLocalTextPrefix"]();
}

describe('EntityGrid.getLocalTextPrefix', () => {
    it('returns class identifier by default', () => {
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("Default");
    });

    it('returns class identifier based on typeName property', () => {
        class DefaultGrid extends EntityGrid<any, any> {
            static readonly __typeName = 'MyProject.TestModule.DefaultGrid';
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("TestModule.Default");
    });

    it('returns class identifier based on registration name', () => {
        @Decorators.registerClass('MyProject.MyModule.Some.DefaultGrid')
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("MyModule.Some.Default");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getLocalTextPrefix() { return "subClassPrefix" };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("subClassPrefix");
    });

    it('can be set via attribute', () => {
        class AttrGrid extends EntityGrid<any, any> {
        }
        addAttribute(AttrGrid, new LocalTextPrefixAttribute("attrPrefix"));

        var grid = new AttrGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("attrPrefix");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("prefixForTestRow");
    });

    it("returns undefined string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBeUndefined();
    });
});