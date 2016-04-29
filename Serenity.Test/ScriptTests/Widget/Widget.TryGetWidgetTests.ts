namespace Serenity.Test {
    let assert: QUnitAssert = QUnit.assert;
    QUnit.module('Widget');

    QUnit.test('TryGetWidget tests', function () {
        var input = $('<input />');
        assert.strictEqual(input.tryGetWidget(Serenity.StringEditor), null,
            'should return null before widget creation');

        var stringEditor = new StringEditor(input);
        assert.strictEqual(input.tryGetWidget(Serenity.StringEditor), stringEditor,
            'should return created stringeditor widget');

        var secondaryWidget = new DecimalEditor(input);
        assert.strictEqual(input.tryGetWidget(Serenity.StringEditor), stringEditor,
            'should still return stringeditor after second widget');

        (<any>secondaryWidget).destroy();
        assert.strictEqual(input.tryGetWidget(Serenity.StringEditor), stringEditor,
            'should still return stringeditor after second widget is destroyed');

        assert.strictEqual(input.tryGetWidget(Serenity.Widget), stringEditor,
            'can return stringeditor using base class');

        (<any>stringEditor).destroy();
        assert.equal(input.tryGetWidget(Serenity.StringEditor), null,
            'should return null after string editor is destroyed');
    });
}