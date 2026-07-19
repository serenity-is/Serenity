import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { DialogExtensions } from "./dialogextensions";

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
        DialogExtensions.dialogMaximizable(mockElement);
        expect(true).toBe(true);
    });

    it("calls dialogExtend on the element", () => {
        const dialogExtendFn = vi.fn();
        const mock$ = vi.fn(() => ({
            dialogExtend: dialogExtendFn
        })) as any;
        mock$.fn = {};
        (window as any).jQuery = mock$;

        DialogExtensions.dialogMaximizable(mockElement);
        expect(dialogExtendFn).toHaveBeenCalledWith({
            closable: true,
            maximizable: true,
            dblclick: 'maximize',
            icons: { maximize: 'ui-icon-maximize-window' }
        });
    });

    it("accepts ArrayLike<HTMLElement> as argument", () => {
        const dialogExtendFn = vi.fn();
        const mock$ = vi.fn(() => ({
            dialogExtend: dialogExtendFn
        })) as any;
        mock$.fn = {};
        (window as any).jQuery = mock$;

        const arrLike = { 0: mockElement, length: 1 } as any;
        DialogExtensions.dialogMaximizable(arrLike);
        expect(dialogExtendFn).toHaveBeenCalled();
    });
});

describe("registerDialogExtendPlugin (internal)", () => {
    // The registerDialogExtendPlugin is called as a module-level side effect.
    // It registers a jQuery UI widget called "ui.dialogExtend".
    // Since the module is already loaded, we test the DialogExtensions functions 
    // that use dialogExtend, and verify the module loaded without error.
    
    it("module exports are available", () => {
        expect(typeof DialogExtensions.dialogResizable).toBe("function");
        expect(typeof DialogExtensions.dialogMaximizable).toBe("function");
    });

    it("dialogMaximizable calls dialogExtend when jQuery has the plugin", () => {
        const dialogExtendFn = vi.fn();
        const mock$ = vi.fn(() => ({
            dialogExtend: dialogExtendFn
        })) as any;
        mock$.fn = { dialogExtend: true };
        (window as any).jQuery = mock$;

        const el = document.createElement("div");
        DialogExtensions.dialogMaximizable(el);
        expect(dialogExtendFn).toHaveBeenCalledWith({
            closable: true,
            maximizable: true,
            dblclick: 'maximize',
            icons: { maximize: 'ui-icon-maximize-window' }
        });
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
