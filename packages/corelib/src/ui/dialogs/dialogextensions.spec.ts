import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { DialogExtensions, UIDialogMaximizer } from "./dialogextensions";

describe("DialogExtensions.dialogResizable", () => {
    let mockDialogInstance: any;
    let mockElement: HTMLElement;

    beforeEach(() => {
        mockElement = document.createElement("div");

        mockDialogInstance = {
            dialog: vi.fn((method: string, option?: any, value?: any) => {
                if (method === "option" && typeof option === "string") {
                    return mockDialogInstance;
                }
                return mockDialogInstance;
            })
        };

        // Create a mock jQuery
        const mock$ = vi.fn((el: any) => ({
            [0]: el,
            length: 1,
            dialog: vi.fn(() => mockDialogInstance)
        })) as any;
        mock$.fn = {};
        (window as any).jQuery = mock$;
    });

    afterEach(() => {
        delete (window as any).jQuery;
    });

    it("returns early if no jQuery is available", () => {
        delete (window as any).jQuery;
        // Should not throw
        DialogExtensions.dialogResizable(mockElement);
        expect(true).toBe(true);
    });

    it("returns early if dialog() method returns falsy", () => {
        const mock$ = vi.fn(() => ({
            dialog: vi.fn(() => undefined)
        })) as any;
        mock$.fn = {};
        (window as any).jQuery = mock$;

        DialogExtensions.dialogResizable(mockElement);
        expect(true).toBe(true);
    });

    it("sets resizable to true", () => {
        DialogExtensions.dialogResizable(mockElement);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "resizable", true);
    });

    it("sets minWidth when provided", () => {
        DialogExtensions.dialogResizable(mockElement, undefined, undefined, 300);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "minWidth", 300);
    });

    it("sets width when provided", () => {
        DialogExtensions.dialogResizable(mockElement, 500);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "width", 500);
    });

    it("sets minHeight when provided", () => {
        DialogExtensions.dialogResizable(mockElement, undefined, undefined, undefined, 200);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "minHeight", 200);
    });

    it("sets height when provided", () => {
        DialogExtensions.dialogResizable(mockElement, undefined, 400);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "height", 400);
    });

    it("sets all size options when provided", () => {
        DialogExtensions.dialogResizable(mockElement, 500, 400, 300, 200);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "resizable", true);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "minWidth", 300);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "width", 500);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "minHeight", 200);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "height", 400);
    });

    it("accepts ArrayLike<HTMLElement> as argument", () => {
        const arrLike = { 0: mockElement, length: 1 } as any;
        DialogExtensions.dialogResizable(arrLike);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "resizable", true);
    });
});

function createMockChain(element: HTMLElement): any {
    const chain: any = {
        [0]: element,
        length: 1,
        closest: vi.fn(() => chain),
        children: vi.fn(() => chain),
        find: vi.fn(() => chain),
        first: vi.fn(() => chain),
        removeClass: vi.fn(() => chain),
        toggleClass: vi.fn(() => chain),
        select: vi.fn(() => chain),
        attr: vi.fn(() => chain),
        toggle: vi.fn(() => chain),
        dblclick: vi.fn(() => chain),
        click: vi.fn(() => chain),
        insertBefore: vi.fn(() => chain),
        appendTo: vi.fn(() => chain),
        dialog: vi.fn((method?: string, option?: any) => {
            if (method === "option" && option === "width")
                return 800;
            if (method === "option" && option === "maxHeight")
                return 700;
            if (method === "option" && option === "resizable")
                return true;
            if (method === "option" && option === "draggable")
                return true;
            return chain;
        }),
        draggable: vi.fn(() => chain),
        end: vi.fn(() => chain),
        show: vi.fn(() => chain),
        css: vi.fn(() => chain),
        outerHeight: vi.fn(() => 500),
        offset: vi.fn(() => ({ left: 100, top: 200 })),
        scrollLeft: vi.fn(() => 0),
        scrollTop: vi.fn(() => 0),
        height: vi.fn(() => 600),
        width: vi.fn(() => 800),
        triggerHandler: vi.fn(() => chain),
    };
    return chain;
}

