var $ = require("jQuery");
global.$ = global.jQuery = $;
require("SerenityCoreLib");

test('works when jQuery script loaded', function() {
    expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});