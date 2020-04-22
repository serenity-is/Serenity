var $ = require("jQuery");
global.$ = global.jQuery = $;
$.ui = {};
require("SerenityCoreLib");

test('works when a dummy jQuery ui object exists', function() {
    expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});