describe("DialogExtensions.dialogMaximizable", () => {
    let mockElement: HTMLElement;

    beforeEach(() => {
        mockElement = document.createElement("div");
    });

    afterEach(() => {
        delete (window as any).jQuery;
    });

    it("returns early if no jQuery is available", () => {
        delete (window as any).jQuery;
        expect(() => DialogExtensions.dialogMaximizable(mockElement)).not.toThrow();
    });

    it("adds maximize and restore buttons to the dialog titlebar", () => {
        const chain = createMockChain(mockElement);
        const mock$ = vi.fn(() => chain) as any;
        mock$.fn = {};
        (window as any).jQuery = mock$;

        DialogExtensions.dialogMaximizable(mockElement);

        // one dblclick handler on the titlebar, plus a click handler per button
        expect(chain.dblclick).toHaveBeenCalledTimes(1);
        expect(chain.click).toHaveBeenCalledTimes(2);
    });

    it("accepts ArrayLike<HTMLElement> as argument", () => {
        const chain = createMockChain(mockElement);
        const mock$ = vi.fn(() => chain) as any;
        mock$.fn = {};
        (window as any).jQuery = mock$;

        const arrLike = { 0: mockElement, length: 1 } as any;
        expect(() => DialogExtensions.dialogMaximizable(arrLike)).not.toThrow();
    });

    it("appends buttons to the titlebar when there is no close button", () => {
        const chain = createMockChain(mockElement);
        const empty = { ...chain, length: 0, first: vi.fn(() => empty) };
        chain.find = vi.fn(() => empty);
        const mock$ = vi.fn(() => chain) as any;
        mock$.fn = {};
        (window as any).jQuery = mock$;

        DialogExtensions.dialogMaximizable(mockElement);

        expect(chain.appendTo).toHaveBeenCalled();
        expect(chain.insertBefore).not.toHaveBeenCalled();
    });

    it("maximizes via the maximize button and restores via the restore button", () => {
        const chain = createMockChain(mockElement);
        const mock$ = vi.fn(() => chain) as any;
        mock$.fn = {};
        (window as any).jQuery = mock$;

        DialogExtensions.dialogMaximizable(mockElement);

        // First added button is "maximize", second is "restore"
        const maximizeClick = chain.click.mock.calls[0][0];
        const restoreClick = chain.click.mock.calls[1][0];

        maximizeClick({ preventDefault: vi.fn() });
        expect(chain.dialog).toHaveBeenCalledWith("option", expect.objectContaining({
            resizable: false,
            draggable: false,
            height: 599,
            width: 799
        }));

        restoreClick({ preventDefault: vi.fn() });
        expect(chain.dialog).toHaveBeenCalledWith("option", expect.objectContaining({
            resizable: true,
            draggable: true,
            height: 500,
            width: 800
        }));
    });

    it("maximizes on titlebar double click", () => {
        const chain = createMockChain(mockElement);
        const mock$ = vi.fn(() => chain) as any;
        mock$.fn = {};
        (window as any).jQuery = mock$;

        DialogExtensions.dialogMaximizable(mockElement);

        const dblClickHandler = chain.dblclick.mock.calls[0][0];
        dblClickHandler();
        expect(chain.dialog).toHaveBeenCalledWith("option", expect.objectContaining({
            resizable: false,
            draggable: false
        }));
    });
});

describe("UIDialogMaximizer", () => {
    let mockElement: HTMLElement;
    let chain: any;

    beforeEach(() => {
        mockElement = document.createElement("div");
        chain = createMockChain(mockElement);
        const mock$ = vi.fn(() => chain) as any;
        mock$.fn = {};
        (window as any).jQuery = mock$;
    });

    afterEach(() => {
        delete (window as any).jQuery;
    });

    it("starts in normal state", () => {
        const maximizer = new UIDialogMaximizer({ element: mockElement });
        expect(maximizer.isMaximized).toBe(false);
    });

    it("throws when jQuery is not available", () => {
        delete (window as any).jQuery;
        expect(() => new UIDialogMaximizer({ element: mockElement })).toThrow();
    });

    it("maximizes and restores, tracking state", () => {
        const dlg = new UIDialogMaximizer({ element: mockElement, showButton: true, dblclick: true });
        expect(dlg.isMaximized).toBe(false);

        dlg.maximize();
        expect(dlg.isMaximized).toBe(true);
        expect(chain.dialog).toHaveBeenCalledWith("option", expect.objectContaining({
            resizable: false,
            draggable: false
        }));

        dlg.restore();
        expect(dlg.isMaximized).toBe(false);
    });
});

describe("DialogExtensions dialogResizable advanced", () => {
    let mockDialogInstance: any;

    beforeEach(() => {
        mockDialogInstance = {
            dialog: vi.fn((method: string, option?: any, value?: any) => {
                return mockDialogInstance;
            })
        };
        const mock$ = vi.fn((el: any) => ({
            [0]: el,
            length: 1,
            dialog: vi.fn(() => mockDialogInstance)
        })) as any;
        mock$.fn = {};
        (window as any).jQuery = mock$;
    });

    afterEach(() => {
        delete (window as any).jQuery;
    });

    it("sets all options correctly", () => {
        const el = document.createElement("div");
        DialogExtensions.dialogResizable(el, 500, 400, 300, 200);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "resizable", true);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "minWidth", 300);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "width", 500);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "minHeight", 200);
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "height", 400);
    });

    it("skips null options", () => {
        const el = document.createElement("div");
        DialogExtensions.dialogResizable(el, null, null, null, null);
        // resizable should still be set to true
        expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "resizable", true);
        // minWidth should not be called with null
        expect(mockDialogInstance.dialog).not.toHaveBeenCalledWith("option", "minWidth", null);
    });
});
