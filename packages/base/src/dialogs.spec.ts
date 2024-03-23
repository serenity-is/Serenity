import { Dialog, alertDialog, confirmDialog, iframeDialog, informationDialog, successDialog, uiAndBSButtonNoConflict, warningDialog, type MessageDialogOptions } from "./dialogs";
import { Fluent } from "./fluent";

function cleanup() {
    document.body.innerHTML = "";
    delete (window as any)["$"]
    delete (window as any)["jQuery"]
    delete (window as any)["bootstrap"]
    jest.restoreAllMocks();
}

function mockUndefinedJQuery() {
    delete (window as any)["jQuery"]
    delete (window as any)["$"]
}

function mockUndefinedBS5Plus() {
    delete (window as any)["bootstrap"]
}

function mockEnvironmentWithBrowserDialogsOnly() {
    mockUndefinedBS5Plus();
    mockUndefinedJQuery();
}

function mockBS5Plus() {
    let modal = jest.fn(function (div: HTMLElement, opt: any) {
        return (div as any).modalInstance = {
            opt: opt,
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
        return (el as any).modalInstance;
    };

    return ((window as any)["bootstrap"] = {
        Modal: modal
    });
}

function mockBS5PlusWithUndefinedJQuery() {
    mockUndefinedJQuery();
    return mockBS5Plus();
}

function mockJQuery(fn: any) {
    let jQuery = function (selector: string | HTMLElement) {
        let result = {
            [0]: selector,
            length: 1,
            _selector: selector,
            _selectorHtml: typeof selector === 'string' ? selector : selector.outerHTML,
            html: function () {
                return <string>null;
            },
            eq: function () {
                return this;
            },
            get: function (i: number) {
                return i === 0 ? this._selector : null
            },
            find: function () {
                return this;
            },
            click: function () {
            },
            addClass: function (cls: string) {
                this._selector?.classList?.add?.(cls);
                return this;
            },
            appendTo: function () {
                return this;
            },
            attr: function (k: string, v: string) {
                if (typeof v === "undefined")
                    return this._selector?.getAttribute?.(k);
                this._selector?.setAttribute?.(k, v);
                return this;
            },
            off: function (ev: string, handler: () => void) {
                this._selector?.removeEventListener?.(ev, handler);
                return this;
            },
            on: function (ev: string, handler: () => void) {
                this._selector?.addEventListener?.(ev, handler);
                return this;
            },
            trigger: function (ev: Event) {
                this._selector?.dispatchEvent?.(typeof ev === "string" ? new Event(ev) : ev);
                return this;
            },
            triggerHandler: function (ev: any) {
                this._selector?.dispatchEvent?.(typeof ev === "string" ? new Event(ev) : ev);
                return this;
            },
            hasClass: function (cls: string) {
                return !!this._selector?.classList?.contains(cls);
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
            },
            jquery: "3.0.0"
        } as any;
        if (fn != null) {
            for (let x of Object.keys(fn)) {
                result[x] = fn[x];
            }
        }
        return result;
    } as any;

    jQuery.fn = fn;
    jQuery.Event = (name: string, prm: any) => {
        var ev: any = new Event(name);
        if (prm) {
            for (var i in prm) {
                if (i != "type" && i != "target") {
                    ev[i] = prm[i];
                }
            }
        }
        ev.isDefaultPrevented = () => false;
        ev.preventDefault = () => { ev.isDefaultPrevented = () => true; }
        return ev;
    }
    (window as any)["jQuery"] = jQuery;
    return jQuery;
}

function mockJQueryWithBootstrapModal(): any {
    let modal = jest.fn(function () {
        return this;
    }) as any;

    modal.Constructor = {
        VERSION: null
    };

    return mockJQuery({
        modal
    });
}

function mockJQueryWithUIDialog(): any {
    let dialog = jest.fn(function (op: any) {
        if (typeof op === "object" && this[0] instanceof HTMLElement && !this[0].parentElement) {
            this[0].classList.add("ui-dialog-content");
            var dlg = document.createElement("div");
            dlg.classList.add("ui-dialog");
            dlg.appendChild(this[0]);
        }
        return this;
    }) as any;

    let jQuery = mockJQuery({
        dialog
    });

    jQuery.ui = {
        dialog: dialog
    }

    return jQuery;
}

describe("Bootstrap version detection", () => {

    afterEach(cleanup);

    it('detects BS3 when modal version starts with 3', async function () {
        let jQuery = mockJQueryWithBootstrapModal();
        jQuery.fn.modal.Constructor.VERSION = "3.3.1";
        alertDialog("hello");
        expect(jQuery.fn.modal).toHaveBeenCalledTimes(2);
        expect(jQuery.fn.modal).toHaveBeenNthCalledWith(1, { backdrop: false, keyboard: true });
        expect(jQuery.fn.modal).toHaveBeenNthCalledWith(2, 'show');
        let html = jQuery.fn.modal.mock?.contexts[0]?._selectorHtml;
        expect(html).toBeDefined();
        let idx1 = html.indexOf('class="close"');
        let idx2 = html.indexOf('<h5');
        expect(idx1).toBeGreaterThan(-1);
        expect(idx2).toBeGreaterThan(idx1);
    });


    it('detects BS4 when modal version does not exist', async function () {
        let $ = mockJQueryWithBootstrapModal();
        delete $.fn.modal.Constructor;
        let opt = {
            onOpen: jest.fn(),
            onClose: jest.fn()
        };
        alertDialog("hello", opt);
        expect($.fn.modal).toHaveBeenCalledTimes(2);
        expect($.fn.modal).toHaveBeenNthCalledWith(1, { backdrop: false, keyboard: true });
        expect($.fn.modal).toHaveBeenNthCalledWith(2, 'show');
        let instance = $.fn.modal.mock?.contexts[0];
        let div = instance._selector;
        expect(div).toBeDefined();
        expect(div instanceof HTMLDivElement).toBe(true);
        let html = instance?._selectorHtml;
        let idx1 = html.indexOf('class="close"');
        let idx2 = html.indexOf('<h5');
        expect(idx1).toBeGreaterThan(-1);
        expect(idx2).toBeGreaterThan(-1);
        expect(idx1).toBeGreaterThan(idx2);
        const shownEvent = new Event("shown.bs.modal");
        div.dispatchEvent(shownEvent);
        expect(opt.onOpen).toHaveBeenCalledTimes(1);
        expect(opt.onClose).not.toHaveBeenCalled();
        expect(opt.onOpen.mock?.contexts?.[0]).toBeDefined();
        const hiddenEvent = new Event("hidden.bs.modal");
        div.dispatchEvent(hiddenEvent);
        expect(opt.onClose).toHaveBeenCalledTimes(1);
    });

    it('detects BS4 when modal version is something other than 3', async function () {

        let $ = mockJQueryWithBootstrapModal();
        $.fn.modal.Constructor.VERSION = '4.1.0';
        alertDialog("hello");
        expect($.fn.modal).toHaveBeenCalledTimes(2);
        expect($.fn.modal).toHaveBeenNthCalledWith(1, { backdrop: false, keyboard: true });
        expect($.fn.modal).toHaveBeenNthCalledWith(2, 'show');
        let html = $.fn.modal.mock?.contexts[0]?._selectorHtml;
        expect(html).toBeDefined();
        let idx1 = html.indexOf('class="close"');
        expect(idx1).toBeGreaterThan(-1);
        let idx2 = html.indexOf('<h5');
        expect(idx2).toBeGreaterThan(-1);
        expect(idx1).toBeGreaterThan(idx2);
    });
});

describe("Bootstrap noConflict", function () {
    afterEach(cleanup);

    function setupDummyJQuery() {
        return (window as any)["jQuery"] = Object.assign(function () {
            return {
                html: function (): any {
                    return null;
                },
                on: function (): any {
                    return this;
                }
            }
        },
            {
                fn: {}
            }) as any
    }

    function uiButton() {
    }

    function bsButton() {
    }

    it('does not call noConflict if no jQuery ui button widget', async function () {
        let $ = setupDummyJQuery();
        let noConflictCalled = false;
        $.ui = {};
        $.fn.button = {
            noConflict: function () {
                noConflictCalled = true;
            }
        }
        uiAndBSButtonNoConflict();
        expect(noConflictCalled).toBe(false);
    });

    it('skips noConflict if button does not have noConflict method', async function () {
        let $ = setupDummyJQuery();
        $.ui = {
            button: function () {
            }
        };
        $.fn.button = {
        }
        uiAndBSButtonNoConflict();
    });

    it('noConflict called if jQuery ui button widget exists and $.fn.button has noConflict method', async function () {
        let $ = setupDummyJQuery();
        let noConflictCalled = false;
        (bsButton as any).noConflict = function () {
            noConflictCalled = true;
            ($ as any).fn.button = uiButton;
            return bsButton;
        };

        $.fn.button = bsButton;
        $.ui = {
            button: uiButton
        };
        uiAndBSButtonNoConflict();
        expect(noConflictCalled).toBe(true);
        expect(($.fn as any).button).toBe(uiButton);
        expect(($.fn as any).btn).toBe(bsButton);
    });

    it('ignores when no $.fn', async function () {
        let $ = setupDummyJQuery();
        let noConflictCalled = false;
        delete $.fn;
        $.ui = {
            button: uiButton
        };
        uiAndBSButtonNoConflict();
        expect(noConflictCalled).toBe(false);
    });

    it('ignores when no $.ui', async function () {
        let $ = setupDummyJQuery();
        let noConflictCalled = false;
        delete $.ui;
        uiAndBSButtonNoConflict();
        expect(noConflictCalled).toBe(false);
    });

    it('ignores when no $.ui.button', async function () {
        let $ = setupDummyJQuery();
        let noConflictCalled = false;
        $.ui = {};
        uiAndBSButtonNoConflict();
        expect(noConflictCalled).toBe(false);
    });

    it('ignores when no $.fn.button.noConflict', async function () {
        let $ = setupDummyJQuery();
        let noConflictCalled = false;
        $.fn.button = bsButton;
        $.ui = {
            button: uiButton
        };
        uiAndBSButtonNoConflict();
        expect(noConflictCalled).toBe(false);
    });
});


describe("alertDialog", () => {
    afterEach(cleanup);

    it('alertDialog uses window.alert when no BS/jQuery UI loaded', async function () {
        let alertSpy = jest.spyOn(window, "alert");
        try {
            mockEnvironmentWithBrowserDialogsOnly();
            alertSpy.mockImplementation(() => { });
            alertDialog('test message');
            expect(alertSpy).toHaveBeenCalledTimes(1);
            expect(alertSpy).toHaveBeenCalledWith("test message");
        }
        finally {
            alertSpy.mockRestore();
        }
    });

    it('calls jQuery ui dialog with expected parameters', async function () {
        let $ = mockJQueryWithUIDialog();
        let opt = {
            onOpen: jest.fn(),
            onClose: jest.fn()
        };
        var dialog = alertDialog("hello", opt);
        expect($.fn.dialog).toHaveBeenCalledTimes(2);
        expect($.fn.dialog).toHaveBeenNthCalledWith(2, 'open');
        let x = $.fn.dialog.mock?.calls?.[0]?.[0] as any & MessageDialogOptions;
        expect(x).toBeDefined();
        expect(x.title).toBe("Alert");
        expect(x.modal).toBe(true);
        expect(x.buttons.length).toEqual(1);
        expect(x.buttons[0].text).toEqual("OK");
        expect(typeof x.buttons[0].click).toBe("function");
        expect(x.dialogClass).toBe("s-MessageDialog s-AlertDialog");
        expect(x.width).toBe("40%");
        expect(x.maxWidth).toBe(450);
        expect(x.minWidth).toBe(180);
        expect(x.modal).toBe(true);
        expect(x.resizable).toBe(false);
        expect(dialog.type).toBe("uidialog");
        opt.onOpen();
        expect(opt.onOpen).toHaveBeenCalledTimes(1);
        expect(opt.onOpen.mock?.contexts?.[0]).toBeDefined();
        expect(opt.onClose).not.toHaveBeenCalled();
        x.buttons[0].click();
        expect(opt.onOpen).toHaveBeenCalledTimes(1);
        expect(dialog.result).toBe("ok");
    });

    it('returns expected bootstrap.Modal markup', async function () {
        let bootstrap = mockBS5PlusWithUndefinedJQuery();
        let opt = {
            onOpen: jest.fn(),
            onClose: jest.fn()
        };
        var dialog = alertDialog("hello", opt);
        let modal = document.querySelector<HTMLElement>(".modal");
        try {
            expect(modal).not.toBeNull();
            expect(modal.classList).toContain("modal");
            expect(modal.classList).toContain("s-MessageDialog");
            expect(modal.classList).toContain("s-AlertDialog");
            expect(modal.getAttribute("tabIndex")).toBe("-1");
            let modalDialog = modal.querySelector(".modal-dialog");
            expect(modalDialog).not.toBeNull();
            expect(modalDialog.parentElement).toBe(modal);
            let modalContent = modalDialog.querySelector(".modal-content");
            expect(modalContent).not.toBeNull();
            expect(modalContent.parentElement).toBe(modalDialog);
            let modalHeader = modalContent.querySelector(".modal-header");
            expect(modalHeader).not.toBeNull();
            expect(modalHeader.parentElement).toBe(modalContent);
            let modalTitle = modalContent.querySelector(".modal-title");
            expect(modalTitle.tagName).toBe("H5");
            let button = modalContent.querySelector(".modal-footer button");
            expect(button).not.toBeNull();
            expect(button.classList).toContain("btn");
            expect(button.classList).toContain("btn-danger");
            expect(button.textContent).toBe("OK");

            expect(bootstrap.Modal).toHaveBeenCalledTimes(1);
            let div = bootstrap.Modal.mock.calls?.[0]?.[0] as HTMLDivElement;
            expect(div).toBeDefined();
            expect(div?.dataset?.showCalls).toBe("1");
            const shownEvent = new Event("shown.bs.modal");
            div.dispatchEvent(shownEvent);
            expect(opt.onOpen).toHaveBeenCalledTimes(1);
            expect(opt.onClose).not.toHaveBeenCalled();
            expect(opt.onOpen.mock?.contexts?.[0]).toBeDefined();
            expect(dialog.type).toBe("bsmodal");
            const hiddenEvent = new Event("hidden.bs.modal");
            div.dispatchEvent(hiddenEvent);
            expect(opt.onClose).toHaveBeenCalledTimes(1);
            const clickEvent = new Event("click");
            button.dispatchEvent(clickEvent);
            expect(Dialog.getInstance(modal)?.getDialogNode()).toBe(modal);
            expect(dialog.result).toBe("ok");
        }
        finally {
            modal?.remove();
        }
    });

    it('calls localText for title', async function () {
        let $ = mockJQueryWithUIDialog();
        let opt = {
            onOpen: jest.fn(),
            onClose: jest.fn()
        };
        alertDialog("hello", opt);
        expect($.fn.dialog).toHaveBeenCalledTimes(2);
        expect($.fn.dialog).toHaveBeenNthCalledWith(2, 'open');
        let x = $.fn.dialog.mock?.calls?.[0]?.[0] as any & MessageDialogOptions;
        expect(x).toBeDefined();
        expect(x.title).toBe("Alert");
    });
});

describe("informationDialog", () => {
    afterEach(cleanup);

    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        mockEnvironmentWithBrowserDialogsOnly();
        let alertSpy = jest.spyOn(window, "alert");
        try {
            alertSpy.mockImplementation(() => { });
            informationDialog('test message', () => { });
            expect(alertSpy).toHaveBeenCalledTimes(1);
            expect(alertSpy).toHaveBeenCalledWith("test message");
        }
        finally {
            alertSpy.mockRestore();
        }
    });

    it('calls jQuery ui dialog with expected parameters', async function () {
        let $ = mockJQueryWithUIDialog();
        let opt = {
            onOpen: jest.fn(),
            onClose: jest.fn()
        };
        let onOK = jest.fn(function () {
        });
        var dialog = informationDialog("hello", onOK, opt);
        expect($.fn.dialog).toHaveBeenCalledTimes(2);
        expect($.fn.dialog).toHaveBeenNthCalledWith(2, 'open');
        let x = $.fn.dialog.mock?.calls?.[0]?.[0] as any & MessageDialogOptions;
        expect(x).toBeDefined();
        expect(x.title).toBe("Information");
        expect(x.modal).toBe(true);

        expect(x.buttons.length).toEqual(1);
        expect(x.buttons[0].text).toEqual("OK");
        expect(typeof x.buttons[0].click).toBe("function");

        expect(x.dialogClass).toBe("s-MessageDialog s-InformationDialog");
        expect(x.width).toBe("40%");
        expect(x.maxWidth).toBe(450);
        expect(x.minWidth).toBe(180);
        expect(x.modal).toBe(true);
        expect(x.resizable).toBe(false);
        opt.onOpen();
        expect(opt.onOpen).toHaveBeenCalledTimes(1);
        expect(opt.onClose).not.toHaveBeenCalled();
        expect(opt.onOpen.mock?.contexts?.[0]).toBeDefined();
        opt.onClose();
        expect(opt.onClose).toHaveBeenCalledTimes(1);
        x.buttons[0].click();
        expect(onOK).toHaveBeenCalledTimes(1);
        expect(dialog.type).toBe("uidialog");
        expect(dialog.result).toBe("ok");
    });
});

