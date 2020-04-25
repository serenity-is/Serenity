var $ = require("jQuery");
global.$ = global.jQuery = $;
var Q = require("SerenityCoreLib").Q;

test('works when jQuery script loaded', function() {
    expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});