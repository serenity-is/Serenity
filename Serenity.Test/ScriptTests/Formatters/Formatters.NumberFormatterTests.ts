namespace Serenity.Test {
    let assert: Assert = QUnit.assert;
    QUnit.module('Formatters');

    QUnit.test('NumberFormatter tests', function () {
        assert.notEqual(null, new NumberFormatter(),
            'can create instance');

        assert.strictEqual(FormatterTypeRegistry.get('Number'), NumberFormatter,
            'can get from formatter type registry with key: "Number"');

        assert.strictEqual(FormatterTypeRegistry.get('Serenity.Number'), NumberFormatter,
            'can get from formatter type registry with key: "Serenity.Number"');

        assert.strictEqual(NumberFormatter.format(9.876), '9.88',
            'formats with two decimals by default');

        assert.strictEqual(NumberFormatter.format(9.876, '#.0'), '9.9',
            'respects format parameter: #.0');

        assert.strictEqual(NumberFormatter.format(9.876, '#.0000'), '9.8760',
            'respects format parameter: 0.0000');

        assert.strictEqual(NumberFormatter.format(undefined), '',
            'returns empty string for undefined value');

        assert.strictEqual(NumberFormatter.format(null), '',
            'returns empty string for null value');

        assert.strictEqual(NumberFormatter.format(NaN), '',
            'returns empty string for NaN');
    });
}