describe("warningDialog", () => {

    afterEach(cleanup);

    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        mockEnvironmentWithBrowserDialogsOnly();
        let alertSpy = jest.spyOn(window, "alert");
        try {
            alertSpy.mockImplementation(() => { });
            warningDialog('test message');
            expect(alertSpy).toHaveBeenCalledTimes(1);
            expect(alertSpy).toHaveBeenCalledWith("test message");
        }
        finally {
            alertSpy.mockRestore();
        }
    });
});

describe("confirmDialog", () => {
    afterEach(cleanup);

    it('uses window.confirm when no BS/jQuery UI loaded', async function () {
        mockEnvironmentWithBrowserDialogsOnly();
        let confirmSpy = jest.spyOn(window, "confirm");
        try {
            confirmSpy.mockImplementation(() => true);
            let onYesCalled;
            confirmDialog('test message', function () {
                onYesCalled = true;
            });
            expect(confirmSpy).toHaveBeenCalledTimes(1);
            expect(confirmSpy).toHaveBeenCalledWith("test message");
            expect(onYesCalled).toBe(true);
        }
        finally {
            confirmSpy.mockRestore();
        }
    });

    it('calls jQuery ui dialog with expected parameters', async function () {
        let $ = mockJQueryWithUIDialog();
        let opt = {
            onOpen: jest.fn(),
            onClose: jest.fn()
        };
        let onYes = jest.fn(function () {
        });
        confirmDialog("hello", onYes, opt);
        expect($.fn.dialog).toHaveBeenCalledTimes(2);
        expect($.fn.dialog).toHaveBeenNthCalledWith(2, 'open');
        let x = $.fn.dialog.mock?.calls?.[0]?.[0] as any & MessageDialogOptions;
        expect(x).toBeDefined();
        expect(x.title).toBe("Confirm");
        expect(x.modal).toBe(true);

        expect(x.buttons.length).toEqual(2);
        expect(x.buttons[0].text).toEqual("Yes");
        expect(typeof x.buttons[0].click).toBe("function");

        expect(x.buttons[1].text).toEqual("No");
        expect(typeof x.buttons[1].click).toBe("function");

        expect(x.dialogClass).toBe("s-MessageDialog s-ConfirmDialog");
        expect(x.width).toBe("40%");
        expect(x.maxWidth).toBe(450);
        expect(x.minWidth).toBe(180);
        expect(x.modal).toBe(true);
        expect(x.resizable).toBe(false);
        opt.onOpen();
        expect(opt.onOpen).toHaveBeenCalledTimes(1);
        expect(opt.onClose).not.toHaveBeenCalled();
        expect(opt.onOpen.mock?.contexts?.[0]).toBeDefined();
        opt.onClose();
        expect(opt.onClose).toHaveBeenCalledTimes(1);
        x.buttons[0].click();
        expect(onYes).toHaveBeenCalledTimes(1);
    });

    it('returns expected bootstrap.Modal markup with BS5+ and no JQuery', async function () {
        let bootstrap = mockBS5PlusWithUndefinedJQuery();
        let opt = {
            onOpen: jest.fn(),
            onClose: jest.fn(),
            cancelButton: true
        };
        let onYes = jest.fn(function () {
        });
        confirmDialog("hello", onYes, opt);
        let modal = document.querySelector<HTMLElement>(".modal");
        try {
            expect(modal).not.toBeNull();
            expect(modal.classList).toContain("modal");
            expect(modal.classList).toContain("s-MessageDialog");
            expect(modal.classList).toContain("s-ConfirmDialog");
            expect(modal.getAttribute("tabIndex")).toBe("-1");
            let modalDialog = modal.querySelector(".modal-dialog");
            expect(modalDialog).not.toBeNull();
            expect(modalDialog.parentElement).toBe(modal);
            let modalContent = modalDialog.querySelector(".modal-content");
            expect(modalContent).not.toBeNull();
            expect(modalContent.parentElement).toBe(modalDialog);
            let modalHeader = modalContent.querySelector(".modal-header");
            expect(modalHeader).not.toBeNull();
            expect(modalHeader.parentElement).toBe(modalContent);
            let modalTitle = modalContent.querySelector(".modal-title");
            expect(modalTitle.tagName).toBe("H5");

            let buttons = modalContent.querySelectorAll(".modal-footer button");
            expect(buttons.length).toBe(3);

            let yesButton = buttons[0];
            expect(yesButton.classList).toContain("btn");
            expect(yesButton.classList).toContain("btn-primary");
            expect(yesButton.textContent).toBe("Yes");

            let noButton = buttons[1];
            expect(noButton.classList).toContain("btn");
            expect(noButton.classList).toContain("btn-danger");
            expect(noButton.textContent).toBe("No");

            expect(bootstrap.Modal).toHaveBeenCalledTimes(1);
            let div = bootstrap.Modal.mock.calls?.[0]?.[0] as HTMLDivElement;
            expect(div).toBeDefined();
            expect(div?.dataset?.showCalls).toBe("1");
            Fluent.trigger(div, "shown.bs.modal");
            expect(opt.onOpen).toHaveBeenCalledTimes(1);
            expect(opt.onClose).not.toHaveBeenCalled();
            expect(opt.onOpen.mock?.contexts?.[0]).toBeDefined();
            Fluent.trigger(div, "hidden.bs.modal");
            expect(opt.onClose).toHaveBeenCalledTimes(1);
            expect(opt.onClose.mock.calls[0][0]).toBe(void 0);
            const clickEvent = new Event("click");
            yesButton.dispatchEvent(clickEvent);
            expect(Dialog.getInstance(modal)?.getDialogNode()).toBe(modal);
        }
        finally {
            modal?.remove();
        }
    });
});

