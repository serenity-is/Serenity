namespace Serenity.Test {
    let assert: Assert = QUnit.assert;
    QUnit.module('Formatters');

    QUnit.test('UrlFormatter tests', function () {
        assert.notEqual(null, new UrlFormatter(),
            'can create instance');

        assert.strictEqual("<a href='http://simpleurl'>http://simpleurl</a>",
            new UrlFormatter().format({
                value: 'http://simpleurl'
            }), 'field with simple url value and text');

        assert.strictEqual("<a href='http://s?a=b&amp;c=d'>http://s?a=b&amp;c=d</a>",
            new UrlFormatter().format({
                value: 'http://s?a=b&c=d'
            }), 'field with simple url value and text, html encoding check');

        let formatter: UrlFormatter;

        formatter = new UrlFormatter();
        formatter.set_target('my');
        assert.strictEqual("<a href='http://s' target='my'>http://s</a>",
            formatter.format({
                value: 'http://s',
            }), 'respects target');

        formatter = new UrlFormatter();
        formatter.set_displayFormat('http://s/{0}');
        assert.strictEqual("<a href='x'>http://s/x</a>",
            formatter.format({
                value: 'x',
            }), 'respects target');
    });
}