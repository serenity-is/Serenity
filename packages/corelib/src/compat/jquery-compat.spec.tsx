import { jQueryPatch } from "./jquery-compat";

afterEach(() => {
    delete (window as any).jQuery;
});

describe("jQuery CSRF-TOKEN integration", () => {
    it("should set the CSRF token in jQuery ajax headers", () => {
        const jQuery = (window as any).jQuery = vi.fn();
        const ajaxSetup = (jQuery as any).ajaxSetup = vi.fn();

        jQueryPatch();

        expect(ajaxSetup).toHaveBeenCalledTimes(1);
        const beforeSend = ajaxSetup.mock.calls[0][0]?.beforeSend;
        expect(beforeSend).toBeDefined();
        const cookieSpy = (jQuery as any).cookie = vi.fn().mockReturnValue("TEST-TOKEN");
        const setRequestHeader = vi.fn();
        beforeSend({ setRequestHeader }, {});
        expect(cookieSpy).toHaveBeenCalledTimes(1);
        expect(cookieSpy).toHaveBeenCalledWith("CSRF-TOKEN");
        expect(setRequestHeader).toHaveBeenCalledTimes(1);
        expect(setRequestHeader).toHaveBeenCalledWith("X-CSRF-TOKEN", "TEST-TOKEN");
    });

    it("should not set the CSRF token for cross origin request", () => {
        const jQuery = (window as any).jQuery = vi.fn();
        const ajaxSetup = (jQuery as any).ajaxSetup = vi.fn();

        jQueryPatch();

        expect(ajaxSetup).toHaveBeenCalledTimes(1);
        const beforeSend = ajaxSetup.mock.calls[0][0]?.beforeSend;
        expect(beforeSend).toBeDefined();
        const cookieSpy = (jQuery as any).cookie = vi.fn().mockReturnValue("TEST-TOKEN");
        const setRequestHeader = vi.fn();
        beforeSend({ setRequestHeader }, { crossDomain: true });
        expect(cookieSpy).not.toHaveBeenCalled();
        expect(setRequestHeader).not.toHaveBeenCalled();
    });
});

describe("jQuery UI Fixes", () => {
    it("should override _allowInteraction method in jQuery UI dialog prototype", () => {
        const jQuery = (window as any).jQuery = vi.fn();
        const ui = (jQuery as any).ui = {
            dialog: {
                prototype: {
                    _allowInteraction: null
                }
            }
        };

        jQueryPatch();

        expect(ui.dialog.prototype._allowInteraction).toBeDefined();
        const allowInteraction = ui.dialog.prototype._allowInteraction;
        expect(allowInteraction({ target: (<div class="ui-dialog"><div></div></div>).firstChild })).toBe(true);
        expect(allowInteraction({ target: (<div class="another-div"><div></div></div>).firstChild })).toBe(false);
        expect(allowInteraction({ target: (<div class="ui-datepicker"><div></div></div>).firstChild })).toBe(true);
        expect(allowInteraction({ target: (<div class="select2-drop"><div></div></div>).firstChild })).toBe(true);
        expect(allowInteraction({ target: (<div class="cke"><div></div></div>).firstChild })).toBe(true);
        expect(allowInteraction({ target: (<div class="cke_dialog"><div></div></div>).firstChild })).toBe(true);
        expect(allowInteraction({ target: (<div class="modal"><div></div></div>).firstChild })).toBe(true);
        expect(allowInteraction({ target: (<div id="support-modal"><div></div></div>).firstChild })).toBe(true);
    });

    it("should override _focusTabbable method in jQuery UI dialog prototype", () => {
        const jQuery = (window as any).jQuery = vi.fn();
        const ui = (jQuery as any).ui = {
            dialog: {
                prototype: {
                    _focusTabbable: vi.fn()
                }
            }
        };

        jQueryPatch();

        expect(ui.dialog.prototype._focusTabbable).toBeDefined();
        const focusTabbable = ui.dialog.prototype._focusTabbable;
        const origMock = vi.fn();
        focusTabbable.call({ uiDialog: { focus: origMock } });
        expect(origMock).toHaveBeenCalled();
    });

    it("should override _createTitlebar method in jQuery UI dialog prototype", () => {
        const jQuery = (window as any).jQuery = vi.fn();
        const ui = (jQuery as any).ui = {
            dialog: {
                prototype: {
                    _createTitlebar: vi.fn()
                }
            }
        };

        jQueryPatch();

        expect(ui.dialog.prototype._createTitlebar).toBeDefined();
        const createTitlebar = ui.dialog.prototype._createTitlebar;
        const findMock = vi.fn().mockReturnValue({ html: vi.fn() });
        createTitlebar.call({ uiDialogTitlebar: { find: findMock } });
        expect(findMock).toHaveBeenCalledWith('.ui-dialog-titlebar-close');
    });
});

describe("jQuery Clean Data Patch", () => {
    it("should call disposing event handlers before cleaning data", () => {
        const jQuery = (window as any).jQuery = vi.fn();
        const cleanDataOrig = vi.fn();
        const _dataMock = vi.fn().mockImplementation((element, key) => {
            if (key === "events") {
                return {
                    disposing: [
                        { handler: vi.fn() },
                        { handler: vi.fn() }
                    ]
                };
            }
            return null;
        });

        (jQuery as any).fn = {};
        (jQuery as any).cleanData = cleanDataOrig;
        (jQuery as any)._data = _dataMock;

        jQueryPatch();

        const elements = [{}, {}];
        (jQuery as any).cleanData(elements);

        expect(_dataMock).toHaveBeenCalledTimes(elements.length);
        elements.forEach((element, index) => {
            const events = _dataMock.mock.results[index].value;
            if (events && events.disposing) {
                events.disposing.forEach((handlerObj: any) => {
                    expect(handlerObj.handler).toHaveBeenCalledWith({ target: element });
                });
            }
        });
        expect(cleanDataOrig).toHaveBeenCalledWith(elements);
    });
});


