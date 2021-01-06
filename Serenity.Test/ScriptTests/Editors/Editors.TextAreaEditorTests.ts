namespace Serenity.Test {
    let assert: Assert = QUnit.assert;
    QUnit.module('Editors');

    QUnit.test('TextAreaEditor creation tests', function () {
        assert.notEqual(null, new TextAreaEditor($('<textarea/>')),
            'create on a textarea with undefined options');

        assert.notEqual(null, new TextAreaEditor($('<textarea/>'), {}),
            'create on a textarea with empty options');

        let txtarea = $('<textarea/>');
        new TextAreaEditor(txtarea, null);
        assert.equal(txtarea.attr('cols'), 80,
            'should have 80 cols by default when options is undefined');
        assert.equal(txtarea.attr('rows'), 6,
            'should have 6 rows by default when options is undefined');

        txtarea = $('<textarea/>');
        new TextAreaEditor(txtarea, {});
        assert.equal(txtarea.attr('cols'), 80,
            'should have 80 cols by default when options is empty');
        assert.equal(txtarea.attr('rows'), 6,
            'should have 6 rows by default when options is empty');
    
        txtarea = $('<textarea/>');
        new TextAreaEditor(txtarea, {
            cols: 77
        });
        assert.equal(txtarea.attr('cols'), 77,
            'respects cols option');

        txtarea = $('<textarea/>');
        new TextAreaEditor(txtarea, {
            rows: 9
        });
        assert.equal(txtarea.attr('rows'), 9,
            'respects rows option');
    });
}