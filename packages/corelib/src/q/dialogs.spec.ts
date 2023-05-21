import type { CommonDialogOptions } from "./dialogs";

beforeEach(() => {
    jest.resetModules();
    jest.unmock('@optionaldeps/jquery');
    jest.unmock('@optionaldeps/bootstrap');
    jest.unmock('./localtext');
})

function mockUndefinedJQuery() {
    jest.mock('@optionaldeps/jquery', () => {
        return {
            __esModule: true,
            default: undefined
        }
    });
}

function mockUndefinedBS5Plus() {
    jest.mock('@optionaldeps/bootstrap', () => {
        return {
            __esModule: true,
            default: undefined
        }
    });
}

function mockEnvironmentWithBrowserDialogsOnly() {
    mockUndefinedBS5Plus();
    mockUndefinedJQuery();
}

function mockBS5Plus() {
    jest.mock('@optionaldeps/bootstrap', () => {
        var modal = jest.fn(function (div: HTMLElement) {
            return {
                show: jest.fn(function () {
                    div.dataset.showCalls = (parseInt(div.dataset.showCalls ?? "0", 10) + 1).toString();
                }),
                hide: jest.fn(function () {
                    div.dataset.hideCalls = (parseInt(div.dataset.hideClass ?? "0", 10) + 1).toString();
                })
            }
        }) as any;

        modal.VERSION = "5.3.2";
        modal.getInstance = function (el: HTMLElement) {
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
            default: {
                Modal: modal
            }
        } as any;
    });
}

function mockBS5PlusWithUndefinedJQuery() {
    mockUndefinedJQuery();
    mockBS5Plus();
}

function mockJQuery(fn: any) {
    var jQuery = function (selector: string | HTMLElement) {
        var result = {
            _selector: selector,
            _selectorHtml: typeof selector === 'string' ? selector : selector.outerHTML,
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
            on: function (ev: string, handler: () => void) {
                this._selector?.addEventListener?.(ev, handler);
                return this;
            },
            one: function (ev: string, handler: () => void) {
                this._selector?.addEventListener?.(ev, handler);
                return this;
            },
            destroy: function () {
                return this;
            },
            remove: function () {
                return this;
            }
        } as any;
        if (fn != null) {
            for (var x of Object.keys(fn)) {
                result[x] = fn[x];
            }
        }
        return result;
    } as any;

    jQuery.fn = fn;

    jest.mock('@optionaldeps/jquery', () => {
        return {
            __esModule: true,
            default: jQuery
        }
    });

    return jQuery;
}

function mockJQueryWithBootstrapModal() {
    var modal = jest.fn(function () {
        return this;
    }) as any;

    modal.Constructor = {
        VERSION: null
    };

    mockJQuery({
        modal
    });
}

function mockJQueryWithUIDialog() {
    var dialog = jest.fn(function () {
        return this;
    }) as any;

    var $ = mockJQuery({
        dialog
    });

    $.ui = {
        dialog: dialog
    }
}

describe("Bootstrap version detection", () => {

    it('detects BS3 when modal version starts with 3', async function () {
        mockJQueryWithBootstrapModal();
        var $ = (await import("@optionaldeps/jquery")).default as any;
        $.fn.modal.Constructor.VERSION = "3.3.1";
        var dialogs = await import("./dialogs");
        dialogs.alertDialog("hello");
        expect($.fn.modal).toHaveBeenCalledTimes(1);
        var html = $.fn.modal.mock?.contexts[0]?._selectorHtml;
        expect(html).toBeDefined();
        var idx1 = html.indexOf('class="close"');
        var idx2 = html.indexOf('<h5');
        expect(idx1).toBeGreaterThan(-1);
        expect(idx2).toBeGreaterThan(idx1);
        expect(dialogs.isBS3()).toBe(true);
        expect(dialogs.isBS5Plus()).toBe(false);
    });


    it('detects BS4 when modal version does not exist', async function () {
        mockJQueryWithBootstrapModal();
        var $ = (await import("@optionaldeps/jquery")).default as any;
        delete $.fn.modal.Constructor;
        var dialogs = await import("./dialogs");
        var opt = {
            onOpen: jest.fn(),
            onClose: jest.fn()
        };
        dialogs.alertDialog("hello", opt);
        expect($.fn.modal).toHaveBeenCalledTimes(1);
        var instance = $.fn.modal.mock?.contexts[0];
        var div = instance._selector;
        expect(div).toBeDefined();
        expect(div instanceof HTMLDivElement).toBe(true);
        var html = instance?._selectorHtml;
        var idx1 = html.indexOf('class="close"');
        var idx2 = html.indexOf('<h5');
        expect(idx1).toBeGreaterThan(-1);
        expect(idx2).toBeGreaterThan(-1);
        expect(idx1).toBeGreaterThan(idx2);
        const shownEvent = new Event("shown.bs.modal");
        div.dispatchEvent(shownEvent);
        expect(opt.onOpen).toBeCalledTimes(1);
        expect(opt.onClose).not.toBeCalled();
        expect(opt.onOpen.mock?.contexts?.[0]).toBeDefined();
        const hiddenEvent = new Event("hidden.bs.modal");
        div.dispatchEvent(hiddenEvent);
        expect(opt.onClose).toBeCalledTimes(1);
        expect(opt.onClose.mock?.contexts?.[0]).toBeDefined();
    });

    it('detects BS4 when modal version is something other than 3', async function () {

        mockJQueryWithBootstrapModal();
        var $ = (await import("@optionaldeps/jquery")).default as any;
        var $ = (await import("@optionaldeps/jquery")).default as any;
        $.fn.modal.Constructor.VERSION = '4.1.0';
        var dialogs = await import("./dialogs");
        dialogs.alertDialog("hello");
        expect($.fn.modal).toHaveBeenCalledTimes(1);
        var html = $.fn.modal.mock?.contexts[0]?._selectorHtml;
        expect(html).toBeDefined();
        var idx1 = html.indexOf('class="close"');
        var idx2 = html.indexOf('<h5');
        expect(idx1).toBeGreaterThan(-1);
        expect(idx2).toBeGreaterThan(-1);
        expect(idx1).toBeGreaterThan(idx2);
    });
});

