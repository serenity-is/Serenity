var Serenity;
(function (Serenity) {
    var Test;
    (function (Test) {
        var assert = QUnit.assert;
        QUnit.module('Editors');
        QUnit.test('Create IntegerEditor with undefined options', function () {
            var input = $('<input/>');
            assert.notEqual(null, new Serenity.IntegerEditor(input), 'create on a input');
            assert.ok(input.hasClass("integerQ"), 'should have integerQ class');
            var autoNumeric = input.data('autoNumeric');
            assert.ok(autoNumeric != null, 'should have autonumeric data');
            assert.strictEqual(autoNumeric.vMin, 0, 'its min value should be 0');
            assert.strictEqual(autoNumeric.vMax, 2147483647, 'its max value should be 2147483647 when options is undefined');
            assert.strictEqual(autoNumeric.mDec, 0, 'shouldnt allow decimals');
            assert.strictEqual(autoNumeric.mInt, 10, 'should allow 10 digits');
        });
        QUnit.test('Create IntegerEditor with empty options', function () {
            var input = $('<input/>');
            assert.notEqual(null, new Serenity.IntegerEditor(input, {}), 'create on a input');
            assert.ok(input.hasClass("integerQ"), 'should have integerQ class');
            var autoNumeric = input.data('autoNumeric');
            assert.ok(autoNumeric != null, 'should have autoNumeric data');
            assert.strictEqual(autoNumeric.vMin, 0, 'its min value should be 0');
            assert.strictEqual(autoNumeric.vMax, 2147483647, 'its max value should be 2147483647');
            assert.strictEqual(autoNumeric.mDec, 0, 'shouldnt allow decimals');
            assert.strictEqual(autoNumeric.mInt, 10, 'should allow 10 digits');
        });
        QUnit.test('Create IntegerEditor with minValue option', function () {
            var input = $('<input/>');
            assert.notEqual(null, new Serenity.IntegerEditor(input, { minValue: 5 }), 'create on a input');
            assert.ok(input.hasClass("integerQ"), 'should have integerQ class');
            var autoNumeric = input.data('autoNumeric');
            assert.ok(autoNumeric != null, 'should have autoNumeric data');
            assert.strictEqual(autoNumeric.vMin, 5, 'its min value should be 5');
            assert.strictEqual(autoNumeric.vMax, 2147483647, 'its max value should be 2147483647');
            assert.strictEqual(autoNumeric.mDec, 0, 'shouldnt allow decimals');
            assert.strictEqual(autoNumeric.mInt, 10, 'should allow 10 digits');
        });
        QUnit.test('Create IntegerEditor with maxValue option', function () {
            var input = $('<input/>');
            assert.notEqual(null, new Serenity.IntegerEditor(input, { maxValue: 79 }), 'create on a input');
            assert.ok(input.hasClass("integerQ"), 'should have integerQ class');
            var autoNumeric = input.data('autoNumeric');
            assert.ok(autoNumeric != null, 'should have autoNumeric data');
            assert.strictEqual(autoNumeric.vMin, 0, 'its min value should be 0');
            assert.strictEqual(autoNumeric.vMax, 79, 'its max value should be 79');
            assert.strictEqual(autoNumeric.mDec, 0, 'shouldnt allow decimals');
            assert.strictEqual(autoNumeric.mInt, 2, 'should allow 2 digits');
        });
        QUnit.test('Create IntegerEditor with minValue and maxValue options', function () {
            var input = $('<input/>');
            assert.notEqual(null, new Serenity.IntegerEditor(input, { minValue: 10, maxValue: 999 }), 'create on a input');
            assert.ok(input.hasClass("integerQ"), 'should have integerQ class');
            var autoNumeric = input.data('autoNumeric');
            assert.ok(autoNumeric != null, 'should have autoNumeric data');
            assert.strictEqual(autoNumeric.vMin, 10, 'its min value should be 10');
            assert.strictEqual(autoNumeric.vMax, 999, 'its max value should be 999');
            assert.strictEqual(autoNumeric.mDec, 0, 'shouldnt allow decimals');
            assert.strictEqual(autoNumeric.mInt, 3, 'should allow 3 digits');
        });
    })(Test = Serenity.Test || (Serenity.Test = {}));
})(Serenity || (Serenity = {}));
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
var Serenity;
(function (Serenity) {
    var Test;
    (function (Test) {
        var assert = QUnit.assert;
        QUnit.module('Widget');
        QUnit.test('GetWidget tests', function () {
            var input = $('<input />');
            assert.throws(function () { input.getWidget(Serenity.StringEditor); }, "Element has no widget of type 'Serenity.StringEditor'!", 'should throw before widget creation');
            var stringEditor = new Serenity.StringEditor(input);
            assert.strictEqual(input.getWidget(Serenity.StringEditor), stringEditor, 'should return created stringeditor widget');
            var secondaryWidget = new Serenity.DecimalEditor(input);
            assert.strictEqual(input.getWidget(Serenity.StringEditor), stringEditor, 'should still return stringeditor after second widget');
            secondaryWidget.destroy();
            assert.strictEqual(input.getWidget(Serenity.StringEditor), stringEditor, 'should still return stringeditor after second widget is destroyed');
            assert.strictEqual(input.getWidget(Serenity.Widget), stringEditor, 'can return stringeditor using base class');
            stringEditor.destroy();
            assert.throws(function () { input.getWidget(Serenity.StringEditor); }, "Element has no widget of type 'Serenity.StringEditor'!", 'should throw after string editor is destroyed');
        });
    })(Test = Serenity.Test || (Serenity.Test = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var Test;
    (function (Test) {
        var assert = QUnit.assert;
        QUnit.module('Widget');
        QUnit.test('TryGetWidget tests', function () {
            var input = $('<input />');
            assert.strictEqual(input.tryGetWidget(Serenity.StringEditor), null, 'should return null before widget creation');
            var stringEditor = new Serenity.StringEditor(input);
            assert.strictEqual(input.tryGetWidget(Serenity.StringEditor), stringEditor, 'should return created stringeditor widget');
            var secondaryWidget = new Serenity.DecimalEditor(input);
            assert.strictEqual(input.tryGetWidget(Serenity.StringEditor), stringEditor, 'should still return stringeditor after second widget');
            secondaryWidget.destroy();
            assert.strictEqual(input.tryGetWidget(Serenity.StringEditor), stringEditor, 'should still return stringeditor after second widget is destroyed');
            assert.strictEqual(input.tryGetWidget(Serenity.Widget), stringEditor, 'can return stringeditor using base class');
            stringEditor.destroy();
            assert.equal(input.tryGetWidget(Serenity.StringEditor), null, 'should return null after string editor is destroyed');
        });
    })(Test = Serenity.Test || (Serenity.Test = {}));
})(Serenity || (Serenity = {}));
//# sourceMappingURL=Serenity.Test.js.map