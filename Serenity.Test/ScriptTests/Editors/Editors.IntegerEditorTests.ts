namespace Serenity.Test {
    let assert: Assert = QUnit.assert;
    QUnit.module('Editors');

    QUnit.test('Create IntegerEditor with undefined options', function () {
        let input = $('<input/>');

        assert.notEqual(null, new IntegerEditor(input),
            'create on a input');

        assert.ok(input.hasClass("integerQ"),
            'should have integerQ class');

        var autoNumeric = input.data('autoNumeric');
        assert.ok(autoNumeric != null,
            'should have autonumeric data');

        assert.strictEqual(autoNumeric.vMin, 0,
            'its min value should be 0');

        assert.strictEqual(autoNumeric.vMax, 2147483647,
            'its max value should be 2147483647 when options is undefined');

        assert.strictEqual(autoNumeric.mDec, 0,
            'shouldnt allow decimals');

        assert.strictEqual(autoNumeric.mInt, 10,
            'should allow 10 digits');
    });

    QUnit.test('Create IntegerEditor with empty options', function () {
        let input = $('<input/>');

        assert.notEqual(null, new IntegerEditor(input, {}),
            'create on a input');

        assert.ok(input.hasClass("integerQ"),
            'should have integerQ class');

        var autoNumeric = input.data('autoNumeric');
        assert.ok(autoNumeric != null, 
            'should have autoNumeric data');

        assert.strictEqual(autoNumeric.vMin, 0,
            'its min value should be 0');

        assert.strictEqual(autoNumeric.vMax, 2147483647,
            'its max value should be 2147483647');

        assert.strictEqual(autoNumeric.mDec, 0,
            'shouldnt allow decimals');

        assert.strictEqual(autoNumeric.mInt, 10,
            'should allow 10 digits');
    });


    QUnit.test('Create IntegerEditor with minValue option', function () {
        let input = $('<input/>');

        assert.notEqual(null, new IntegerEditor(input, { minValue: 5 }),
            'create on a input');

        assert.ok(input.hasClass("integerQ"),
            'should have integerQ class');

        var autoNumeric = input.data('autoNumeric');
        assert.ok(autoNumeric != null,
            'should have autoNumeric data');

        assert.strictEqual(autoNumeric.vMin, 5,
            'its min value should be 5');

        assert.strictEqual(autoNumeric.vMax, 2147483647,
            'its max value should be 2147483647');

        assert.strictEqual(autoNumeric.mDec, 0,
            'shouldnt allow decimals');

        assert.strictEqual(autoNumeric.mInt, 10,
            'should allow 10 digits');
    });

    QUnit.test('Create IntegerEditor with maxValue option', function () {
        let input = $('<input/>');

        assert.notEqual(null, new IntegerEditor(input, { maxValue: 79 }),
            'create on a input');

        assert.ok(input.hasClass("integerQ"),
            'should have integerQ class');

        var autoNumeric = input.data('autoNumeric');
        assert.ok(autoNumeric != null,
            'should have autoNumeric data');

        assert.strictEqual(autoNumeric.vMin, 0,
            'its min value should be 0');

        assert.strictEqual(autoNumeric.vMax, 79,
            'its max value should be 79');

        assert.strictEqual(autoNumeric.mDec, 0,
            'shouldnt allow decimals');

        assert.strictEqual(autoNumeric.mInt, 2,
            'should allow 2 digits');
    });

    QUnit.test('Create IntegerEditor with minValue and maxValue options', function () {
        let input = $('<input/>');

        assert.notEqual(null, new IntegerEditor(input, { minValue: 10, maxValue: 999 }),
            'create on a input');

        assert.ok(input.hasClass("integerQ"),
            'should have integerQ class');

        var autoNumeric = input.data('autoNumeric');
        assert.ok(autoNumeric != null,
            'should have autoNumeric data');

        assert.strictEqual(autoNumeric.vMin, 10,
            'its min value should be 10');

        assert.strictEqual(autoNumeric.vMax, 999,
            'its max value should be 999');

        assert.strictEqual(autoNumeric.mDec, 0,
            'shouldnt allow decimals');

        assert.strictEqual(autoNumeric.mInt, 3,
            'should allow 3 digits');
    });

}