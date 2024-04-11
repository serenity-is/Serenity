import { Dialog, alertDialog, cancelDialogButton, confirmDialog, iframeDialog, informationDialog, noDialogButton, okDialogButton, successDialog, uiAndBSButtonNoConflict, warningDialog, yesDialogButton, type MessageDialogOptions } from "./dialogs";
import { Fluent } from "./fluent";
import { mockJQuery, unmockBSAndJQuery } from "../mocks";

afterEach(function cleanup() {
    document.body.innerHTML = "";
    unmockBSAndJQuery();
    jest.restoreAllMocks();
});

function mockBS5Plus() {
    let modal = jest.fn(function (div: HTMLElement, opt: any) {
        return (div as any).modalInstance = {
            opt: opt,
            dispose: jest.fn(),
            show: jest.fn(),
            hide: jest.fn()
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

function mockJQueryWithBSModal(): any {
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

    it('detects BS3 when modal version starts with 3', async function () {
        let jQuery = mockJQueryWithBSModal();
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
        let $ = mockJQueryWithBSModal();
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

        let $ = mockJQueryWithBSModal();
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

describe("Dialog constructor", () => {
    it("accepts ArrayLike element", () => {
        const body = document.createElement("div");

        const dlg = new Dialog({
            element: [body]
        })
        expect(dlg.getContentNode()).toBe(body);
        dlg.dispose();
    });

    it("removes hidden class from the element", () => {
        const body = document.createElement("div");
        body.classList.add("hidden");

        const dlg = new Dialog({
            element: [body]
        })
        expect(dlg.getContentNode()).toBe(body);
        expect(body.classList.contains("hidden")).toBe(false);
        dlg.dispose();
    });

    it("creates UI dialog if both jQuery UI and Bootstrap are available and preferBSModal is false", () => {
        mockJQueryWithUIDialog();
        mockBS5Plus();
        const dlg = new Dialog({
            preferBSModal: false,
            preferPanel: false
        });
        expect(dlg.getDialogNode().classList.contains("ui-dialog")).toBe(true);
    });
});


describe("alertDialog", () => {
    it('alertDialog uses window.alert when no BS/jQuery UI loaded', async function () {
        let alertSpy = jest.spyOn(window, "alert");
        try {
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
        let bootstrap = mockBS5Plus();
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
            expect((div as any).modalInstance).toBeDefined();
            expect((div as any).modalInstance?.show).toHaveBeenCalledTimes(1);
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

    it('uses window.alert when no BS/jQuery UI loaded', async function () {
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

    it('uses window.alert when no BS/jQuery UI loaded', async function () {
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

    it('uses window.confirm when no BS/jQuery UI loaded', async function () {
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
        let bootstrap = mockBS5Plus();
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
            expect((div as any).modalInstance).toBeDefined();
            expect((div as any).modalInstance?.show).toHaveBeenCalledTimes(1);
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

    it('uses window.alert when no BS/jQuery UI loaded', async function () {
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

    it('uses window.alert when no BS/jQuery UI loaded', async function () {
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
        let bootstrap = mockBS5Plus();
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
            expect((div as any).modalInstance).toBeDefined();
            expect((div as any).modalInstance.show).toHaveBeenCalledTimes(1);
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
        mockBS5Plus();
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

describe("dialog button result handling", () => {

    let dlg: Dialog;
    let okButton: HTMLButtonElement;
    let okClickSpy: jest.Mock;
    let closeSpy: jest.SpyInstance;

    beforeEach(() => {
        okClickSpy = jest.fn();
        dlg = new Dialog({
            buttons: [
                okDialogButton({
                    cssClass: 'ok-button',
                    click: okClickSpy,
                    result: 'ok'
                }),
                cancelDialogButton()
            ]
        });
        okButton = dlg.getFooterNode().querySelector<HTMLButtonElement>(".ok-button");
        closeSpy = jest.spyOn(Dialog.prototype, "close");
    });

    it("closes the dialog after calling the click handler returns void", async function () {
        okButton.click();
        
        expect(okClickSpy).toHaveBeenCalledTimes(1);
        expect(closeSpy).toHaveBeenCalledTimes(1);
        const okOrder = okClickSpy.mock.invocationCallOrder[0];
        const closeOrder = closeSpy.mock.invocationCallOrder[0];
        expect(okOrder).toBeLessThan(closeOrder);
    });

    it("closes the dialog after calling the click handler returns true", async function () {
        okClickSpy.mockImplementation(() => true);

        okButton.click();
        expect(okClickSpy).toHaveBeenCalledTimes(1);
        expect(closeSpy).toHaveBeenCalledTimes(1);
        const okOrder = okClickSpy.mock.invocationCallOrder[0];
        const closeOrder = closeSpy.mock.invocationCallOrder[0];
        expect(okOrder).toBeLessThan(closeOrder);
    });

    it("does not close the dialog if the click handler returns false", async function () {
        okClickSpy.mockImplementation(() => false);

        okButton.click();
        expect(okClickSpy).toHaveBeenCalledTimes(1);
        expect(closeSpy).not.toHaveBeenCalled();
    });

    it("closes the dialog when the promise is resolved if the click handler returns a promise", async function () {
        okClickSpy.mockImplementation(() => Promise.resolve());

        okButton.click();
        expect(okClickSpy).toHaveBeenCalledTimes(1);
        expect(closeSpy).not.toHaveBeenCalled();

        await Promise.resolve();

        expect(closeSpy).toHaveBeenCalledTimes(1);
        const okOrder = okClickSpy.mock.invocationCallOrder[0];
        const closeOrder = closeSpy.mock.invocationCallOrder[0];
        expect(okOrder).toBeLessThan(closeOrder);
    });

    it("does not close the dialog when the promise is rejected if the click handler returns a promise", async function () {
        okClickSpy.mockImplementation(() => Promise.reject("test"));

        okButton.click();

        expect(okClickSpy).toHaveBeenCalledTimes(1);
        expect(closeSpy).not.toHaveBeenCalled();

        await Promise.resolve();

        expect(closeSpy).not.toHaveBeenCalled();
    });

    it("does not close the dialog when the promise returns false if the click handler returns a promise", async function () {
        okClickSpy.mockImplementation(() => Promise.resolve(false));

        okButton.click();

        expect(okClickSpy).toHaveBeenCalledTimes(1);
        expect(closeSpy).not.toHaveBeenCalled();

        await Promise.resolve();
        
        expect(closeSpy).not.toHaveBeenCalled();
    });
});


//describe("dialogButtonToBS", () => {
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

    it("can close panel via jQuery", async function () {
        const jQuery = mockJQuery({});
        const panel = document.body.appendChild(document.createElement('div'));
        panel.classList.add("s-Panel");
        const body = panel.appendChild(document.createElement("div"));
        body.classList.add("panel-body");
        let closingPanel: any;
        let closedPanel: any;
        const panelBeforeClose = (e: any) => closingPanel = e.target;
        const panelClose = (e: any) => closedPanel = e.target;
        Fluent.on(window, 'panelbeforeclose', panelBeforeClose);
        Fluent.on(window, 'panelclose', panelClose);
        try {
            Dialog.getInstance(jQuery(body)).close();
            expect(panel.classList.contains("hidden")).toBe(true);
            expect(closingPanel).toBe(body);
            expect(closedPanel).toBe(body);
        }
        finally {
            Fluent.off(window, 'panelbeforeclose', panelBeforeClose);
            Fluent.off(window, 'panelclose', panelClose);
            panel.remove();
        }
    });

    it("can close panel with Undefined jQuery", async function () {
        const panel = document.body.appendChild(document.createElement('div'));
        panel.classList.add("s-Panel");
        const body = panel.appendChild(document.createElement("div"));
        body.classList.add("panel-body");
        let closingPanel: any;
        let closedPanel: any;
        const panelBeforeClose = (e: any) => closingPanel = e.target;
        const panelClose = (e: any) => closedPanel = e.target;
        Fluent.on(window, 'panelbeforeclose', panelBeforeClose);
        Fluent.on(window, 'panelclose', panelClose);
        try {
            Dialog.getInstance(body).close();
            expect(panel.classList.contains("hidden")).toBe(true);
            expect(closingPanel).toBe(body);
            expect(closedPanel).toBe(body);
        }
        finally {
            Fluent.off(window, 'panelbeforeclose', panelBeforeClose);
            Fluent.off(window, 'panelclose', panelClose);
            panel.remove();
        }
    });

    it("can be cancelled with preventDefault", async function () {
        const panel = document.body.appendChild(document.createElement('div'));
        panel.classList.add("s-Panel");
        const body = panel.appendChild(document.createElement("div"));
        body.classList.add("panel-body");
        try {
            body.addEventListener("panelbeforeclose", e => {
                e.preventDefault();
            });
            Dialog.getInstance(body).close();
            expect(panel.dataset.hiddenby).toBeFalsy();
            expect(panel.classList.contains("hidden")).toBe(false);
        }
        finally {
            panel.remove();
        }
    });

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


describe("Dialog.dispose", () => {
    let bodyEl: HTMLElement;

    beforeEach(() => {
        bodyEl = document.createElement("div");
    });

    afterEach(() => {
        bodyEl = null;
    });

    it("should do nothing if target is not found", () => {
        const dlg = new Dialog();
        (dlg as any).el = null;
        dlg.dispose();
    });

    it("should set private property el to null", () => {
        const dlg = new Dialog();
        dlg.dispose();
        expect(dlg.getDialogNode()).toBe(null);
        expect(dlg.getContentNode()).toBe(null);
        expect(dlg.getEventsNode()).toBe(null);
    });

    it("should use modal instance dispose method if Bootstrap 5+", () => {
        const bootstrap = mockBS5Plus();
        const dlg = new Dialog({ preferBSModal: true, preferPanel: false });
        const modal = bootstrap.Modal.getInstance(dlg.getEventsNode());
        expect(modal).toBeTruthy();
        expect(modal.dispose).not.toHaveBeenCalled();
        dlg.dispose();
        expect(modal.dispose).toHaveBeenCalledTimes(1);
        expect(dlg.getDialogNode()).toBe(null);
        expect(dlg.getContentNode()).toBe(null);
        expect(dlg.getEventsNode()).toBe(null);
    });


    it("should remove panel-body element if s-Panel element is not found", () => {
        const dlg = new Dialog();
        (dlg as any).el = bodyEl;;
        bodyEl.classList.add('panel-body');
        document.body.appendChild(bodyEl);
        dlg.dispose();
        expect(bodyEl.parentNode).toBe(null);
        expect(bodyEl.classList.contains('panel-body')).toBe(false);
        expect((dlg as any).el).toBe(null);
    });

    it("should destroy jQuery UI dialog if target has 'ui-dialog-content' class", () => {
        const dlg = new Dialog();
        (dlg as any).el = bodyEl;
        bodyEl.classList.add("ui-dialog-content");
        document.body.appendChild(bodyEl);
        const destroyMock = jest.fn();
        const dialogMock = jest.fn((method) => {
            if (method == 'destroy')
                destroyMock();
            return this;
        });
        const removeMock = jest.fn();
        const jQueryMock = (window as any).jQuery = jest.fn(() => ({
            dialog: dialogMock,
            remove: removeMock
        }));
        try {
            dlg.dispose();
            expect(jQueryMock).toHaveBeenCalled();
            expect(dialogMock).toHaveBeenCalled();
            expect(destroyMock).toHaveBeenCalled();
            expect(removeMock).toHaveBeenCalled();
            expect(bodyEl.classList.contains('ui-dialog-content')).toBe(false);
        }
        finally {
            delete (window as any).jQuery;
        }
    });

});

describe("Dialog.onOpen", () => {
    it("ignores if panel is disposed", () => {
        const dlg = new Dialog({ preferBSModal: false, preferPanel: true });
        dlg.dispose();
        const onOpen = jest.fn();
        dlg.onOpen(onOpen);
        expect(onOpen).not.toHaveBeenCalled();
    });

    it("attaches an event handler for panelopen event", () => {
        const dlg = new Dialog({ preferBSModal: false, preferPanel: true });
        try {
            const onOpen1 = jest.fn();
            const onOpen2 = jest.fn();
            dlg.onOpen(onOpen1);
            dlg.onOpen(onOpen2);
            expect(onOpen1).not.toHaveBeenCalled();
            expect(onOpen2).not.toHaveBeenCalled();
            dlg.open();
            expect(onOpen1).toHaveBeenCalledTimes(1);
            expect(onOpen2).toHaveBeenCalledTimes(1);
        }
        finally {
            dlg.dispose();
        }
    });

    it("attaches an event handler for panelbeforeopen event if second argument is true", () => {
        const dlg = new Dialog({ preferBSModal: false, preferPanel: true });
        try {
            const beforeOpen1 = jest.fn();
            const beforeOpen2 = jest.fn();
            dlg.onOpen(beforeOpen1, true);
            dlg.onOpen(beforeOpen2, true);
            expect(beforeOpen1).not.toHaveBeenCalled();
            expect(beforeOpen2).not.toHaveBeenCalled();
            dlg.open();
            expect(beforeOpen1).toHaveBeenCalledTimes(1);
            expect(beforeOpen2).toHaveBeenCalledTimes(1);
        }
        finally {
            dlg.dispose();
        }
    });

    it("can abort openining preventDefault is called in panelbeforeopen", () => {
        const dlg = new Dialog({ preferBSModal: false, preferPanel: true, autoOpen: false });
        try {
            const beforeOpen = jest.fn().mockImplementation((e) => e.preventDefault());
            const afterOpen = jest.fn();
            dlg.onOpen(beforeOpen, true);
            dlg.open();
            expect(beforeOpen).toHaveBeenCalledTimes(1);
            expect(afterOpen).not.toHaveBeenCalled();
            expect(dlg.getDialogNode().classList.contains("hidden")).toBe(true);
        }
        finally {
            dlg.dispose();
        }
    });

});

describe("okDialogButton", () => {
    it("should return a dialog button with 'OK' text", () => {
        const button = okDialogButton();
        expect(button.result).toBe("ok");
        expect(button.text).toBe("OK");
        expect(button.cssClass).toBe("btn-info");
    });

    it("should return a dialog button with the specified options", () => {
        const click = jest.fn();
        const button = okDialogButton({ text: "Confirm", cssClass: "btn-primary", result: "test", click });
        expect(button.text).toBe("Confirm");
        expect(button.cssClass).toBe("btn-primary");
        expect(button.result).toBe("test");
        expect(button.click).toBe(click);
    });
});

describe("yesDialogButton", () => {
    it("should return a dialog button with 'Yes' text", () => {
        const button = yesDialogButton();
        expect(button.result).toBe("yes");
        expect(button.text).toBe("Yes");
        expect(button.cssClass).toBe("btn-primary");
    });

    it("should return a dialog button with the specified options", () => {
        const click = jest.fn();
        const button = yesDialogButton({ text: "Agree", cssClass: "btn-success", result: "test", click });
        expect(button.text).toBe("Agree");
        expect(button.cssClass).toBe("btn-success");
        expect(button.result).toBe("test");
        expect(button.click).toBe(click);
    });
});

describe("noDialogButton", () => {
    it("should return a dialog button with 'No' text", () => {
        const button = noDialogButton();
        expect(button.result).toBe("no");
        expect(button.text).toBe("No");
        expect(button.cssClass).toBe("btn-default");
    });

    it("should return a dialog button with the specified options", () => {
        const click = jest.fn();
        const button = noDialogButton({ text: "Disagree", cssClass: "btn-danger", result: "test", click });
        expect(button.text).toBe("Disagree");
        expect(button.cssClass).toBe("btn-danger");
        expect(button.result).toBe("test");
        expect(button.click).toBe(click);
    });
});

describe("cancelDialogButton", () => {
    it("should return a dialog button with 'Cancel' text", () => {
        const button = cancelDialogButton();
        expect(button.result).toBe("cancel");
        expect(button.text).toBe("Cancel");
        expect(button.cssClass).toBe("btn-default");
    });

    it("should return a dialog button with the specified options", () => {
        const click = jest.fn();
        const button = cancelDialogButton({ text: "Abort", cssClass: "btn-secondary", result: "test", click });
        expect(button.text).toBe("Abort");
        expect(button.cssClass).toBe("btn-secondary");
        expect(button.result).toBe("test");
        expect(button.click).toBe(click);
    });
});
