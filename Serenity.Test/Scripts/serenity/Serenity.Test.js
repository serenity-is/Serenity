var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
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
        QUnit.test('NumberFormatter tests', function () {
            assert.notEqual(null, new Serenity.NumberFormatter(), 'can create instance');
            assert.strictEqual(Serenity.FormatterTypeRegistry.get('Number'), Serenity.NumberFormatter, 'can get from formatter type registry with key: "Number"');
            assert.strictEqual(Serenity.FormatterTypeRegistry.get('Serenity.Number'), Serenity.NumberFormatter, 'can get from formatter type registry with key: "Serenity.Number"');
            assert.strictEqual(Serenity.NumberFormatter.format(9.876), '9.88', 'formats with two decimals by default');
            assert.strictEqual(Serenity.NumberFormatter.format(9.876, '#.0'), '9.9', 'respects format parameter: #.0');
            assert.strictEqual(Serenity.NumberFormatter.format(9.876, '#.0000'), '9.8760', 'respects format parameter: 0.0000');
            assert.strictEqual(Serenity.NumberFormatter.format(undefined), '', 'returns empty string for undefined value');
            assert.strictEqual(Serenity.NumberFormatter.format(null), '', 'returns empty string for null value');
            assert.strictEqual(Serenity.NumberFormatter.format(NaN), '', 'returns empty string for NaN');
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
            formatter.target = 'my';
            assert.strictEqual("<a href='http://s' target='my'>http://s</a>", formatter.format({
                value: 'http://s',
            }), 'respects target');
            formatter = new Serenity.UrlFormatter();
            formatter.displayFormat = 'http://s/{0}';
            assert.strictEqual("<a href='x'>http://s/x</a>", formatter.format({
                value: 'x',
            }), 'respects target');
        });
    })(Test = Serenity.Test || (Serenity.Test = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var Test;
    (function (Test) {
        QUnit.module('Q.ArrayUtils');
        QUnit.test('any()', function (assert) {
            assert.throws(function () { return Q.any(null, function () { return true; }); }, null, "should throw for null reference");
            assert.strictEqual(false, Q.any([], function () { return false; }), "should return false for empty array and false predicate");
            assert.strictEqual(false, Q.any([], function () { return true; }), "should return false for empty array and true predicate");
            assert.strictEqual(false, Q.any([1, 2, 3], function (x) { return x === 4; }), "should return false for non-matching predicate");
            assert.strictEqual(true, Q.any([1, 2, 3], function (x) { return x === 2; }), "should return true for matching predicate");
        });
        QUnit.test('count()', function (assert) {
            assert.throws(function () { return Q.count(null, function () { return true; }); }, null, "should throw for null reference");
            assert.strictEqual(0, Q.count([], function () { return false; }), "should return 0 for empty array and false predicate");
            assert.strictEqual(0, Q.count([], function () { return true; }), "should return 0 for empty array and true predicate");
            assert.strictEqual(0, Q.count([1, 2, 3], function () { return false; }), "should return 0 for array with 3 elements and false predicate");
            assert.strictEqual(3, Q.count([1, 2, 3], function () { return true; }), "should return 3 for array with 3 elements and true predicate");
            assert.strictEqual(0, Q.count([1, 2, 3], function (x) { return x === 4; }), "should return 0 for non-matching predicate");
            assert.strictEqual(1, Q.count([1, 2, 3], function (x) { return x === 2; }), "should return 1 for matching predicate");
            assert.strictEqual(3, Q.count([4, 3, 2, 2, 1, 2], function (x) { return x === 2; }), "shouldn't short circuit at first match");
        });
    })(Test = Serenity.Test || (Serenity.Test = {}));
})(Serenity || (Serenity = {}));
var DummyRoot;
(function (DummyRoot) {
    var SomeModule;
    (function (SomeModule) {
        var SomeDialog = /** @class */ (function (_super) {
            __extends(SomeDialog, _super);
            function SomeDialog() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            SomeDialog = __decorate([
                Serenity.Decorators.registerClass()
            ], SomeDialog);
            return SomeDialog;
        }(Serenity.PropertyDialog));
        SomeModule.SomeDialog = SomeDialog;
        var SomeDialogWithoutDialogSuffix = /** @class */ (function (_super) {
            __extends(SomeDialogWithoutDialogSuffix, _super);
            function SomeDialogWithoutDialogSuffix() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            SomeDialogWithoutDialogSuffix = __decorate([
                Serenity.Decorators.registerClass()
            ], SomeDialogWithoutDialogSuffix);
            return SomeDialogWithoutDialogSuffix;
        }(Serenity.PropertyDialog));
        SomeModule.SomeDialogWithoutDialogSuffix = SomeDialogWithoutDialogSuffix;
    })(SomeModule = DummyRoot.SomeModule || (DummyRoot.SomeModule = {}));
})(DummyRoot || (DummyRoot = {}));
var Serenity;
(function (Serenity) {
    var Test;
    (function (Test) {
        var assert = QUnit.assert;
        QUnit.module('Registry');
        QUnit.test('DialogTypeRegistry tests', function () {
            Q.Config.rootNamespaces.push("DummyRoot");
            try {
                assert.equal(DummyRoot.SomeModule.SomeDialog, Serenity.DialogTypeRegistry.get("DummyRoot.SomeModule.SomeDialog"), "should find dialog class with full namespace");
                assert.equal(DummyRoot.SomeModule.SomeDialog, Serenity.DialogTypeRegistry.get("SomeModule.SomeDialog"), "should find dialog class with module name only");
                assert.equal(DummyRoot.SomeModule.SomeDialog, Serenity.DialogTypeRegistry.get("SomeModule.Some"), "should find dialog class without specifying Dialog suffix");
                assert.equal(DummyRoot.SomeModule.SomeDialogWithoutDialogSuffix, Serenity.DialogTypeRegistry.get("SomeModule.SomeDialogWithoutDialogSuffix"), "should find dialog class that doesn't have Dialog suffix");
                assert.throws(function () { return Serenity.DialogTypeRegistry.get("SomeDialog"); }, function (err) { return err.toString().indexOf("SomeDialog dialog class is not found") >= 0; }, "shouldn't find dialog class without module name");
                assert.throws(function () { return Serenity.DialogTypeRegistry.get("Some"); }, function (err) { return err.toString().indexOf("Some dialog class is not found") >= 0; }, "shouldn't find dialog class without module name and suffix");
                assert.throws(function () { return Serenity.DialogTypeRegistry.get("SomeDialogWithoutDialogSuffix"); }, function (err) { return err.toString().indexOf("SomeDialogWithoutDialogSuffix dialog class is not found") >= 0; }, "shouldn't find dialog class that doesn't have dialog suffix without module");
            }
            finally {
                Q.Config.rootNamespaces = Q.Config.rootNamespaces.filter(function (x) { return x != "DummyRoot"; });
            }
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
            assert.throws(function () { input.getWidget(Serenity.StringEditor); }, function (err) { return err.toString().indexOf("Element has no widget of type") >= 0; }, 'should throw before widget creation');
            var stringEditor = new Serenity.StringEditor(input);
            assert.strictEqual(input.getWidget(Serenity.StringEditor), stringEditor, 'should return created stringeditor widget');
            var secondaryWidget = new Serenity.DecimalEditor(input);
            assert.strictEqual(input.getWidget(Serenity.StringEditor), stringEditor, 'should still return stringeditor after second widget');
            secondaryWidget.destroy();
            assert.strictEqual(input.getWidget(Serenity.StringEditor), stringEditor, 'should still return stringeditor after second widget is destroyed');
            assert.strictEqual(input.getWidget(Serenity.Widget), stringEditor, 'can return stringeditor using base class');
            stringEditor.destroy();
            assert.throws(function () { input.getWidget(Serenity.StringEditor); }, function (err) { return err.toString().indexOf("Element has no widget of type") >= 0; }, 'should throw after string editor is destroyed');
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