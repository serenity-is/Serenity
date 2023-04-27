import { any, count, first, groupBy, indexOf, insert, isArray, single, toGrouping, tryFirst } from "./arrays";

describe("Q.any", () => {
    it('returns false for empty array', function () {
        expect(any([], () => true)).toBe(false);
    });

    it('returns false if predicate returns false for all', () => {
        expect(any([1], () => false)).toBe(false);
    });

    it('returns true if predicate returns true for one', () => {
        expect(any([1, 2, 3], (x) => x === 3)).toBe(true);
    });

    it('returns true if predicate returns true for more than one', () => {
        expect(any([1, 2, 3], (x) => x % 2 == 1)).toBe(true);
    });
});

describe("Q.count", () => {
    it('returns 0 for empty array', function () {
        expect(count([], () => true)).toBe(0);
    });

    it('returns 0 if predicate returns false for all', () => {
        expect(count([1, 2, 3], () => false)).toBe(0);
    });

    it('returns number of elements predicate returns true', () => {
        expect(count([1, 2, 3], (x) => x === 3)).toBe(1);
        expect(count([1, 2, 3], (x) => x % 2 == 1)).toBe(2);
    });
});

describe("Q.first", () => {
    it('throws for empty array', function () {
        var thrown = false;
        try {
            first([], () => true)
        }
        catch (e) {
            thrown = true;
            expect(e.toString().indexOf('satisfies') >= 0).toBe(true);
        }
        expect(thrown).toBe(true);
    });

    it('throws if predicate returns false for all', () => {
        var thrown = false;
        try {
            first([1, 2, 3], () => false)
        }
        catch (e) {
            thrown = true;
            expect(e.toString().indexOf('satisfies') >= 0).toBe(true);
        }
        expect(thrown).toBe(true);
    });

    it('returns the first element that matches the predicate', () => {
        expect(first([1, 2, 3], (x) => x === 3)).toBe(3);
        expect(first([1, 2, 3], (x) => x % 2 == 1)).toBe(1);
    });
});

describe("Q.groupBy", () => {
    it('returns object with empty properties for empty array', () => {
        expect(groupBy([], x => x)).toEqual({
            byKey: {},
            inOrder: []
        });
    });

    it('uses empty string if key selector returns null or undefined', () => {
        expect(groupBy([1], x => undefined)).toEqual({
            byKey: {
                "": {
                    order: 0,
                    key: "",
                    items: [1],
                    start: 0
                }
            },
            inOrder: [
                {
                    order: 0,
                    key: "",
                    items: [1],
                    start: 0
                }
            ]
        });

        expect(groupBy([1], x => null)).toEqual({
            byKey: {
                "": {
                    order: 0,
                    key: "",
                    items: [1],
                    start: 0
                }
            },
            inOrder: [
                {
                    order: 0,
                    key: "",
                    items: [1],
                    start: 0
                }
            ]
        });
    });

    it('returns object with keys returned by key selector, and values as an array of matching items', () => {
        const actual = groupBy([1, 2, 3, 4, 5], x => x % 2);
        const expectedByKey = {
            "0": {
                order: 1,
                key: 0,
                items: [2, 4],
                start: 1
            },
            "1": {
                order: 0,
                key: 1,
                items: [1, 3, 5],
                start: 0
            }
        };
        expect(actual).toBeDefined();
        expect(actual.byKey).toBeDefined();
        expect(actual.inOrder).toBeDefined();
        expect(actual.inOrder.length).toBe(2);
        expect(actual.inOrder[0]).toBeDefined();
        expect(actual.inOrder[1]).toBeDefined();
        expect(actual.inOrder[0] === actual.byKey[1]).toBe(true);
        expect(actual.inOrder[1] === actual.byKey[0]).toBe(true);
        expect(actual.byKey).toEqual(expectedByKey);
    });
});

