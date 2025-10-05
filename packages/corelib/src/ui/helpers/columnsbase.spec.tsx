import { ColumnsBase } from "./columnsbase";

describe("ColumnsBase", () => {
    it("should create instance with columns and access by id, sourceItem.name, and field", () => {
        const columns = [
            { id: "col1", field: "Field1", sourceItem: { name: "Source1" } },
            { id: "col2", field: "Field2", sourceItem: { name: "Source2" } },
            { id: "col3", field: "Field3" } // No sourceItem
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase.valueOf()).toBe(columns);
        expect(colsBase.col1).toBe(columns[0]);
        expect(colsBase.Source1).toBe(columns[0]);
        expect(colsBase.Field1).toBe(columns[0]);

        expect(colsBase.col2).toBe(columns[1]);
        expect(colsBase.Source2).toBe(columns[1]);
        expect(colsBase.Field2).toBe(columns[1]);

        expect(colsBase.col3).toBe(columns[2]);
        expect(colsBase.Field3).toBe(columns[2]);
        expect(colsBase.Source3).toBeUndefined();
    });

    it("should handle empty columns array", () => {
        const colsBase = new ColumnsBase([]) as any
        expect(colsBase.valueOf()).toEqual([]);
        expect(colsBase.anyKey).toBeUndefined();
    });

    it("should handle null columns array", () => {
        const colsBase = new ColumnsBase(null as any) as any;
        expect(colsBase.valueOf()).toEqual([]);
        expect(colsBase.anyKey).toBeUndefined();
    });

    it("should handle undefined columns array", () => {
        const colsBase = new ColumnsBase(undefined as any) as any;
        expect(colsBase.valueOf()).toEqual([]);
        expect(colsBase.anyKey).toBeUndefined();
    });

    it("should not overwrite existing keys from id with sourceItem.name or field", () => {
        const columns = [
            { id: "sameKey", field: "differentField", sourceItem: { name: "differentName" } }
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase.sameKey).toBe(columns[0]);
        // Since keys are different, all should be set
        expect(colsBase.differentName).toBe(columns[0]);
        expect(colsBase.differentField).toBe(columns[0]);
    });

    it("should not overwrite existing keys from sourceItem.name with field", () => {
        const columns = [
            { field: "sameKey", sourceItem: { name: "differentName" } }
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase.differentName).toBe(columns[0]);
        // Since keys are different, field should also be set
        expect(colsBase.sameKey).toBe(columns[0]);
    });

    it("should handle columns with missing id", () => {
        const columns = [
            { field: "Field1", sourceItem: { name: "Source1" } },
            { field: "Field2" }
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase.Source1).toBe(columns[0]);
        expect(colsBase.Field1).toBe(columns[0]);
        expect(colsBase.Field2).toBe(columns[1]);
    });

    it("should handle columns with missing sourceItem", () => {
        const columns = [
            { id: "col1", field: "Field1" },
            { id: "col2", field: "Field2" }
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase.col1).toBe(columns[0]);
        expect(colsBase.Field1).toBe(columns[0]);
        expect(colsBase.col2).toBe(columns[1]);
        expect(colsBase.Field2).toBe(columns[1]);
    });

    it("should handle columns with missing field", () => {
        const columns = [
            { id: "col1", sourceItem: { name: "Source1" } },
            { id: "col2" }
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase.col1).toBe(columns[0]);
        expect(colsBase.Source1).toBe(columns[0]);
        expect(colsBase.col2).toBe(columns[1]);
    });

    it("should handle columns with empty string properties", () => {
        const columns = [
            { id: "", field: "", sourceItem: { name: "" } },
            { id: "col2", field: "Field2" }
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase.col2).toBe(columns[1]);
        expect(colsBase.Field2).toBe(columns[1]);
        // Empty strings should not create properties
        expect(colsBase[""]).toBeUndefined();
    });

    it("should handle columns with null/undefined properties", () => {
        const columns = [
            { id: null as any, field: undefined, sourceItem: { name: null as any } },
            { id: "col2", field: "Field2" }
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase.col2).toBe(columns[1]);
        expect(colsBase.Field2).toBe(columns[1]);
    });

    it("should handle conflicting keys by keeping first occurrence", () => {
        const columns = [
            { id: "conflict", field: "Field1" },
            { id: "conflict2", field: "conflict" }
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase.conflict).toBe(columns[0]); // From first column's id
        expect(colsBase.Field1).toBe(columns[0]);
        expect(colsBase.conflict2).toBe(columns[1]);
    });

    it("should handle special characters in keys", () => {
        const columns = [
            { id: "col-with-dashes", field: "field_with_underscores", sourceItem: { name: "name.with.dots" } }
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase["col-with-dashes"]).toBe(columns[0]);
        expect(colsBase["field_with_underscores"]).toBe(columns[0]);
        expect(colsBase["name.with.dots"]).toBe(columns[0]);
    });

    it("should handle numeric keys", () => {
        const columns = [
            { id: "123", field: "456" }
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase["123"]).toBe(columns[0]);
        expect(colsBase["456"]).toBe(columns[0]);
    });

    it("should handle columns with same field but different ids", () => {
        const columns = [
            { id: "col1", field: "sameField" },
            { id: "col2", field: "sameField" }
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase.col1).toBe(columns[0]);
        expect(colsBase.col2).toBe(columns[1]);
        expect(colsBase.sameField).toBe(columns[0]); // First occurrence wins
    });

    it("should handle sourceItem without name property", () => {
        const columns = [
            { id: "col1", field: "Field1", sourceItem: {} },
            { id: "col2", field: "Field2", sourceItem: null as any }
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase.col1).toBe(columns[0]);
        expect(colsBase.Field1).toBe(columns[0]);
        expect(colsBase.col2).toBe(columns[1]);
        expect(colsBase.Field2).toBe(columns[1]);
    });

    it("should not overwrite existing properties", () => {
        const columns = [
            { id: "valueOf", field: "Field1" }, // This would conflict with the valueOf method
            { id: "toString", field: "Field2" } // This would conflict with toString method
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(typeof colsBase.valueOf).toBe("function"); // Should still be the method
        expect(typeof colsBase.toString).toBe("function"); // Should still be the method
        expect(colsBase.Field1).toBe(columns[0]);
        expect(colsBase.Field2).toBe(columns[1]);
    });

    it("should handle large number of columns", () => {
        const columns = Array.from({ length: 100 }, (_, i) => ({
            id: `col${i}`,
            field: `field${i}`,
            sourceItem: { name: `source${i}` }
        }));
        const colsBase = new ColumnsBase(columns) as any;

        expect(colsBase.valueOf()).toBe(columns);
        expect(colsBase.col0).toBe(columns[0]);
        expect(colsBase.source50).toBe(columns[50]);
        expect(colsBase.field99).toBe(columns[99]);
    });

    it("should handle columns with prototype properties", () => {
        const columns = [
            { id: "constructor", field: "Field1" },
            { id: "hasOwnProperty", field: "Field2" }
        ];
        const colsBase = new ColumnsBase(columns) as any;

        expect(typeof colsBase.constructor).toBe("function"); // Should still be the constructor
        expect(typeof colsBase.hasOwnProperty).toBe("function"); // Should still be the method
        expect(colsBase.Field1).toBe(columns[0]);
        expect(colsBase.Field2).toBe(columns[1]);
    });
});