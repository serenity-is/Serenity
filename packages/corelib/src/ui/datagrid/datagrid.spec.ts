import { Column } from "@serenity-is/sleekgrid";
import { DataGrid, omitAllGridPersistenceFlags } from "./datagrid";

function getIdProperty(grid: DataGrid<any, any>): string {
    return grid["getIdProperty"]();
}

describe('DataGrid.getIdProperty', () => {
    it('returns ID by default', () => {
        class DefaultGrid extends DataGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getIdProperty(grid)).toBe("ID");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getIdProperty() { return "subClassId" };
        }

        var grid = new SubClassGrid({});
        expect(getIdProperty(grid)).toBe("subClassId");
    });

    it('returns value from getRowDefition()', () => {
        class TestRow {
            static readonly idProperty = "idForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIdProperty(grid)).toBe("idForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly idProperty: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
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

        var grid = new DefaultGrid({});
        expect(getIsActiveProperty(grid)).toBe("");
    });

    it('can be set via attribute', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getIsActiveProperty() { return "subClassIsActive" };
        }

        var grid = new SubClassGrid({});
        expect(getIsActiveProperty(grid)).toBe("subClassIsActive");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly isActiveProperty = "activeForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIsActiveProperty(grid)).toBe("activeForTestRow");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly isActiveProperty: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
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

        var grid = new DefaultGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("");
    });

    it('can be overridden in subclass via getLocalTextDbPrefix', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getLocalTextDbPrefix() { return "My.Prefix." };
        }

        var grid = new SubClassGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("My.Prefix.");
    });

    it('can be overridden in subclass via getLocalTextPrefix', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getLocalTextPrefix() { return "MySubClassPrefix" };
        }

        var grid = new SubClassGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.MySubClassPrefix.");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.prefixForTestRow.");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
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

        var grid = new DefaultGrid({});
        expect(getLocalTextPrefix(grid)).toBeUndefined();
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getLocalTextPrefix() { return "subClassPrefix" };
        }

        var grid = new SubClassGrid({});
        expect(getLocalTextPrefix(grid)).toBe("subClassPrefix");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextPrefix(grid)).toBe("prefixForTestRow");
    });

    it("returns undefined if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextPrefix(grid)).toBeUndefined();
    });
});

describe("DataGrid.getCurrentSettings", () => {
    it("returns an empty object with columns for a default grid", () => {
        class MyGrid extends DataGrid<any, any> {
        }

        const grid = new MyGrid({});
        expect(grid.getCurrentSettings()).toEqual({
            columns: [],
            flags: DataGrid.defaultOptions.persistenceFlags
        });
    });

    it("does not return columns if flags.columnVisibility is false, flags.columnWidths is false, and flags.sortColumns is false", () => {
        class MyGrid extends DataGrid<any, any> {
        }

        const grid = new MyGrid({});
        const flags = {
            ...omitAllGridPersistenceFlags,
            columnVisibility: false,
            columnWidths: false,
            sortColumns: false
        };
        expect(grid.getCurrentSettings(flags)).toEqual({
            flags: flags
        });
    });

    it("sets column visibility only for visible columns", () => {
        class MyGrid extends DataGrid<any, any> {
            getColumns(): Column[] {
                return [
                    { field: "A" },
                    { field: "B", visible: false },
                    { field: "C" }
                ];
            }
        }

        const grid = new MyGrid({});
        const flags = {
            ...omitAllGridPersistenceFlags,
            columnVisibility: true
        };
        expect(grid.getCurrentSettings(flags)).toEqual({
            columns: [
                { id: "A", visible: true },
                { id: "B", visible: false },
                { id: "C", visible: true }
            ],
            flags: flags
        });
    });

    it("sets column widths", () => {
        class MyGrid extends DataGrid<any, any> {
            getColumns(): Column[] {
                return [
                    { field: "A", width: 100 },
                    { field: "B", width: 200 },
                ];
            }
        }

        const grid = new MyGrid({});
        const flags = { 
            ...omitAllGridPersistenceFlags,
            columnWidths: true 
        };
        expect(grid.getCurrentSettings(flags)).toEqual({
            columns: [
                { id: "A", width: 100 },
                { id: "B", width: 200 }
            ],
            flags: flags
        });
    });
});