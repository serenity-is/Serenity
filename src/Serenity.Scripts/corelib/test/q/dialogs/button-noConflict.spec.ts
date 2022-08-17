export { }

function setupDummyJQuery() {
    global.$ = global.jQuery = function () {
        return {
            html: function (): any {
                return null;
            },
        }
    } as any;
    (global.$ as any).fn = {};
}

test('noConflict is not called if no jQuery ui button widget', function() {
    jest.isolateModules(function() {
        setupDummyJQuery();
        try {
            var noConflictCalled = false;
            (global.$ as any).ui = {};
            (global.$ as any).fn.button = {
                noConflict: function() {
                    noConflictCalled = true;
                }
            }
            require("@/q/dialogs");
            expect(noConflictCalled).toBe(false);
        }
        finally {
            delete global.$;
            delete global.jQuery;
        }
    });
});

test('noConflict skipped if button does not have noConflict method', function() {
    jest.isolateModules(function() {
        setupDummyJQuery();
        try {
            (global.$ as any).ui = { 
                button: function() {
                }
            };
            (global.$ as any).fn.button = {
            }
            require("@/q/dialogs");
        }
        finally {
            delete global.$;
            delete global.jQuery;
        }
    });
});

function uiButton() {
}

function bsButton() {
}

test('noConflict called if jQuery ui button widget exists and $.fn.button has noConflict method', function() {
    jest.isolateModules(function() {
        setupDummyJQuery();
        try {
            var noConflictCalled = false;

            (bsButton as any).noConflict = function () {
                noConflictCalled = true;
                (global.$ as any).fn.button = uiButton;
                return bsButton;
            };

            (global.$ as any).fn.button = bsButton;
            (global.$ as any).ui = {
                button: uiButton
            };
            require("@/q/dialogs");
            expect(noConflictCalled).toBe(true);
            expect($.fn.button).toBe(uiButton);
            expect(($.fn as any).btn).toBe(bsButton);
        }
        finally {
            delete global.$;
            delete global.jQuery;
        }
    });
});