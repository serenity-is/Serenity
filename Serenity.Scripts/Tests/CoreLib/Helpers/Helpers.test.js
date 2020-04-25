var __extends = require("SerenityCoreLibBase").__extends;


test('__extends skips special type system properties', function() {
    var baseObj = function() {
    }
    baseObj.__metadata = 1;
    baseObj.__typeName = 'B';
    baseObj.__componentFactory = 'C'
    baseObj.__ok = 'D';
    baseObj.some = 2;
    var d = function() {
    }
    __extends(d, baseObj);
    expect(d.__metadata).toBeUndefined();
    expect(d.__typeName).toBeUndefined();
    expect(d.__componentFactory).toBeUndefined();
    expect(d.__ok).toBe('D');
    expect(d.some).toBe(2);
});