namespace Serenity.Test {
    let assert: Assert = QUnit.assert;
    QUnit.module('Widget');

    QUnit.test('GetWidget tests', function () {
        var input = $('<input />');
        assert.throws(function () { input.getWidget(Serenity.StringEditor) }, err => err.toString().indexOf("Element has no widget of type") >= 0,
            'should throw before widget creation');

        var stringEditor = new StringEditor(input);
        assert.strictEqual(input.getWidget(Serenity.StringEditor), stringEditor,
            'should return created stringeditor widget');

        var secondaryWidget = new DecimalEditor(input);
        assert.strictEqual(input.getWidget(Serenity.StringEditor), stringEditor,
            'should still return stringeditor after second widget');

        (<any>secondaryWidget).destroy();
        assert.strictEqual(input.getWidget(Serenity.StringEditor), stringEditor,
            'should still return stringeditor after second widget is destroyed');

        assert.strictEqual(input.getWidget(Serenity.Widget), stringEditor,
            'can return stringeditor using base class');

        (<any>stringEditor).destroy();
        assert.throws(function () { input.getWidget(Serenity.StringEditor) }, err => err.toString().indexOf("Element has no widget of type") >= 0,
            'should throw after string editor is destroyed');
    });
}