describe("Bootstrap noConflict", function () {
    function setupDummyJQuery() {
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
        $.ui = {};
        $.fn.button = {
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
        $.ui = {
            button: function () {
            }
        };
        $.fn.button = {
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

        $.fn.button = bsButton;
        $.ui = {
            button: uiButton
        };
        await import("./dialogs");
        expect(noConflictCalled).toBe(true);
        expect(($.fn as any).button).toBe(uiButton);
        expect(($.fn as any).btn).toBe(bsButton);
    });

    it('ignores when no $.fn', async function () {
        setupDummyJQuery();
        var noConflictCalled = false;
        var $ = (await import("@optionaldeps/jquery")).default as any;
        delete $.fn;
        $.ui = {
            button: uiButton
        };
        await import("./dialogs");
        expect(noConflictCalled).toBe(false);
    });

    it('ignores when no $.ui', async function () {
        setupDummyJQuery();
        var noConflictCalled = false;
        var $ = (await import("@optionaldeps/jquery")).default as any;
        delete $.ui;
        await import("./dialogs");
        expect(noConflictCalled).toBe(false);
    });

    it('ignores when no $.ui.button', async function () {
        setupDummyJQuery();
        var noConflictCalled = false;
        var $ = (await import("@optionaldeps/jquery")).default as any;
        $.ui = {};
        await import("./dialogs");
        expect(noConflictCalled).toBe(false);
    });

    it('ignores when no $.fn.button.noConflict', async function () {
        setupDummyJQuery();
        var noConflictCalled = false;
        var $ = (await import("@optionaldeps/jquery")).default as any;
        $.fn.button = bsButton;
        $.ui = {
            button: uiButton
        };
        await import("./dialogs");
        expect(noConflictCalled).toBe(false);
    });
});


describe("Q.alertDialog", () => {
    it('Q.alertDialog uses window.alert when no BS/jQuery UI loaded', async function () {
        var alertSpy = jest.spyOn(window, "alert");
        try {
            mockEnvironmentWithBrowserDialogsOnly();
            alertSpy.mockImplementation(() => { });
            const dialogs = (await import("./dialogs"));
            dialogs.alertDialog('test message');
            expect(alertSpy).toBeCalledTimes(1);
            expect(alertSpy).toBeCalledWith("test message");
        }
        finally {
            alertSpy.mockRestore();
        }
    });

    it('calls jQuery ui dialog with expected parameters', async function () {
        mockJQueryWithUIDialog();
        var $ = (await import("@optionaldeps/jquery")).default as any;
        const dialogs = await import("./dialogs");
        var opt = {
            onOpen: jest.fn(),
            onClose: jest.fn()
        };
        dialogs.alertDialog("hello", opt);
        expect($.fn.dialog).toHaveBeenCalledTimes(1);
        var x = $.fn.dialog.mock?.calls?.[0]?.[0] as any & CommonDialogOptions;
        expect(x).toBeDefined();
        expect(x.title).toBe("Alert");
        expect(x.modal).toBe(true);
        expect(x.buttons.length).toEqual(1);
        expect(x.buttons[0].text).toEqual("OK");
        expect(typeof x.buttons[0].click).toBe("function");
        expect(typeof x.close).toBe("function");
        expect(x.dialogClass).toBe("s-MessageDialog s-AlertDialog");
        expect(x.width).toBe("40%");
        expect(x.maxWidth).toBe(450);
        expect(x.minWidth).toBe(180);
        expect(x.modal).toBe(true);
        expect(typeof x.open).toBe("function");
        expect(x.resizable).toBe(false);
        x.open();
        expect(opt.onOpen).toBeCalledTimes(1);
        expect(opt.onClose).not.toBeCalled();
        expect(opt.onOpen.mock?.contexts?.[0]).toBeDefined();
        x.close();
        expect(opt.onClose).toBeCalledTimes(1);
        expect(opt.onClose.mock?.contexts?.[0]).toBeDefined();
        x.buttons[0].click();
        expect((opt as any).result).toBe("ok");
    });

    it('returns expected bootstrap.Modal markup', async function () {
        mockBS5PlusWithUndefinedJQuery();
        var bootstrap = (await import("@optionaldeps/bootstrap")).default as any;
        const dialogs = await import("./dialogs");
        var opt = {
            onOpen: jest.fn(),
            onClose: jest.fn()
        };
        dialogs.alert("hello", opt);
        var modal = document.querySelector(".modal");
        try {
            expect(modal).not.toBeNull();
            expect(modal.classList).toContain("modal");
            expect(modal.classList).toContain("s-MessageModal");
            expect(modal.classList).toContain("s-AlertModal");
            expect(modal.getAttribute("tabIndex")).toBe("-1");
            expect(modal.getAttribute("role")).toBe("dialog");
            var modalDialog = modal.querySelector(".modal-dialog");
            expect(modalDialog).not.toBeNull();
            expect(modalDialog.parentElement).toBe(modal);
            expect(modalDialog.getAttribute("role")).toBe("document");
            var modalContent = modalDialog.querySelector(".modal-content");
            expect(modalContent).not.toBeNull();
            expect(modalContent.parentElement).toBe(modalDialog);
            var modalHeader = modalContent.querySelector(".modal-header");
            expect(modalHeader).not.toBeNull();
            expect(modalHeader.parentElement).toBe(modalContent);
            var modalTitle = modalContent.querySelector(".modal-title");
            expect(modalTitle.tagName).toBe("H5");
            var button = modalContent.querySelector(".modal-footer button");
            expect(button).not.toBeNull();
            expect(button.classList).toContain("btn");
            expect(button.classList).toContain("btn-danger");
            expect(button.textContent).toBe("OK");

            expect(bootstrap.Modal).toHaveBeenCalledTimes(1);
            var div = bootstrap.Modal.mock.calls?.[0]?.[0] as HTMLDivElement;
            expect(div).toBeDefined();
            expect(div?.dataset?.showCalls).toBe("1");
            const shownEvent = new Event("shown.bs.modal");
            div.dispatchEvent(shownEvent);
            expect(opt.onOpen).toBeCalledTimes(1);
            expect(opt.onClose).not.toBeCalled();
            expect(opt.onOpen.mock?.contexts?.[0]).toBeDefined();
            const hiddenEvent = new Event("hidden.bs.modal");
            div.dispatchEvent(hiddenEvent);
            expect(opt.onClose).toBeCalledTimes(1);
            expect(opt.onClose.mock?.contexts?.[0]).toBeDefined();
            const clickEvent = new Event("click");
            button.dispatchEvent(clickEvent);
            expect((opt as any).result).toBe("ok");
            expect(modal.getRootNode()).toBe(modal);
        }
        finally {
            modal?.remove();
        }
    });

    it('calls tryGetText for title', async function () {
        mockJQueryWithUIDialog();
        jest.mock("./localtext", () => {
            return {
                __esModule: true,
                localText: (s: string) => "Local" + s,
                tryGetText: (s: string) => "Local" + s
            }
        });
        var $ = (await import("@optionaldeps/jquery")).default as any;
        const dialogs = await import("./dialogs");
        var opt = {
            onOpen: jest.fn(),
            onClose: jest.fn()
        };
        dialogs.alertDialog("hello", opt);
        expect($.fn.dialog).toHaveBeenCalledTimes(1);
        var x = $.fn.dialog.mock?.calls?.[0]?.[0] as any & CommonDialogOptions;
        expect(x).toBeDefined();
        expect(x.title).toBe("LocalDialogs.AlertTitle");
    });

});

describe("Q.informationDialog", () => {
    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        mockEnvironmentWithBrowserDialogsOnly();
        var alertSpy = jest.spyOn(window, "alert");
        try {
            alertSpy.mockImplementation(() => { });
            const dialogs = (await import("./dialogs"));
            dialogs.informationDialog('test message', () => { });
            expect(alertSpy).toBeCalledTimes(1);
            expect(alertSpy).toBeCalledWith("test message");
        }
        finally {
            alertSpy.mockRestore();
        }
    });


    it('is aliased by obsolete inform', async function () {
        const dialogs = (await import("./dialogs"));
        expect(dialogs.information).toBe(dialogs.informationDialog);
    });        
});

