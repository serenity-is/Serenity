export { }

function setupDummyJQuery() {
    globalThis.$ = globalThis.jQuery = function () {
        return {
            html: function (): any {
                return null;
            },
        }
    } as any;
    (globalThis.$ as any).fn = {};
}

test('noConflict is not called if no jQuery ui button widget', function() {
    jest.isolateModules(function() {
        setupDummyJQuery();
        try {
            var noConflictCalled = false;
            (globalThis.$ as any).ui = {};
            (globalThis.$ as any).fn.button = {
                noConflict: function() {
                    noConflictCalled = true;
                }
            }
            globalThis.require("@/q/dialogs");
            expect(noConflictCalled).toBe(false);
        }
        finally {
            delete globalThis.$;
            delete globalThis.jQuery;
        }
    });
});

test('noConflict skipped if button does not have noConflict method', function() {
    jest.isolateModules(function() {
        setupDummyJQuery();
        try {
            (globalThis.$ as any).ui = { 
                button: function() {
                }
            };
            (globalThis.$ as any).fn.button = {
            }
            globalThis.require("@/q/dialogs");
        }
        finally {
            delete globalThis.$;
            delete globalThis.jQuery;
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
                (globalThis.$ as any).fn.button = uiButton;
                return bsButton;
            };

            (globalThis.$ as any).fn.button = bsButton;
            (globalThis.$ as any).ui = {
                button: uiButton
            };
            globalThis.require("@/q/dialogs");
            expect(noConflictCalled).toBe(true);
            expect($.fn.button).toBe(uiButton);
            expect(($.fn as any).btn).toBe(bsButton);
        }
        finally {
            delete globalThis.$;
            delete globalThis.jQuery;
        }
    });
});