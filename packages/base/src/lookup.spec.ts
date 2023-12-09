import { Lookup } from "./lookup";

describe("Lookup", () => {
    it('has itemById is populated', function () {
        let item1 = { x: 5, n: 'Item5' };
        let item2 = { x: 9, n: 'Item9' };
        let lookup = new Lookup({
            idField: 'x'
        }, [item1, item2]);

        expect(lookup.itemById != null).toBe(true);
        expect(lookup.itemById["5"]).toStrictEqual(item1);
        expect(lookup.itemById["5"].n).toEqual("Item5");
        expect(lookup.itemById["9"]).toStrictEqual(item2);
        expect(lookup.itemById["9"].n).toEqual("Item9");
    });

    it('can work with null options but produces empty itemById', () => {
        let lookup1 = new Lookup(null, [{ a: 5 }]);
        expect(lookup1.items).toEqual([{ a: 5 }]);
        expect(lookup1.itemById).toEqual({});

        let lookup2 = new Lookup(undefined, [{ a: 5 }]);
        expect(lookup2.items).toEqual([{ a: 5 }]);
        expect(lookup2.itemById).toEqual({});
    });

    it('can work if idField not specified but produces empty itemById', () => {
        let lookup1 = new Lookup({ textField: "a" }, [{ a: 5 }]);
        expect(lookup1.items).toEqual([{ a: 5 }]);
        expect(lookup1.itemById).toEqual({});
    });

    it('skips items with null or undefined value for itemById', function () {
        let item1 = { x: null as any, n: 'Item5' };
        let item2 = { x: undefined as any, n: 'Item9' };
        let item3 = { x: "test" as any, n: 'ItemTest' };
        let lookup = new Lookup({
            idField: 'x'
        }, [item1, item2, item3]);

        expect(lookup.itemById).toBeTruthy();
        expect(lookup.itemById["test"]).toStrictEqual(item3);
        expect(lookup.itemById).toEqual({
            test: item3
        });
    });

    it("can handle special distinct lookup data", () => {
        let lookup = new Lookup({
            idField: 'v',
            textField: 'v'
        }, ["a", "b"]);

        expect(lookup.itemById).toBeTruthy();
        expect(lookup.items).toEqual([{ v: "a" }, { v: "b" }]);
        expect(lookup.itemById["a"]).toEqual({ v: "a" });
        expect(lookup.itemById["b"]).toEqual({ v: "b" });
    });

    it("can handle distinct lookup with numeric values", () => {
        let lookup = new Lookup({
            idField: 't',
            textField: 't'
        }, [5, 3, 4]);

        expect(lookup.itemById).toBeTruthy();
        expect(lookup.items).toEqual([{ t: 5 }, { t: 3 }, { t: 4 }]);
        expect(lookup.itemById["5"]).toEqual({ t: 5 });
        expect(lookup.itemById["3"]).toEqual({ t: 3 });
        expect(lookup.itemById["4"]).toEqual({ t: 4 });
    });

    it("can handle distinct lookup with null or undefined", () => {
        let lookup = new Lookup({
            idField: 't',
            textField: 't'
        }, [null, 5, undefined]);

        expect(lookup.itemById).toBeTruthy();
        expect(lookup.items).toEqual([{ t: null }, { t: 5 }, { t: undefined }]);
        // it does not add null/undefined to itemById as they are converted to 
        // "null" or "undefined" strings
        expect(lookup.itemById).toEqual({
            "5": { t: 5 }
        });
    });
});