describe("Q.warningDialog", () => {

    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        mockEnvironmentWithBrowserDialogsOnly();
        var alertSpy = jest.spyOn(window, "alert");
        try {
            alertSpy.mockImplementation(() => { });
            const dialogs = (await import("./dialogs"));
            dialogs.warningDialog('test message');
            expect(alertSpy).toBeCalledTimes(1);
            expect(alertSpy).toBeCalledWith("test message");
        }
        finally {
            alertSpy.mockRestore();
        }
    });

    it('is aliased by obsolete warning', async function () {
        const dialogs = (await import("./dialogs"));
        expect(dialogs.warning).toBe(dialogs.warningDialog);
    });
});

describe("Q.confirmDialog", () => {
    it('uses window.confirm when no BS/jQuery UI loaded', async function () {
        mockEnvironmentWithBrowserDialogsOnly();
        var confirmSpy = jest.spyOn(window, "confirm");
        try {
            confirmSpy.mockImplementation(() => true);
            var onYesCalled;
            const dialogs = (await import("./dialogs"));
            dialogs.confirmDialog('test message', function () {
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

    it('calls jQuery ui dialog with expected parameters', async function () {
        mockJQueryWithUIDialog();
        var $ = (await import("@optionaldeps/jquery")).default as any;
        const dialogs = await import("./dialogs");
        var opt = {
            onOpen: jest.fn(),
            onClose: jest.fn()
        };
        var onYes = jest.fn(function () {
        });
        dialogs.confirmDialog("hello", onYes, opt);
        expect($.fn.dialog).toHaveBeenCalledTimes(1);
        var x = $.fn.dialog.mock?.calls?.[0]?.[0] as any & CommonDialogOptions;
        expect(x).toBeDefined();
        expect(x.title).toBe("Confirm");
        expect(x.modal).toBe(true);

        expect(x.buttons.length).toEqual(2);
        expect(x.buttons[0].text).toEqual("Yes");
        expect(typeof x.buttons[0].click).toBe("function");

        expect(x.buttons[1].text).toEqual("No");
        expect(typeof x.buttons[1].click).toBe("function");

        expect(typeof x.close).toBe("function");
        expect(x.dialogClass).toBe("s-MessageDialog s-ConfirmDialog");
        expect(x.width).toBe("40%");
        expect(x.maxWidth).toBe(450);
        expect(x.minWidth).toBe(180);
        expect(x.modal).toBe(true);
        expect(typeof x.open).toBe("function");
        expect(x.resizable).toBe(false);
        x.open();
        expect(opt.onOpen).toBeCalledTimes(1);
        expect(opt.onClose).not.toBeCalled();
        expect(opt.onOpen.mock?.contexts?.[0]).toBeDefined();
        x.close();
        expect(opt.onClose).toBeCalledTimes(1);
        expect(opt.onClose.mock?.contexts?.[0]).toBeDefined();
        x.buttons[0].click();
        expect((opt as any).result).toBe("yes");
        expect(onYes).toBeCalledTimes(1);
    });

    it('returns expected bootstrap.Modal markup with BS5+ and no JQuery', async function () {
        mockBS5PlusWithUndefinedJQuery();
        var bootstrap = (await import("@optionaldeps/bootstrap")).default as any;
        const dialogs = await import("./dialogs");
        var opt = {
            onOpen: jest.fn(),
            onClose: jest.fn(),
            cancelButton: true
        };
        var onYes = jest.fn(function () {
        });
        dialogs.confirm("hello", onYes, opt);
        var modal = document.querySelector(".modal");
        try {
            expect(modal).not.toBeNull();
            expect(modal.classList).toContain("modal");
            expect(modal.classList).toContain("s-MessageModal");
            expect(modal.classList).toContain("s-ConfirmModal");
            expect(modal.getAttribute("tabIndex")).toBe("-1");
            expect(modal.getAttribute("role")).toBe("dialog");
            var modalDialog = modal.querySelector(".modal-dialog");
            expect(modalDialog).not.toBeNull();
            expect(modalDialog.parentElement).toBe(modal);
            expect(modalDialog.getAttribute("role")).toBe("document");
            var modalContent = modalDialog.querySelector(".modal-content");
            expect(modalContent).not.toBeNull();
            expect(modalContent.parentElement).toBe(modalDialog);
            var modalHeader = modalContent.querySelector(".modal-header");
            expect(modalHeader).not.toBeNull();
            expect(modalHeader.parentElement).toBe(modalContent);
            var modalTitle = modalContent.querySelector(".modal-title");
            expect(modalTitle.tagName).toBe("H5");

            var buttons = modalContent.querySelectorAll(".modal-footer button");
            expect(buttons.length).toBe(3);

            var yesButton = buttons[0];
            expect(yesButton.classList).toContain("btn");
            expect(yesButton.classList).toContain("btn-primary");
            expect(yesButton.textContent).toBe("Yes");

            var noButton = buttons[1];
            expect(noButton.classList).toContain("btn");
            expect(noButton.classList).toContain("btn-danger");
            expect(noButton.textContent).toBe("No");

            expect(bootstrap.Modal).toHaveBeenCalledTimes(1);
            var div = bootstrap.Modal.mock.calls?.[0]?.[0] as HTMLDivElement;
            expect(div).toBeDefined();
            expect(div?.dataset?.showCalls).toBe("1");
            const shownEvent = new Event("shown.bs.modal");
            div.dispatchEvent(shownEvent);
            expect(opt.onOpen).toBeCalledTimes(1);
            expect(opt.onClose).not.toBeCalled();
            expect(opt.onOpen.mock?.contexts?.[0]).toBeDefined();
            const hiddenEvent = new Event("hidden.bs.modal");
            div.dispatchEvent(hiddenEvent);
            expect(opt.onClose).toBeCalledTimes(1);
            expect(opt.onClose.mock?.contexts?.[0]).toBeDefined();
            const clickEvent = new Event("click");
            yesButton.dispatchEvent(clickEvent);
            expect((opt as any).result).toBe("yes");
            expect(modal.getRootNode()).toBe(modal);
        }
        finally {
            modal?.remove();
        }
    });

    it('is aliased by obsolete confirm', async function () {
        const dialogs = (await import("./dialogs"));
        expect(dialogs.confirm).toBe(dialogs.confirmDialog);
    });
});

describe("Q.successDialog", () => {
    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        mockEnvironmentWithBrowserDialogsOnly();
        var alertSpy = jest.spyOn(window, "alert");
        try {
            alertSpy.mockImplementation(() => { });
            var onOKCalled;
            const dialogs = (await import("./dialogs"));
            dialogs.successDialog('test message', () => {
                onOKCalled = true;
            });
            expect(alertSpy).toBeCalledTimes(1);
            expect(alertSpy).toBeCalledWith("test message");
            expect(onOKCalled).toBe(true);
        }
        finally {
            alertSpy.mockRestore();
        }
    });

    it('is aliased by obsolete success', async function () {
        const dialogs = (await import("./dialogs"));
        expect(dialogs.success).toBe(dialogs.successDialog);
    });    
});


describe("Q.iframeDialog", () => {
    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        mockEnvironmentWithBrowserDialogsOnly();
        var alertSpy = jest.spyOn(window, "alert");
        try {
            alertSpy.mockImplementation(() => { });
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

    it('calls jQuery ui dialog with expected parameters', async function () {
        mockJQueryWithUIDialog();
        var $ = (await import("@optionaldeps/jquery")).default as any;
        const dialogs = await import("./dialogs");
        var opt = {
            html: "<span>test</span>"
        };
        dialogs.iframeDialog(opt);
        expect($.fn.dialog).toHaveBeenCalledTimes(1);
        var x = $.fn.dialog.mock?.calls?.[0]?.[0] as any & CommonDialogOptions;
        expect(x).toBeDefined();
        expect(x.title).toBe("Alert");
        expect(x.modal).toBe(true);

        expect(x.buttons).toBeUndefined();

        expect(typeof x.close).toBe("function");
        expect(x.dialogClass).toBeUndefined();
        expect(x.width).toBe("60%");
        expect(x.height).toBe("400");
        expect(x.modal).toBe(true);
        expect(typeof x.open).toBe("function");
        expect(x.resizable).toBeUndefined();
        x.open();
    });

    it('returns expected bootstrap.Modal markup', async function () {
        mockBS5PlusWithUndefinedJQuery();
        var bootstrap = (await import("@optionaldeps/bootstrap")).default as any;
        const dialogs = await import("./dialogs");
        var opt = {
            html: "<span>test</span>",
        };
        dialogs.iframeDialog(opt);
        var modal = document.querySelector(".modal");
        try {
            expect(modal).not.toBeNull();
            expect(modal.classList).toContain("modal");
            expect(modal.getAttribute("tabIndex")).toBe("-1");
            expect(modal.getAttribute("role")).toBe("dialog");
            var modalDialog = modal.querySelector(".modal-dialog");
            expect(modalDialog).not.toBeNull();
            expect(modalDialog.parentElement).toBe(modal);
            expect(modalDialog.getAttribute("role")).toBe("document");
            var modalContent = modalDialog.querySelector(".modal-content");
            expect(modalContent).not.toBeNull();
            expect(modalContent.parentElement).toBe(modalDialog);
            var modalHeader = modalContent.querySelector(".modal-header");
            expect(modalHeader).not.toBeNull();
            expect(modalHeader.parentElement).toBe(modalContent);
            var modalTitle = modalContent.querySelector(".modal-title");
            expect(modalTitle.tagName).toBe("H5");

            var buttons = modalContent.querySelectorAll(".modal-footer button");
            expect(buttons.length).toBe(0);
            expect(bootstrap.Modal).toHaveBeenCalledTimes(1);
            var div = bootstrap.Modal.mock.calls?.[0]?.[0] as HTMLDivElement;
            expect(div).toBeDefined();
            expect(div?.dataset?.showCalls).toBe("1");
            const shownEvent = new Event("shown.bs.modal");
            div.dispatchEvent(shownEvent);
            const hiddenEvent = new Event("hidden.bs.modal");
            div.dispatchEvent(hiddenEvent);
        }
        finally {
            modal?.remove();
        }
    });
});

describe("dialog button icon handling", () => {
    it("auto prefixes icons with 'fa-' prefix with 'fa' with BS5+ and no JQuery", async function () {
        mockBS5PlusWithUndefinedJQuery();
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
        mockBS5PlusWithUndefinedJQuery();
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
        mockBS5PlusWithUndefinedJQuery();
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
        mockBS5PlusWithUndefinedJQuery();
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
        mockBS5PlusWithUndefinedJQuery();
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

    it("html encodes by default", async function () {
        const dialogs = (await import("./dialogs"));
        var button = dialogs.dialogButtonToBS({ text: "<div>x</div>" });
        expect(button.textContent).toBe("<div>x</div>");
    });

    it("html encodes when htmlEncode: undefined", async function () {
        const dialogs = (await import("./dialogs"));
        var button1 = dialogs.dialogButtonToBS({ text: "<div>x</div>", htmlEncode: undefined });
        expect(button1.textContent).toBe("<div>x</div>");
        var button2 = dialogs.dialogButtonToBS({ text: "<div>x</div>", htmlEncode: undefined });
        expect(button2.textContent).toBe("<div>x</div>");
    });

    it("can skip htmlencode with htmlEncode: false", async function() {
        const dialogs = (await import("./dialogs"));
        var button = dialogs.dialogButtonToBS({ text: "<div>x</div>", htmlEncode: false });
        expect(button.innerHTML).toBe("<div>x</div>");
    });

    it("can set button title to hint", async function() {
        const dialogs = (await import("./dialogs"));
        var button = dialogs.dialogButtonToBS({ text: "x", hint: "test" });
        expect(button.getAttribute("title")).toBe("test");
    });
});

describe("bsModalMarkup", () => {
    it("html encodes by default", async function () {
        const dialogs = (await import("./dialogs"));
        var div = dialogs.bsModalMarkup("test", "<div>x</div>", null);
        expect(div.className).toBe("modal");
        expect(div.querySelector(".modal-body")?.textContent).toBe("<div>x</div>");
    });
});


describe("dialogButtonToUI", () => {
    it("html encodes by default", async function () {
        const dialogs = (await import("./dialogs"));
        var button = dialogs.dialogButtonToUI({ text: "<div>x</div>" });
        expect(button.text).toBe("&lt;div&gt;x&lt;/div&gt;");
    });

    it("does not html encode if htmlEncode is false", async function () {
        const dialogs = (await import("./dialogs"));
        var button = dialogs.dialogButtonToUI({ text: "<div>x</div>", htmlEncode: false });
        expect(button.text).toBe("<div>x</div>");
    });

    it("sets css class", async function () {
        const dialogs = (await import("./dialogs"));
        var button = dialogs.dialogButtonToUI({ text: "<div>x</div>", cssClass: "x" });
        expect(button.cssClass).toBe("x");
    });

    it("adds the icon", async function () {
        const dialogs = (await import("./dialogs"));
        var button = dialogs.dialogButtonToUI({ text: "test",  icon: "x" });
        expect(button.text).toBe('<i class="x"></i> test');
    });
});

describe("closePanel", () => {
    it("ignores when element is null or undefined", async function () {
        var dialogs = (await import("./dialogs"));
        dialogs.closePanel(null);
        expect(true).toBe(true);
        dialogs.closePanel(undefined);
        expect(true).toBe(true);
    });

    it("ignores when element does not have s-Panel class", async function () {
        var div = document.createElement("div");
        div.className = "test";
        var dialogs = (await import("./dialogs"));
        dialogs.closePanel(div);
        expect(div.classList.contains("test")).toBe(true);
        expect(div.classList.contains("hidden")).toBe(false);
    });

    it("ignores when element already has hidden class", async function () {
        var div = document.createElement("div");
        div.className = "s-Panel hidden test";
        var dialogs = (await import("./dialogs"));
        dialogs.closePanel(div);
        expect(div.classList.contains("test")).toBe(true);
        expect(div.classList.contains("hidden")).toBe(true);
    });

    it("can close panel via jQuery", async function () {
        var $ = (await import("@optionaldeps/jquery")).default;
        var div = $(`<div class="s-Panel"/>`).appendTo(document.body);
        var closingPanel: any;
        var closedPanel: any;
        var panelClosing = (e: any) => closingPanel = e.panel;
        var panelClosed = (e: any) => closedPanel = e.panel;
        $(window).on('panelclosing', panelClosing);
        $(window).on('panelclosed', panelClosed);
        try {
            var dialogs = (await import("./dialogs"));
            dialogs.closePanel(div);
            expect(div.hasClass("hidden")).toBe(true);
            expect(closingPanel).toBe(div[0]);
            expect(closedPanel).toBe(div[0]);
        }
        finally {
            $(window).off('panelclosing', panelClosing);
            $(window).off('panelclosed', panelClosed);
            div.remove();
        }
    });

    it("can close panel with Undefined jQuery", async function () {
        mockUndefinedJQuery();
        var div = document.body.appendChild(document.createElement("div"));
        var closingPanel: any;
        var closedPanel: any;
        var panelClosing = (e: any) => closingPanel = e.panel;
        var panelClosed = (e: any) => closedPanel = e.panel;
        window.addEventListener('panelclosing', panelClosing);
        window.addEventListener('panelclosed', panelClosed);
        try {
            div.classList.add("s-Panel");
            var dialogs = (await import("./dialogs"));
            dialogs.closePanel(div);
            expect(div.classList.contains("hidden")).toBe(true);
            expect(closingPanel).toBe(div);
            expect(closedPanel).toBe(div);
        }
        finally {
            window.removeEventListener('panelclosing', panelClosing);
            window.removeEventListener('panelclosed', panelClosed);
            div.remove();
        }
    });

    it("can be cancelled with preventDefault with Undefined jQuery", async function () {
        mockUndefinedJQuery();
        var div = document.body.appendChild(document.createElement("div"));
        try {
            div.classList.add("s-Panel");
            div.addEventListener("panelbeforeclose", e => {
                e.preventDefault();
            });
            var dialogs = (await import("./dialogs"));
            dialogs.closePanel(div);
            expect(div.classList.contains("panel-hidden")).toBe(false);
            expect(div.classList.contains("hidden")).toBe(false);
        }
        finally {
            div.remove();
        }
    });

    it("can be cancelled with preventDefault with jQuery", async function () {
        var $ = (await import("@optionaldeps/jquery")).default;
        var div = document.body.appendChild(document.createElement("div"));
        try {
            div.classList.add("s-Panel");
            $(div).on("panelbeforeclose", e => {
                e.preventDefault();
            });
            var dialogs = (await import("./dialogs"));
            dialogs.closePanel($(div));
            expect(div.classList.contains("panel-hidden")).toBe(false);
            expect(div.classList.contains("hidden")).toBe(false);
        }
        finally {
            div.remove();
        }
    });

    it("removes .panel-hidden from other panels that were hidden by this one with jQuery", async function () {
        var $ = (await import("@optionaldeps/jquery")).default;
        document.body.classList.add(".panels-container");
        var div1 = $(`<div class="s-Panel"/>`).appendTo(document.body);
        div1.addClass("panel-hidden").attr("data-panelhiddenby", "test"); 
        var div2 = $(`<div class="s-Panel"/>`).appendTo(document.body);
        div2.addClass("panel-hidden").attr("data-panelhiddenby", "test2");
        var div3 = $(`<div class="s-Panel"/>`).appendTo(document.body);
        div3.attr('data-paneluniquename', 'test');
        try {
            var dialogs = (await import("./dialogs"));
            dialogs.closePanel(div3);
            expect(div1.hasClass("panel-hidden")).toBe(false);
            expect(div2.hasClass("panel-hidden")).toBe(true);
            expect(div3.hasClass("hidden")).toBe(true);
            expect(div3.hasClass("panel-hidden")).toBe(false);
        }
        finally {
            div1.remove();
            div2.remove();
            div3.remove();
            document.body.classList.remove("panels-container");
        }
    
    });

    it("removes .panel-hidden from other panels that were hidden by this one with undefined jQuery", async function () {
        mockUndefinedJQuery();
        document.body.classList.add(".panels-container");
        var div1 = document.body.appendChild(document.createElement("div"));
        div1.classList.add("s-Panel");
        div1.classList.add("panel-hidden");
        div1.setAttribute("data-panelhiddenby", "test");

        var div2 = document.body.appendChild(document.createElement("div"));
        div2.classList.add("s-Panel");
        div2.classList.add("panel-hidden");
        div2.setAttribute("data-panelhiddenby", "test2");

        var div3 = document.body.appendChild(document.createElement("div"));
        div3.classList.add("s-Panel");
        div3.setAttribute("data-paneluniquename", "test");

        try {
            var dialogs = (await import("./dialogs"));
            dialogs.closePanel(div3);
            expect(div1.classList.contains("panel-hidden")).toBe(false);
            expect(div2.classList.contains("panel-hidden")).toBe(true);
            expect(div3.classList.contains("hidden")).toBe(true);
            expect(div3.classList.contains("panel-hidden")).toBe(false);
        }
        finally {
            div1.remove();
            div2.remove();
            div3.remove();
            document.body.classList.remove("panels-container");
        }
    });

    it("triggers layout event for elements with .require-layout class where offsetWidth > 0 without jQuery", async function () {
        mockUndefinedJQuery();
        document.body.classList.add(".panels-container");
        var div = document.body.appendChild(document.createElement("div"));
        Object.defineProperty(div, "offsetWidth", { value: 1 });
        div.classList.add("require-layout");
        var layoutCalls = 0;
        div.addEventListener("layout", e => layoutCalls++);

        var div2 = document.body.appendChild(document.createElement("div"));
        div2.classList.add("s-Panel");
        div2.setAttribute("data-paneluniquename", "test");

        try {
            var dialogs = (await import("./dialogs"));
            dialogs.closePanel(div2);
            expect(layoutCalls).toBe(1);
        }
        finally {
            div.remove();
        }
    });

    it("triggers layout event for elements with .require-layout class where offsetHeight > 0 without jQuery", async function () {
        mockUndefinedJQuery();
        document.body.classList.add(".panels-container");
        var div = document.body.appendChild(document.createElement("div"));
        Object.defineProperty(div, "offsetHeight", { value: 1 });
        div.classList.add("require-layout");
        var layoutCalls = 0;
        div.addEventListener("layout", e => layoutCalls++);

        var div2 = document.body.appendChild(document.createElement("div"));
        div2.classList.add("s-Panel");
        div2.setAttribute("data-paneluniquename", "test");

        try {
            var dialogs = (await import("./dialogs"));
            dialogs.closePanel(div2);
            expect(layoutCalls).toBe(1);
        }
        finally {
            div.remove();
        }
    });

});

describe("openPanel", () => {
    it("ignores when element is null or undefined", async function () {
        var dialogs = (await import("./dialogs"));
        dialogs.closePanel(null);
        expect(true).toBe(true);
        dialogs.closePanel(undefined);
        expect(true).toBe(true);
    });

    it("ignores when element already has hidden class", async function () {
        var div = document.createElement("div");
        div.className = "s-Panel hidden test";
        var dialogs = (await import("./dialogs"));
        dialogs.closePanel(div);
        expect(div.classList.contains("test")).toBe(true);
        expect(div.classList.contains("hidden")).toBe(true);
    });

    it("can open panel via jQuery", async function () {
        var $ = (await import("@optionaldeps/jquery")).default;

        document.body.classList.add("panels-container");
        var div1 = $(`<div class="s-Panel"/>`).appendTo(document.body);
        var div2 = $(`<div class="s-Panel"/>`);
        var openingPanel, openedPanel;
        const panelOpening = function(e: any) {
            openingPanel = e?.panel;
        }
        const panelOpened = function(e: any) {
            openedPanel = e?.panel;
        }
        $(window).on('panelopening', panelOpening);
        $(window).on('panelopened', panelOpened);

        try {
            var dialogs = (await import("./dialogs"));
            dialogs.openPanel(div2);
            expect(openingPanel).toBe(div2[0]);
            expect(openedPanel).toBe(div2[0]);
            expect(div1.hasClass("panel-hidden")).toBe(true);
            expect(div1.attr("data-panelhiddenby") != null).toBe(true);
            expect(div2.attr("data-paneluniquename")).toBe(div1.attr('data-panelhiddenby'));
            expect(div2.hasClass("hidden")).toBe(false);
            expect(div2.hasClass("panel-hidden")).toBe(false);
        }
        finally {
            $(window).off('panelopening', panelOpening);
            $(window).off('panelopened', panelOpened);
            div1.remove();
            div2.remove();
        }
    });

    it("can open panel without jQuery", async function () {
        mockUndefinedJQuery();
        document.body.classList.add("panels-container");
        var div1 = document.body.appendChild(document.createElement('div'));
        div1.className = "s-Panel";
        var div2 = document.createElement('div');
        div2.className = "s-Panel";
        var openingPanel, openedPanel;
        const panelOpening = function(e: any) {
            openingPanel = e?.panel;
        }
        const panelOpened = function(e: any) {
            openedPanel = e?.panel;
        }
        window.addEventListener('panelopening', panelOpening);
        window.addEventListener('panelopened', panelOpened);

        try {
            var dialogs = (await import("./dialogs"));
            dialogs.openPanel(div2);
            expect(openingPanel).toBe(div2);
            expect(openedPanel).toBe(div2);
            expect(div1.classList.contains("panel-hidden")).toBe(true);
            expect(div1.getAttribute("data-panelhiddenby") != null).toBe(true);
            expect(div2.getAttribute("data-paneluniquename")).toBe(div1.getAttribute('data-panelhiddenby'));
            expect(div2.classList.contains("hidden")).toBe(false);
            expect(div2.classList.contains("panel-hidden")).toBe(false);
        }
        finally {
            window.removeEventListener('panelopening', panelOpening);
            window.removeEventListener('panelopened', panelOpened);
            div1.remove();
            div2.remove();
        }
    });

});

export { }