describe("Q.insert", () => {
    it('calls obj.insert if available', function () {
        var obj = {
            insert: function () {
            }
        }
        var mock = jest.spyOn(obj, "insert");
        insert(obj, 3, 7);
        expect(mock.mock.calls.length).toEqual(1);
        expect(mock.mock.calls[0]).toEqual([3, 7]);
    });

    it('uses array.splice if an array', function () {
        const array = [1, 2, 3, 4, 5];
        insert(array, 3, 7);
        expect(array === array).toBe(true);
        expect(array).toEqual([1, 2, 3, 7, 4, 5]);
    });

    it('throws for other types', function () {
        var obj = {};
        var thrown = false;
        try {
            insert(obj, 3, 5);
        }
        catch (e) {
            thrown = true;
            expect(e.toString().indexOf('support') >= 0).toBe(true);
        }
        expect(thrown).toBe(true);
    });

});

describe("Q.indexOf", () => {
    it('returns -1 for empty array', function () {
        expect(indexOf([], () => true)).toBe(-1);
    });

    it('returns -1 if predicate returns false for all', () => {
        expect(indexOf([1, 2, 3], () => false)).toBe(-1);
    });

    it('returns the index of first element that matches the predicate', () => {
        expect(indexOf([1, 2, 3], (x) => x === 3)).toBe(2);
        expect(indexOf([1, 2, 3], (x) => x % 2 == 1)).toBe(0);
    });
});

describe("Q.isArray", () => {
    it('is equal to Array.isArray method', function() {
        expect(isArray === Array.isArray).toBe(true);
    });
});


describe("Q.single", () => {
    it('throws for empty array', function () {
        var thrown = false;
        try {
            single([], () => true)
        }
        catch (e) {
            thrown = true;
            expect(e.toString().indexOf('satisfies') >= 0).toBe(true);
        }
        expect(thrown).toBe(true);
    });

    it('throws if predicate returns false for all', () => {
        var thrown = false;
        try {
            single([1, 2, 3], () => false)
        }
        catch (e) {
            thrown = true;
            expect(e.toString().indexOf('satisfies') >= 0).toBe(true);
        }
        expect(thrown).toBe(true);
    });

    it('returns the first single element that matches the predicate', () => {
        expect(single([1], (x) => x === 1)).toBe(1);
        expect(single([1, 2, 3], (x) => x === 3)).toBe(3);
    });

    it('throws if more than one element matches the predicate', () => {
        var thrown = false;
        try {
            single([1, 2, 3], x => x % 2 == 1)
        }
        catch (e) {
            thrown = true;
            expect(e.toString().indexOf('more than one') >= 0).toBe(true);
        }
        expect(thrown).toBe(true);
    });
});

describe("Q.toGrouping", () => {
    it('returns empty object for empty array', () => {
        expect(toGrouping([], x => x)).toStrictEqual({})
    });

    it('uses empty string if key selector returns null or undefined', () => {
        expect(toGrouping([1], x => null)).toStrictEqual({
            "": [1]
        });

        expect(toGrouping([1], x => undefined)).toStrictEqual({
            "": [1]
        });
    });

    it('returns object with keys returned by key selector, and values as an array of matching items', () => {
        expect(toGrouping([1, 2, 3, 4, 5], x => x % 2)).toStrictEqual({
            "0": [2, 4],
            "1": [1, 3, 5],
        })
    });
});

describe("Q.tryFirst", () => {
    it('returns undefined for empty array', function () {
        expect(tryFirst([], () => true)).toBeUndefined();
    });

    it('returns undefined if predicate returns false for all', () => {
        expect(tryFirst([1, 2, 3], () => false)).toBeUndefined();
    });

    it('returns the first element that matches the predicate', () => {
        expect(tryFirst([1, 2, 3], (x) => x === 3)).toBe(3);
        expect(tryFirst([1, 2, 3], (x) => x % 2 == 1)).toBe(1);
    });
});