
test('works when a dummy jQuery ui object exists', function() {
    var $ = require("jQuery");
    try {
        global.$ = global.jQuery = $;
        $.ui = {};
        var Q = require("SerenityCoreLib").Q;
        expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
    }
    finally {
        delete global.$;
        delete global.jQuery;
    }
});