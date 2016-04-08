var Serenity;
(function (Serenity) {
    var Test;
    (function (Test) {
        var assert = QUnit.assert;
        QUnit.module('Editors');
        QUnit.test('TextAreaEditor creation tests', function () {
            assert.notEqual(null, new Serenity.TextAreaEditor($('<textarea/>')), 'create on a textarea with undefined options');
            assert.notEqual(null, new Serenity.TextAreaEditor($('<textarea/>'), {}), 'create on a textarea with empty options');
            var txtarea = $('<textarea/>');
            new Serenity.TextAreaEditor(txtarea, null);
            assert.equal(txtarea.attr('cols'), 80, 'should have 80 cols by default when options is undefined');
            assert.equal(txtarea.attr('rows'), 6, 'should have 6 rows by default when options is undefined');
            txtarea = $('<textarea/>');
            new Serenity.TextAreaEditor(txtarea, {});
            assert.equal(txtarea.attr('cols'), 80, 'should have 80 cols by default when options is empty');
            assert.equal(txtarea.attr('rows'), 6, 'should have 6 rows by default when options is empty');
            txtarea = $('<textarea/>');
            new Serenity.TextAreaEditor(txtarea, {
                cols: 77
            });
            assert.equal(txtarea.attr('cols'), 77, 'respects cols option');
            txtarea = $('<textarea/>');
            new Serenity.TextAreaEditor(txtarea, {
                rows: 9
            });
            assert.equal(txtarea.attr('rows'), 9, 'respects rows option');
        });
    })(Test = Serenity.Test || (Serenity.Test = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var Test;
    (function (Test) {
        var assert = QUnit.assert;
        QUnit.module('Formatters');
        QUnit.test('UrlFormatter tests', function () {
            assert.notEqual(null, new Serenity.UrlFormatter(), 'can create instance');
            assert.strictEqual("<a href='http://simpleurl'>http://simpleurl</a>", new Serenity.UrlFormatter().format({
                value: 'http://simpleurl'
            }), 'field with simple url value and text');
            assert.strictEqual("<a href='http://s?a=b&amp;c=d'>http://s?a=b&amp;c=d</a>", new Serenity.UrlFormatter().format({
                value: 'http://s?a=b&c=d'
            }), 'field with simple url value and text, html encoding check');
            var formatter;
            formatter = new Serenity.UrlFormatter();
            formatter.set_target('my');
            assert.strictEqual("<a href='http://s' target='my'>http://s</a>", formatter.format({
                value: 'http://s'
            }), 'respects target');
            formatter = new Serenity.UrlFormatter();
            formatter.set_displayFormat('http://s/{0}');
            assert.strictEqual("<a href='x'>http://s/x</a>", formatter.format({
                value: 'x'
            }), 'respects target');
        });
    })(Test = Serenity.Test || (Serenity.Test = {}));
})(Serenity || (Serenity = {}));
//# sourceMappingURL=Serenity.Test.js.map