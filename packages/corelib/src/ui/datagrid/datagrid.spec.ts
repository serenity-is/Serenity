import { IdPropertyAttribute, IsActivePropertyAttribute, LocalTextPrefixAttribute } from "../../decorators";
import { DataGrid } from "./datagrid";
import $ from "@optionaldeps/jquery";
import { addAttribute } from "../../q/system";

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
            static readonly idProperty: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("");
    });
});

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
            static readonly isActiveProperty: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("");
    });
});

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
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("");
    });
});

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
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBeUndefined();
    });
});