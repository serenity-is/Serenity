beforeEach(() => {
    jest.resetModules();
})

function setupMockBootstrap5PlusWithNoJQuery() {
    jest.unmock('@optionaldeps/jquery')
    jest.mock('@optionaldeps/jquery', () => {
        return {
            __esModule: true,
            default: undefined
        }
    });
    jest.unmock('@optionaldeps/bootstrap')
    jest.mock('@optionaldeps/bootstrap', () => {
        var bs = {
            Modal: function (div: HTMLElement, opt?: { backdrop?: boolean }) {
                this.div = div;
                this.opt = opt;
                this.div && (this.div.dataset.options = JSON.stringify(opt));
                this.show = function () {
                    this.div.dataset.shown = (parseInt(this.div.dataset.shown ?? "0", 10) + 1).toString();
                };
                this.hide = function () {
                    this.div.dataset.hidden = (parseInt(this.div.dataset.hidden ?? "0", 10) + 1).toString();
                }
            }
        } as any;

        bs.Modal.VERSION = "5.3.2";
        bs.Modal.getInstance = function(el: HTMLElement) {
            if (el && el.dataset.options) {
                return {
                    opt: JSON.parse(el.dataset.options),
                    show: function () {
                        el.dataset.shown = (parseInt(el.dataset.shown ?? "0", 10) + 1).toString();
                    },
                    hide: function () {
                        el.dataset.hidden = (parseInt(el.dataset.hidden ?? "0", 10) + 1).toString();
                    }
                }
            }
        }

        return {
            __esModule: true,
            default: bs
        } as any;
    });
}

/*
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
                default: Object.assign(function () {
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
        var alertSpy = jest.spyOn(window, "alert");
        try {
            const alertDialog = (await import("./dialogs")).alertDialog;
            alertDialog('test message');
            expect(alertSpy).toBeCalledTimes(1);
            expect(alertSpy).toBeCalledWith("test message");
        }
        finally {
            alertSpy.mockRestore();
        }
    });
});

describe("Q.informationDialog", () => {
    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        var alertSpy = jest.spyOn(window, "alert");
        try {
            const informationDialog = (await import("./dialogs")).informationDialog;
            informationDialog('test message', () => { });
            expect(alertSpy).toBeCalledTimes(1);
            expect(alertSpy).toBeCalledWith("test message");
        }
        finally {
            alertSpy.mockRestore();
        }
    });
});

describe("Q.warningDialog", () => {

    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        var alertSpy = jest.spyOn(window, "alert");
        try {        
            const warningDialog = (await import("./dialogs")).warningDialog;
            warningDialog('test message');
            expect(alertSpy).toBeCalledTimes(1);
            expect(alertSpy).toBeCalledWith("test message");
        }
        finally {
            alertSpy.mockRestore();
        }
    });
});


describe("Q.confirmDialog", () => {
    it('uses window.confirm when no BS/jQuery UI loaded', async function () {
        var confirmSpy = jest.spyOn(window, "confirm");
        try {
            var onYesCalled;
            const confirmDialog = (await import("./dialogs")).confirmDialog;
            confirmDialog('test message', function () {
                onYesCalled = true;
            });
            expect(confirmSpy).toBeCalledTimes(1);
            expect(confirmSpy).toBeCalledWith("test message");            
            expect(onYesCalled).toBe(true);
        }
        finally {
            confirmSpy.mockRestore();
        }
    });
});

describe("Q.iframeDialog", () => {
    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        var alertSpy = jest.spyOn(window, "alert");
        try {
            var testHtml = '<html><body>test message<body></html>';
            const iframeDialog = (await import("./dialogs")).iframeDialog;
            iframeDialog({
                html: testHtml
            });
            expect(alertSpy).toBeCalledTimes(1);
            expect(alertSpy).toBeCalledWith(testHtml);
        }
        finally {
            alertSpy.mockRestore();
        }
    });
});
*/

describe("dialog button icon handling", () => {
    it("auto prefixes icons with 'fa-' prefix with 'fa' with BS5+ and no JQuery", async function () {
        setupMockBootstrap5PlusWithNoJQuery();
        const dialogs = await import("./dialogs");
        dialogs.alertDialog("test", {
            buttons: [{
                result: "ok",
                text: "ok",
                icon: "fa-test"
            }]
        });
        try {
            var i = document.querySelector(".modal-footer button i");
            expect(i != null).toBe(true);
            expect(i.classList.contains("fa-test")).toBe(true);
            expect(i.classList.contains("fa")).toBe(true);
        }
        finally {
            document.querySelector(".modal")?.remove();
        }
    });
});

describe("dialogButtonToBS", () => {
    it("converts dialog button to BS5+ button", async function () {
        setupMockBootstrap5PlusWithNoJQuery();
        const dialogs = await import("./dialogs");
        var button = dialogs.dialogButtonToBS({
            result: "ok",
            cssClass: "btn-success",
            text: "ok",
            icon: "fa-test"
        });
        expect(button != null).toBe(true);
        expect(button.className).toBe("btn btn-success");
        var i = button.querySelector("i");
        expect(i.classList.contains("fa-test")).toBe(true);
        expect(i.classList.contains("fa")).toBe(true);
    });

    it("can work without icons", async function () {
        setupMockBootstrap5PlusWithNoJQuery();
        const dialogs = await import("./dialogs");
        var button = dialogs.dialogButtonToBS({
            result: "ok",
            cssClass: "btn-success",
            text: "ok"
        });
        expect(button != null).toBe(true);
        expect(button.className).toBe("btn btn-success");
        var i = button.querySelector("i");
        expect(i).toBeNull();
    });

    it("adds 'glyphicon' class when icon starts with 'glyphicon-'", async function () {
        setupMockBootstrap5PlusWithNoJQuery();
        const dialogs = await import("./dialogs");
        var button = dialogs.dialogButtonToBS({
            result: "ok",
            icon: 'glyphicon-test',
            text: "ok"
        });
        expect(button != null).toBe(true);
        var i = button.querySelector("i");
        expect(i.classList.contains("glyphicon-test")).toBe(true);
        expect(i.classList.contains("glyphicon")).toBe(true);
    });

    it("returns other icon classes as is", async function () {
        setupMockBootstrap5PlusWithNoJQuery();
        const dialogs = await import("./dialogs");
        var button = dialogs.dialogButtonToBS({
            result: "ok",
            icon: 'xy-some-icon',
            text: "ok"
        });
        expect(button != null).toBe(true);
        var i = button.querySelector("i");
        expect(i.className).toBe("xy-some-icon");
    });    
});

export { }