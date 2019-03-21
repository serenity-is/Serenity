namespace Serenity.Test {
    let assert: Assert = QUnit.assert;
    QUnit.module('Widget');

    QUnit.test('GetWidget tests', function () {
        var input = $('<input />');
        assert.throws(function () { input.getWidget(Serenity.StringEditor) },
            "Element has no widget of type 'Serenity.StringEditor'! If you have recently changed editor type of a property in a form class, or changed data type in row (which also changes editor type) your script side Form definition might be out of date. Make sure your project builds successfully and transform T4 templates",
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
        assert.throws(function () { input.getWidget(Serenity.StringEditor) },
            "Element has no widget of type 'Serenity.StringEditor'! If you have recently changed editor type of a property in a form class, or changed data type in row (which also changes editor type) your script side Form definition might be out of date. Make sure your project builds successfully and transform T4 templates",
            'should throw after string editor is destroyed');
    });
}