describe("successDialog", () => {
    afterEach(cleanup);

    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        mockEnvironmentWithBrowserDialogsOnly();
        let alertSpy = jest.spyOn(window, "alert");
        try {
            alertSpy.mockImplementation(() => { });
            let onOKCalled;
            successDialog('test message', () => {
                onOKCalled = true;
            });
            expect(alertSpy).toHaveBeenCalledTimes(1);
            expect(alertSpy).toHaveBeenCalledWith("test message");
            expect(onOKCalled).toBe(true);
        }
        finally {
            alertSpy.mockRestore();
        }
    });

    it('calls jQuery ui dialog with expected parameters', async function () {
        let $ = mockJQueryWithUIDialog();
        let opt = {
            onOpen: jest.fn(),
            onClose: jest.fn()
        };
        let onOK = jest.fn(function () {
        });
        successDialog("hello", onOK, opt);
        expect($.fn.dialog).toHaveBeenCalledTimes(2);
        expect($.fn.dialog).toHaveBeenNthCalledWith(2, 'open');
        let x = $.fn.dialog.mock?.calls?.[0]?.[0] as any & MessageDialogOptions;
        expect(x).toBeDefined();
        expect(x.title).toBe("Success");
        expect(x.modal).toBe(true);

        expect(x.buttons.length).toEqual(1);
        expect(x.buttons[0].text).toEqual("OK");
        expect(typeof x.buttons[0].click).toBe("function");

        expect(x.dialogClass).toBe("s-MessageDialog s-SuccessDialog");
        expect(x.width).toBe("40%");
        expect(x.maxWidth).toBe(450);
        expect(x.minWidth).toBe(180);
        expect(x.modal).toBe(true);
        expect(x.resizable).toBe(false);
        opt.onOpen();
        expect(opt.onOpen).toHaveBeenCalledTimes(1);
        expect(opt.onClose).not.toHaveBeenCalled();
        expect(opt.onOpen.mock?.contexts?.[0]).toBeDefined();
        opt.onClose();
        expect(opt.onClose).toHaveBeenCalledTimes(1);
        expect(opt.onClose.mock?.contexts?.[0]).toBeDefined();
        x.buttons[0].click();
        expect(onOK).toHaveBeenCalledTimes(1);
    });
});

