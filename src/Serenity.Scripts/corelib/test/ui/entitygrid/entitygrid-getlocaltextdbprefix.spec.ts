import { Decorators, LocalTextPrefixAttribute } from "@/decorators";
import { addAttribute } from "@/q";
import { EntityGrid } from "@/ui/datagrid/entitygrid";
import { jqueryPath, loadExternalScripts } from "../../testutil"

loadExternalScripts(window, jqueryPath);

function getLocalTextDbPrefix(grid: EntityGrid<any, any>): string {
    return grid["getLocalTextDbPrefix"]();
}

describe('EntityGrid.getLocalTextDbPrefix', () => {
    it('returns class identifier by default', () => {
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.Default.");
    });

    it('returns class identifier based on typeName property', () => {
        class DefaultGrid extends EntityGrid<any, any> {
            static readonly __typeName = 'MyProject.TestModule.DefaultGrid';
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.TestModule.Default.");
    });

    it('returns class identifier based on registration name', () => {
        @Decorators.registerClass('MyProject.MyModule.Some.DefaultGrid')
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.MyModule.Some.Default.");
    });

    it('can be overridden in subclass via getLocalTextDbPrefix', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getLocalTextDbPrefix() { return "My.Prefix." };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("My.Prefix.");
    });

    it('can be overridden in subclass via getLocalTextPrefix', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getLocalTextPrefix() { return "MySubClassPrefix" };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.MySubClassPrefix.");
    });

    it('can be set via attribute', () => {
        class AttrGrid extends EntityGrid<any, any> {
        }
        addAttribute(AttrGrid, new LocalTextPrefixAttribute("attrPrefix"));

        var grid = new AttrGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.attrPrefix.");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.prefixForTestRow.");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("");
    });
});