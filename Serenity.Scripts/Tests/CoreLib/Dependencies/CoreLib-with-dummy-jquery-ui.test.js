var $ = require("jQuery");
global.$ = global.jQuery = $;
$.ui = {};
var Q = require("SerenityCoreLib").Q;

test('works when a dummy jQuery ui object exists', function() {
    expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});