describe("iframeDialog", () => {
    afterEach(cleanup);

    it('uses window.alert when no BS/jQuery UI loaded', async function () {
        mockEnvironmentWithBrowserDialogsOnly();
        var alertSpy = jest.spyOn(window, "alert");
        try {
            alertSpy.mockImplementation(() => { });
            var testHtml = '<html><body>test message<body></html>';
            iframeDialog({
                html: testHtml
            });
            expect(alertSpy).toHaveBeenCalledTimes(1);
            expect(alertSpy).toHaveBeenCalledWith(testHtml);
        }
        finally {
            alertSpy.mockRestore();
        }
    });

    it('calls jQuery ui dialog with expected parameters', async function () {
        let $ = mockJQueryWithUIDialog();
        const dialogs = await import("./dialogs");
        var opt = {
            html: "<span>test</span>"
        };
        dialogs.iframeDialog(opt);
        expect($.fn.dialog).toHaveBeenCalledTimes(2);
        expect($.fn.dialog).toHaveBeenNthCalledWith(2, 'open');

        var x = $.fn.dialog.mock?.calls?.[0]?.[0] as any & MessageDialogOptions;
        expect(x).toBeDefined();
        expect(x.title).toBe("Alert");
        expect(x.modal).toBe(true);

        expect(x.buttons).toBeUndefined();

        expect(x.dialogClass).toBe("s-IFrameDialog");
        expect(x.width).toBe("60%");
        expect(x.height).toBe("400");
        expect(x.modal).toBe(true);
        expect(x.resizable).toBe(false);
    });

    it('returns expected bootstrap.Modal markup', async function () {
        let bootstrap = mockBS5PlusWithUndefinedJQuery();
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
            var modalDialog = modal.querySelector(".modal-dialog");
            expect(modalDialog).not.toBeNull();
            expect(modalDialog.parentElement).toBe(modal);
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
    afterEach(cleanup);

    it("auto prefixes icons with 'fa-' prefix with 'fa' with BS5+ and no JQuery", async function () {
        mockBS5PlusWithUndefinedJQuery();
        alertDialog("test", {
            buttons: [{
                result: "ok",
                text: "ok",
                icon: "fa-test"
            }]
        });
        try {
            let i = document.querySelector(".modal-footer button i");
            expect(i != null).toBe(true);
            expect(i.classList.contains("fa-test")).toBe(true);
            expect(i.classList.contains("fa")).toBe(true);
        }
        finally {
            document.querySelector(".modal")?.remove();
        }
    });
});

//describe("dialogButtonToBS", () => {
//    afterEach(cleanup);
//    it("converts dialog button to BS5+ button", async function () {
//        mockBS5PlusWithUndefinedJQuery();
//        const dialogs = await import("./dialogs");
//        let button = dialogs.Dialog(document.createElement("div")).dialogButtonToBS({
//            result: "ok",
//            cssClass: "btn-success",
//            text: "ok",
//            icon: "fa-test"
//        });
//        expect(button != null).toBe(true);
//        expect(button.className).toBe("btn btn-success");
//        let i = button.querySelector("i");
//        expect(i.classList.contains("fa-test")).toBe(true);
//        expect(i.classList.contains("fa")).toBe(true);
//    });
//
//    it("can work without icons", async function () {
//        mockBS5PlusWithUndefinedJQuery();
//        const dialogs = await import("./dialogs");
//        let button = dialogs.dialogButtonToBS({
//            result: "ok",
//            cssClass: "btn-success",
//            text: "ok"
//        });
//        expect(button != null).toBe(true);
//        expect(button.className).toBe("btn btn-success");
//        let i = button.querySelector("i");
//        expect(i).toBeNull();
//    });
//
//    it("returns other icon classes as is", async function () {
//        mockBS5PlusWithUndefinedJQuery();
//        const dialogs = await import("./dialogs");
//        let button = dialogs.dialogButtonToBS({
//            result: "ok",
//            icon: 'xy-some-icon',
//            text: "ok"
//        });
//        expect(button != null).toBe(true);
//        let i = button.querySelector("i");
//        expect(i.className).toBe("xy-some-icon");
//    });
//
//    it("html encodes by default", async function () {
//        const dialogs = (await import("./dialogs"));
//        let button = dialogs.dialogButtonToBS({ text: "<div>x</div>" });
//        expect(button.textContent).toBe("<div>x</div>");
//    });
//
//    it("html encodes when htmlEncode: undefined", async function () {
//        const dialogs = (await import("./dialogs"));
//        let button1 = dialogs.dialogButtonToBS({ text: "<div>x</div>", htmlEncode: undefined });
//        expect(button1.textContent).toBe("<div>x</div>");
//        let button2 = dialogs.dialogButtonToBS({ text: "<div>x</div>", htmlEncode: undefined });
//        expect(button2.textContent).toBe("<div>x</div>");
//    });
//
//    it("can skip htmlencode with htmlEncode: false", async function() {
//        const dialogs = (await import("./dialogs"));
//        let button = dialogs.dialogButtonToBS({ text: "<div>x</div>", htmlEncode: false });
//        expect(button.innerHTML).toBe("<div>x</div>");
//    });
//
//    it("can set button title to hint", async function() {
//        const dialogs = (await import("./dialogs"));
//        let button = dialogs.dialogButtonToBS({ text: "x", hint: "test" });
//        expect(button.getAttribute("title")).toBe("test");
//    });
//});
//
//describe("dialogButtonToUI", () => {
//    afterEach(cleanup);
//    it("html encodes by default", async function () {
//        const dialogs = (await import("./dialogs"));
//        let button = dialogs.dialogButtonToUI({ text: "<div>x</div>" });
//        expect(button.text).toBe("&lt;div&gt;x&lt;/div&gt;");
//    });
//
//    it("does not html encode if htmlEncode is false", async function () {
//        const dialogs = (await import("./dialogs"));
//        let button = dialogs.dialogButtonToUI({ text: "<div>x</div>", htmlEncode: false });
//        expect(button.text).toBe("<div>x</div>");
//    });
//
//    it("sets css class", async function () {
//        const dialogs = (await import("./dialogs"));
//        let button = dialogs.dialogButtonToUI({ text: "<div>x</div>", cssClass: "x" });
//        expect(button.cssClass).toBe("x");
//    });
//
//    it("adds the icon", async function () {
//        const dialogs = (await import("./dialogs"));
//        let button = dialogs.dialogButtonToUI({ text: "test",  icon: "x" });
//        expect(button.text).toBe('<i class="x"></i> test');
//    });
//});

describe("Dialog.close", () => {
    afterEach(cleanup);

    it("ignores when element is null or undefined", async function () {
        new Dialog(null).close();
        expect(true).toBe(true);
        new Dialog(undefined).close();
        expect(true).toBe(true);
    });

    it("ignores when element does not have s-Panel class", async function () {
        let div = document.createElement("div");
        div.className = "test";
        Dialog.getInstance(div)?.close();
        expect(div.classList.contains("test")).toBe(true);
        expect(div.classList.contains("hidden")).toBe(false);
    });

    it("ignores when element already has hidden class", async function () {
        let div = document.createElement("div");
        div.className = "s-Panel hidden test";
        Dialog.getInstance(div)?.close();
        expect(div.classList.contains("test")).toBe(true);
        expect(div.classList.contains("hidden")).toBe(true);
    });

    //it("can close panel via jQuery", async function () {
    //    let jQuery = mockJQuery({});
    //    var divEl = document.body.appendChild(document.createElement('div'));
    //    divEl.classList.add("s-Panel");
    //    let div = jQuery(divEl);
    //    let closingPanel: any;
    //    let closedPanel: any;
    //    let panelBeforeClose = (e: any) => closingPanel = e.target;
    //    let panelClose = (e: any) => closedPanel = e.target;
    //    Fluent.on(window, 'panelbeforeclose', panelBeforeClose);
    //    Fluent.on(window, 'panelclose', panelClose);
    //    try {
    //        let dialogs = (await import("./dialogs"));
    //        dialogs.Dialog.getInstance(div).close();
    //        expect(div.hasClass("hidden")).toBe(true);
    //        expect(closingPanel).toBe(divEl);
    //        expect(closedPanel).toBe(divEl);
    //    }
    //    finally {
    //        Fluent.off(window, 'panelbeforeclose', panelBeforeClose);
    //        Fluent.off(window, 'panelclose', panelClose);
    //        div.remove();
    //    }
    //});

    // it("can close panel with Undefined jQuery", async function () {
    //     mockUndefinedJQuery();
    //     let div = document.body.appendChild(document.createElement("div"));
    //     let closingPanel: any;
    //     let closedPanel: any;
    //     let panelClosing = (e: any) => closingPanel = e.target;
    //     let panelClose = (e: any) => closedPanel = e.target;
    //     Fluent.on(window, 'panelbeforeclose', panelClosing);
    //     Fluent.on(window, 'panelclose', panelClose);
    //     try {
    //         div.classList.add("s-Panel");
    //         let dialogs = (await import("./dialogs"));
    //         dialogs.Dialog.getInstance(div).close();
    //         expect(div.classList.contains("hidden")).toBe(true);
    //         expect(closingPanel).toBe(div);
    //         expect(closedPanel).toBe(div);
    //     }
    //     finally {
    //         Fluent.off(window, 'panelbeforeclose', panelClosing);
    //         Fluent.off(window, 'panelclose', panelClose);
    //         div.remove();
    //     }
    // });

    // it("can be cancelled with preventDefault with Undefined jQuery", async function () {
    //     mockUndefinedJQuery();
    //     let div = document.body.appendChild(document.createElement("div"));
    //     try {
    //         div.classList.add("s-Panel");
    //         div.addEventListener("panelbeforeclose", e => {
    //             e.preventDefault();
    //         });
    //         let dialogs = (await import("./dialogs"));
    //         dialogs.Dialog.getInstance(div).close();
    //         expect(div.dataset.hiddenby).toBeFalsy();
    //         expect(div.classList.contains("hidden")).toBe(false);
    //     }
    //     finally {
    //         div.remove();
    //     }
    // });

    //it("can be cancelled with preventDefault with jQuery", async function () {
    //    let jQuery = mockJQuery({});
    //    let div = document.body.appendChild(document.createElement("div"));
    //    try {
    //        div.classList.add("s-Panel");
    //        jQuery(div).on("panelbeforeclose", (e: Event) => {
    //            e.preventDefault();
    //        });
    //        let dialogs = (await import("./dialogs"));
    //        dialogs.Dialog.getInstance(jQuery(div)).close();
    //        expect(div.dataset.hiddenby).toBeFalsy();
    //        expect(div.classList.contains("hidden")).toBe(false);
    //    }
    //    finally {
    //        div.remove();
    //    }
    //});
    //
    //it("removes [data-hiddenby] from other panels that were hidden by this one with jQuery", async function () {
    //    let jQuery = mockJQuery({});
    //    document.body.classList.add(".panels-container");
    //    let div1El = document.body.appendChild(document.createElement("div"));
    //    div1El.classList.add("s-Panel");
    //    div1El.setAttribute("data-hiddenby", "test"); 
    //    let div1 = jQuery(div1El);
    //    let div2El = document.body.appendChild(document.createElement("div"));
    //    div2El.classList.add("s-Panel");
    //    div2El.setAttribute("data-hiddenby", "test2");
    //    let div2 = jQuery(div2El);
    //    let div3El = document.body.appendChild(document.createElement("div"));
    //    div3El.classList.add("s-Panel");
    //    div3El.setAttribute('data-paneluniquename', 'test');
    //    let div3 = jQuery(div3El);
    //    try {
    //        let dialogs = (await import("./dialogs"));
    //        dialogs.Dialog.getInstance(div3).close();
    //        expect(div1.attr("data-hiddenby")).toBeFalsy();
    //        expect(div2.attr("data-hiddenby")).toBeTruthy();
    //        expect(div3.hasClass("hidden")).toBe(true);
    //        expect(div3.attr("data-hiddenby")).toBeFalsy();
    //    }
    //    finally {
    //        div1.remove();
    //        div2.remove();
    //        div3.remove();
    //        document.body.classList.remove("panels-container");
    //    }
    //
    //});
    //
    //it("removes [data-hiddenby] from other panels that were hidden by this one with undefined jQuery", async function () {
    //    mockUndefinedJQuery();
    //    document.body.classList.add(".panels-container");
    //    let div1 = document.body.appendChild(document.createElement("div"));
    //    div1.classList.add("s-Panel");
    //    div1.setAttribute("data-hiddenby", "test");
    //
    //    let div2 = document.body.appendChild(document.createElement("div"));
    //    div2.classList.add("s-Panel");
    //    div2.setAttribute("data-hiddenby", "test2");
    //
    //    let div3 = document.body.appendChild(document.createElement("div"));
    //    div3.classList.add("s-Panel");
    //    div3.setAttribute("data-paneluniquename", "test");
    //
    //    try {
    //        let dialogs = (await import("./dialogs"));
    //        dialogs.Dialog.getInstance(div3).close();
    //        expect(div1.dataset.hiddenby).toBeFalsy();
    //        expect(div2.dataset.hiddenby).toBeTruthy();
    //        expect(div3.classList.contains("hidden")).toBe(true);
    //        expect(div3.dataset.hiddenby).toBeFalsy();
    //    }
    //    finally {
    //        div1.remove();
    //        div2.remove();
    //        div3.remove();
    //        document.body.classList.remove("panels-container");
    //    }
    //});
    //
    //it("triggers layout event for elements with .require-layout class where offsetWidth > 0 without jQuery", async function () {
    //    mockUndefinedJQuery();
    //    document.body.classList.add(".panels-container");
    //    let div = document.body.appendChild(document.createElement("div"));
    //    Object.defineProperty(div, "offsetWidth", { value: 1 });
    //    div.classList.add("require-layout");
    //    let layoutCalls = 0;
    //    div.addEventListener("layout", e => layoutCalls++);
    //
    //    let div2 = document.body.appendChild(document.createElement("div"));
    //    div2.classList.add("s-Panel");
    //    div2.setAttribute("data-paneluniquename", "test");
    //
    //    try {
    //        let dialogs = (await import("./dialogs"));
    //        dialogs.Dialog.getInstance(div2).close();
    //        expect(layoutCalls).toBe(1);
    //    }
    //    finally {
    //        div.remove();
    //    }
    //});

    //it("triggers layout event for elements with .require-layout class where offsetHeight > 0 without jQuery", async function () {
    //    mockUndefinedJQuery();
    //    document.body.classList.add(".panels-container");
    //    let div = document.body.appendChild(document.createElement("div"));
    //    Object.defineProperty(div, "offsetHeight", { value: 1 });
    //    div.classList.add("require-layout");
    //    let layoutCalls = 0;
    //    div.addEventListener("layout", e => layoutCalls++);
    //
    //    let div2 = document.body.appendChild(document.createElement("div"));
    //    div2.classList.add("s-Panel");
    //    div2.setAttribute("data-paneluniquename", "test");
    //
    //    try {
    //        let dialogs = (await import("./dialogs"));
    //        dialogs.Dialog.getInstance(div2).close();
    //        expect(layoutCalls).toBe(1);
    //    }
    //    finally {
    //        div.remove();
    //    }
    //});

});

describe("Dialog panels", () => {
    afterEach(cleanup);

    it("can open panel via jQuery", async function () {
        let jQuery = mockJQuery({});

        document.body.classList.add("panels-container");
        var body1 = Fluent("div").addClass("panel-body 1");
        var panel1 = Fluent("div").class("s-Panel").appendTo(document.body).append(body1);
        var body2 = Fluent("div").addClass("panel-body 2");
        var panel2 = Fluent("div").class("s-Panel").appendTo(document.body).append(body2);

        let openingPanel, openedPanel;
        const panelBeforeOpen = function (e: any) {
            openingPanel = e?.target;
        }
        const panelOpen = function (e: any) {
            openedPanel = e?.target;
        }
        jQuery(window).on('panelbeforeopen', panelBeforeOpen);
        jQuery(window).on('panelopen', panelOpen);

        try {
            Dialog.getInstance(panel2).open();
            expect(openingPanel).toBe(body2.getNode());
            expect(openedPanel).toStrictEqual(body2.getNode());
            expect(panel1.attr("data-hiddenby")).toBeTruthy();
            expect(panel2.attr("data-paneluniquename")).toBe(panel1.attr('data-hiddenby'));
            expect(panel2.hasClass("hidden")).toBe(false);
            expect(panel2.attr("data-hiddenby")).toBeFalsy();
        }
        finally {
            jQuery(window).off('panelbeforeopen', panelBeforeOpen);
            jQuery(window).off('panelopen', panelOpen);
            panel1.remove();
            panel2.remove();
        }
    });

    it("can open panel without jQuery", async function () {
        mockUndefinedJQuery();
        document.body.classList.add("panels-container");
        var body1 = Fluent("div").addClass("panel-body 1");
        var panel1 = Fluent("div").class("s-Panel").appendTo(document.body).append(body1);
        var body2 = Fluent("div").addClass("panel-body 2");
        var panel2 = Fluent("div").class("s-Panel").appendTo(document.body).append(body2);

        let openingPanel, openedPanel;
        const panelOpening = function (e: any) {
            openingPanel = e?.target;
        }
        const panelOpened = function (e: any) {
            openedPanel = e?.target;
        }
        window.addEventListener('panelbeforeopen', panelOpening);
        window.addEventListener('panelopen', panelOpened);
        try {
            Dialog.getInstance(panel2).open();
            expect(openingPanel).toBe(body2.getNode());
            expect(openedPanel).toBe(body2.getNode());
            expect(panel1.data("hiddenby")).toBeTruthy();
            expect(panel2.attr("data-paneluniquename")).toBe(panel1.data("hiddenby"));
            expect(panel2.getNode().classList.contains("hidden")).toBe(false);
            expect(panel2.data("hiddenby")).toBeFalsy();
        }
        finally {
            window.removeEventListener('panelbeforeopen', panelOpening);
            window.removeEventListener('panelopen', panelOpened);
            panel1.remove();
            panel2.remove();
        }
    });

    it("creates panels when preferPane is true even if jquery ui and bootstrap are both available", () => {
        mockBS5Plus();
        mockJQueryWithUIDialog();
        var dlg = new Dialog({ preferPanel: true });
        dlg.open();
        expect(dlg.getDialogNode().classList.contains("s-Panel")).toBe(true);
        expect(dlg.getContentNode().classList.contains("panel-body")).toBe(true);
    });

    it("creates panels when preferPane is true even if jquery ui is available", () => {
        mockJQueryWithUIDialog();
        var dlg = new Dialog({ preferPanel: true });
        dlg.open();
        expect(dlg.getDialogNode().classList.contains("s-Panel")).toBe(true);
        expect(dlg.getContentNode().classList.contains("panel-body")).toBe(true);
    });

    
    it("creates panels when preferPane is true even if bootstrap is available", () => {
        mockBS5Plus();
        var dlg = new Dialog({ preferPanel: true });
        dlg.open();
        expect(dlg.getDialogNode().classList.contains("s-Panel")).toBe(true);
        expect(dlg.getContentNode().classList.contains("panel-body")).toBe(true);
    });
});

describe("modal event propagation to modal-body", () => {

    afterEach(cleanup);

    it("installs an event propagation handler for show.bs.modal to modal body as modalbeforeopen event", () => {
        const spy = jest.fn();
        Fluent("div")
            .addClass("modal")
            .append(Fluent("div").addClass("modal-body").on("modalbeforeopen", spy))
            .appendTo(document.body)
            .trigger("show.bs.modal");
        expect(spy).toHaveBeenCalledTimes(1);
    });

    it("can prevent modal to be shown by calling original event's preventDefault if preventDefault is called on modalbeforeopen ", () => {
        const spy = jest.fn().mockImplementation(e => {
            e.preventDefault();
        });

        var modal = Fluent("div")
            .addClass("modal")
            .append(Fluent("div").addClass("modal-body").on("modalbeforeopen", spy))
            .appendTo(document.body);

        var e = Fluent.trigger(modal.getNode(), "show.bs.modal");
        expect(spy).toHaveBeenCalledTimes(1);
        expect(e.defaultPrevented).toBeTruthy();
    });

    it("installs an event propagation handler for shown.bs.modal to modal body as modalopen event", () => {
        const spy = jest.fn();
        Fluent("div")
            .addClass("modal")
            .append(Fluent("div").addClass("modal-body").on("modalopen", spy))
            .appendTo(document.body)
            .trigger("shown.bs.modal");
        expect(spy).toHaveBeenCalledTimes(1);
    });

});

export { };

