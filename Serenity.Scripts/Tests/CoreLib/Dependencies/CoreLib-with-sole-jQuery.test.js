var $ = require("jQuery");

test('works when jQuery script loaded', function() {
    try {
        global.$ = global.jQuery = $;
        var Q = require("SerenityCoreLib").Q;
    
        expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
    }
    finally {
        delete global.$;
        delete global.jQuery;
    }
});