beforeEach(() => {
    jest.resetModules();
});

afterEach(() => {
    jest.resetModules();
});

function setupDummyJQuery() {
    global.$ = global.jQuery = function() {
        return {
            html: function() {
                return null;
            },
        }
    }
    global.$.fn = {};
}

test('noConflict is not called if no jQuery ui button widget', function() {
    setupDummyJQuery();
    try {
        var noConflictCalled = false;
        global.$.ui = {};
        global.$.fn.button = {
            noConflict: function() {
                noConflictCalled = true;
            }
        }
        var Q = require("SerenityCoreLibBase").Q;
        expect(noConflictCalled).toBe(false);
    }
    finally {
        delete global.$;
        delete global.jQuery;
    }
});

test('noConflict skipped if button does not have noConflict method', function() {
    setupDummyJQuery();
    try {
        global.$.ui = { 
            button: function() {
            }
        };
        global.$.fn.button = {
        }
        var Q = require("SerenityCoreLibBase").Q;
    }
    finally {
        delete global.$;
        delete global.jQuery;
    }
});

test('noConflict called if jQuery ui button widget exists and $.fn.button has noConflict method', function() {
    setupDummyJQuery();
    try {
        var noConflictCalled = false;
        function uiButton() {
        }
        function bsButton() {
        }
        bsButton.noConflict = function() {
            noConflictCalled = true;
            global.$.fn.button = uiButton;
            return bsButton;
        }
        global.$.fn.button = bsButton;
        global.$.ui = {
            button: uiButton
        };
        var Q = require("SerenityCoreLibBase").Q;
        expect(noConflictCalled).toBe(true);
        expect($.fn.button).toBe(uiButton);
        expect($.fn.btn).toBe(bsButton);
    }
    finally {
        delete global.$;
        delete global.jQuery;
    }
});