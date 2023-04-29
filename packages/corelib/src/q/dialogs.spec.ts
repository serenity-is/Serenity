beforeEach(() => {
    jest.resetModules();
})

describe("Bootstrap version detection", () => {

    function setupDummyJQueryForModal(callback: (s: string | HTMLElement) => void) {
        jest.unmock('@optionaldeps/jquery')
        jest.mock('@optionaldeps/jquery', () => {
            return {
                __esModule: true,
                default: function dummyJQueryForModal(html: string | HTMLElement) {
                    return {
                        _html: html,
                        html: function () {
                            return <string>null;
                        },
                        eq: function () {
                            return this;
                        },
                        find: function () {
                            return this;
                        },
                        click: function () {
                        },
                        appendTo: function () {
                            return this;
                        },
                        one: function () {
                            return this;
                        },
                        modal: function () {
                            expect(this._html).toBe(html);
                            callback(html);
                        }
                    }
                } as any as JQueryStatic
            }
        });        
    }

    it('detects BS3 when modal version starts with 3', async function () {

        var passedHtml: string;
        setupDummyJQueryForModal(function (html) {
            passedHtml = typeof html === 'string' ? html : html.outerHTML;
        });
        try {
            var $ = (await import("@optionaldeps/jquery")).default as any;
            $.fn = {
                modal: {
                    Constructor: {
                        VERSION: '3.3.1'
                    }
                }
            }
            var dialogs = await import("./dialogs");
            dialogs.alertDialog("hello");

            expect(passedHtml).toBeDefined();

            var idx1 = passedHtml.indexOf('class="close"');
            var idx2 = passedHtml.indexOf('<h5');
            expect(idx1).toBeGreaterThan(-1);
            expect(idx2).toBeGreaterThan(idx1);
        }
        finally {
            delete globalThis.$;
            delete globalThis.jQuery;
        }
    });


    it('detects BS4 when modal version does not exist', async function () {
        var passedHtml: string;
        setupDummyJQueryForModal(function (html) {
            passedHtml = typeof html === 'string' ? html : html.outerHTML;
        });
        try {
            var $ = (await import("@optionaldeps/jquery")).default as any;
            ($ as any).fn = {
                modal: {
                }
            }

            var dialogs = await import("./dialogs");
            dialogs.alertDialog("hello");

            expect(passedHtml).toBeDefined();

            var idx1 = passedHtml.indexOf('class="close"');
            var idx2 = passedHtml.indexOf('<h5');
            expect(idx1).toBeGreaterThan(-1);
            expect(idx2).toBeGreaterThan(-1);
            expect(idx1).toBeGreaterThan(idx2);
        }
        finally {
            delete globalThis.$;
            delete globalThis.jQuery;
        }
    });

    it('detects BS4 when modal version is something other than 3', async function () {

        var passedHtml: string;
        setupDummyJQueryForModal(function (html) {
            passedHtml = typeof html === 'string' ? html : html.outerHTML;
        });
        try {
            var $ = (await import("@optionaldeps/jquery")).default as any;
            ($ as any).fn = {
                modal: {
                    Constructor: {
                        VERSION: '4.1.0'
                    }
                }
            }

            var dialogs = await import("./dialogs");
            dialogs.alertDialog("hello");

            expect(passedHtml).toBeDefined();

            var idx1 = passedHtml.indexOf('class="close"');
            var idx2 = passedHtml.indexOf('<h5');
            expect(idx1).toBeGreaterThan(-1);
            expect(idx2).toBeGreaterThan(-1);
            expect(idx1).toBeGreaterThan(idx2);
        }
        finally {
            delete globalThis.$;
            delete globalThis.jQuery;
        }
    });
});

