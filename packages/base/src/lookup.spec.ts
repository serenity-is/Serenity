import { Lookup } from "./lookup";

describe("Q.Lookup", () => {
    it('has itemById is populated', function () {
        var item1 = { x: 5, n: 'Item5' };
        var item2 = { x: 9, n: 'Item9' };
        var lookup = new Lookup({
            idField: 'x'
        }, [item1, item2]);

        expect(lookup.itemById != null).toBe(true);
        expect(lookup.itemById["5"]).toStrictEqual(item1);
        expect(lookup.itemById["5"].n).toEqual("Item5");
        expect(lookup.itemById["9"]).toStrictEqual(item2);
        expect(lookup.itemById["9"].n).toEqual("Item9");
    });
});