namespace Serenity.Test {
    QUnit.module('Q.ArrayUtils');

    QUnit.test('arrayClone()', function (assert: QUnitAssert) {
        assert.throws(() => Q.arrayClone(null), null,
            "should throw for null reference");

        assert.notEqual(null, Q.arrayClone([]),
            "shouldn't return null");

        let a = ['a'];
        let b = Q.arrayClone(a);
        assert.strictEqual(b.length, 1,
            "clone has same length");

        assert.strictEqual('a', b[0],
            "clone has 'a' at index 0");

        assert.notStrictEqual(a, b,
            "clone doesn't have same reference");

        a = ['a', 'b', 'c'];
        b = Q.arrayClone(a);
        assert.strictEqual(b.length, 3,
            "clone has same length");

        assert.strictEqual('a', b[0],
            "clone has 'a' at index 0");

        assert.strictEqual('b', b[1],
            "clone has 'b' at index 1");

        assert.strictEqual('c', b[2],
            "clone has 'c' at index 2");

        assert.notStrictEqual(a, b,
            "clone doesn't have same reference");
    });

    QUnit.test('any()', function (assert: QUnitAssert) {
        assert.throws(() => Q.any(null, () => true), null,
            "should throw for null reference");

        assert.strictEqual(false, Q.any([], () => false),
            "should return false for empty array and false predicate");

        assert.strictEqual(false, Q.any([], () => true),
            "should return false for empty array and true predicate");

        assert.strictEqual(false, Q.any([1, 2, 3], x => x === 4),
            "should return false for non-matching predicate");

        assert.strictEqual(true, Q.any([1, 2, 3], x => x === 2),
            "should return true for matching predicate");
    });

    QUnit.test('count()', function (assert: QUnitAssert) {
        assert.throws(() => Q.count(null, () => true), null,
            "should throw for null reference");

        assert.strictEqual(0, Q.count([], () => false),
            "should return 0 for empty array and false predicate");

        assert.strictEqual(0, Q.count([], () => true),
            "should return 0 for empty array and true predicate");

        assert.strictEqual(0, Q.count([1, 2, 3], () => false),
            "should return 0 for array with 3 elements and false predicate");

        assert.strictEqual(3, Q.count([1, 2, 3], () => true),
            "should return 3 for array with 3 elements and true predicate");

        assert.strictEqual(0, Q.count([1, 2, 3], x => x === 4),
            "should return 0 for non-matching predicate");

        assert.strictEqual(1, Q.count([1, 2, 3], x => x === 2),
            "should return 1 for matching predicate");

        assert.strictEqual(3, Q.count([4, 3, 2, 2, 1, 2], x => x === 2),
            "shouldn't short circuit at first match");
    });

}