describe("Bootstrap noConflict", function () {
    function setupDummyJQuery() {
        jest.unmock('@optionaldeps/jquery')
        jest.mock('@optionaldeps/jquery', () => {
            return {
                __esModule: true,
                default: Object.assign(function() {
                    return {
                        html: function (): any {
                            return null;
                        }
                    }
                },
                {
                    fn: {}
                }) as any
            }
        });                
    }

    function uiButton() {
    }

    function bsButton() {
    }

    it('does not call noConflict if no jQuery ui button widget', async function () {
        setupDummyJQuery();
        var $ = (await import("@optionaldeps/jquery")).default as any;
        var noConflictCalled = false;
        ($ as any).ui = {};
        ($ as any).fn.button = {
            noConflict: function () {
                noConflictCalled = true;
            }
        }
        await import("./dialogs");
        expect(noConflictCalled).toBe(false);
    });

    it('skips noConflict if button does not have noConflict method', async function () {
        setupDummyJQuery();
        var $ = (await import("@optionaldeps/jquery")).default as any;
        ($ as any).ui = {
            button: function () {
            }
        };
        ($ as any).fn.button = {
        }
        await import("./dialogs");
    });

    it('noConflict called if jQuery ui button widget exists and $.fn.button has noConflict method', async function () {
        setupDummyJQuery();
        var noConflictCalled = false;
        var $ = (await import("@optionaldeps/jquery")).default as any;
        (bsButton as any).noConflict = function () {
            noConflictCalled = true;
            ($ as any).fn.button = uiButton;
            return bsButton;
        };

        ($ as any).fn.button = bsButton;
        ($ as any).ui = {
            button: uiButton
        };
        await import("./dialogs");
        expect(noConflictCalled).toBe(true);
        expect(($.fn as any).button).toBe(uiButton);
        expect(($.fn as any).btn).toBe(bsButton);
    });
});


describe("Q.alertDialog", () => {
    it('Q.alertDialog uses window.alert when no BS/jQuery UI loaded', async function () {
        var alertCount = 0;
        var alertMessage = null;
        globalThis.alert = function (message) {
            alertCount++;
            alertMessage = message;
        }
        try {
            const alertDialog = (await import("./dialogs")).alertDialog;
            alertDialog('test message');
            expect(alertCount).toBe(1);
            expect(alertMessage).toBe('test message');
        }
        finally {
            delete globalThis.alert;
            delete globalThis.window;
        }
    });
});

describe("Q.informationDialog", () => {
    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        var alertCount = 0;
        var alertMessage = null;
        try {
            (globalThis as any).window = globalThis;
            globalThis.alert = function (message) {
                alertCount++;
                alertMessage = message;
            }

            const informationDialog = (await import("./dialogs")).informationDialog;
            informationDialog('test message', () => { });
            expect(alertCount).toBe(1);
            expect(alertMessage).toBe('test message');
        }
        finally {
            delete globalThis.alert;
            delete globalThis.window;
        }
    });
});

describe("Q.warningDialog", () => {

    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        var alertCount = 0;
        var alertMessage = null;
        (globalThis as any).window = globalThis;
        globalThis.alert = function (message) {
            alertCount++;
            alertMessage = message;
        }

        try {
            const warningDialog = (await import("./dialogs")).warningDialog;
            warningDialog('test message');
            expect(alertCount).toBe(1);
            expect(alertMessage).toBe('test message');
        }
        finally {
            delete globalThis.alert;
            delete globalThis.window;
        }
    });
});


describe("Q.confirmDialog", () => {
    it('uses window.confirm when no BS/jQuery UI loaded', async function () {
        var confirmCount = 0;
        var confirmMessage: string = null;
        (globalThis as any).window = globalThis;
        globalThis.confirm = function (message) {
            confirmCount++;
            confirmMessage = message;
            return true;
        }
        try {
            var onYesCalled;
            const confirmDialog = (await import("./dialogs")).confirmDialog;
            confirmDialog('test message', function () {
                onYesCalled = true;
            });
            expect(confirmCount).toBe(1);
            expect(confirmMessage).toBe('test message');
            expect(onYesCalled).toBe(true);
        }
        finally {
            delete globalThis.confirm;
            delete globalThis.window;
        }
    });
});

describe("Q.iframeDialog", () => {
    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        var alertCount = 0;
        var alertMessage = null;
        (globalThis as any).window = globalThis;
        globalThis.alert = function (message) {
            alertCount++;
            alertMessage = message;
        }
        try {
            var testHtml = '<html><body>test message<body></html>';
            const iframeDialog = (await import("./dialogs")).iframeDialog;
            iframeDialog({
                html: testHtml
            });
            expect(alertCount).toBe(1);
            expect(alertMessage).toBe(testHtml);
        }
        finally {
            delete globalThis.alert;
            delete globalThis.window;
        }
    });
});

export { }