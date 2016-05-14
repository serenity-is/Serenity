namespace Serenity.Test {
    QUnit.module('Q.ArrayUtils');

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