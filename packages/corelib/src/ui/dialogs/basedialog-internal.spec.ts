import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { applyCssSizes, handleUIDialogResponsive } from "./basedialog-internal";

// Mock compat module to control isMobileView
vi.mock("../../compat", async (importOriginal) => {
    const actual: any = await importOriginal();
    return {
        ...actual,
        isMobileView: vi.fn(() => false),
        layoutFillHeight: vi.fn()
    };
});

describe("applyCssSizes", () => {
    let originalGetComputedStyle: typeof window.getComputedStyle;

    beforeEach(() => {
        document.body.innerHTML = "";
        originalGetComputedStyle = window.getComputedStyle;
    });

    afterEach(() => {
        window.getComputedStyle = originalGetComputedStyle;
    });

    function mockComputedStyle(styles: Record<string, string>) {
        window.getComputedStyle = ((el: HTMLElement, pseudo?: string | null) => {
            if (el.classList?.contains?.("size")) {
                return {
                    getPropertyValue: (prop: string) => styles[prop] ?? "",
                } as any;
            }
            return originalGetComputedStyle(el, pseudo);
        }) as any;
    }

    it("returns undefined for opt properties when no size elements exist", () => {
        const opt: any = {};
        applyCssSizes(opt, "test-dialog-class");
        expect(opt.minWidth).toBeUndefined();
        expect(opt.width).toBeUndefined();
        expect(opt.height).toBeUndefined();
        expect(opt.minHeight).toBeUndefined();
    });

    it("reads size values from size element's computed style", () => {
        mockComputedStyle({
            minWidth: "300px",
            width: "500px",
            height: "400px",
            minHeight: "200px"
        });

        const opt: any = {};
        applyCssSizes(opt, "test-size-dlg");

        expect(opt.minWidth).toBe(300);
        expect(opt.width).toBe(500);
        expect(opt.height).toBe(400);
        expect(opt.minHeight).toBe(200);
    });

    it("handles partial size values", () => {
        mockComputedStyle({
            minWidth: "150px"
        });

        const opt: any = {};
        applyCssSizes(opt, "partial-size-dlg");

        expect(opt.minWidth).toBe(150);
        expect(opt.width).toBeUndefined();
        expect(opt.height).toBeUndefined();
        expect(opt.minHeight).toBeUndefined();
    });

    it("handles zero values by not setting opt properties", () => {
        mockComputedStyle({
            minWidth: "0px",
            width: "0px"
        });

        const opt: any = {};
        applyCssSizes(opt, "zero-size-dlg");

        // getCssSize returns null for zero values, so the property is not set
        expect(opt.minWidth).toBeUndefined();
        expect(opt.width).toBeUndefined();
        expect(opt.height).toBeUndefined();
    });

    it("handles non-px values like auto by not setting opt properties", () => {
        mockComputedStyle({
            minWidth: "auto",
            width: "auto"
        });

        const opt: any = {};
        applyCssSizes(opt, "auto-size-dlg");

        // getCssSize returns null for non-px values, so the property is not set
        expect(opt.minWidth).toBeUndefined();
        expect(opt.width).toBeUndefined();
    });

    it("handles empty dialogClass string", () => {
        const opt: any = {};
        applyCssSizes(opt, "");
        expect(opt.minWidth).toBeUndefined();
    });

    it("handles NaN values in px string", () => {
        mockComputedStyle({
            minWidth: "abcpx"
        });

        const opt: any = {};
        applyCssSizes(opt, "nan-size-dlg");
        // parseInt("abc", 10) returns NaN, so getCssSize returns null, property not set
        expect(opt.minWidth).toBeUndefined();
    });
});

describe("handleUIDialogResponsive", () => {
    let mockDialogInstance: any;
    let mockDomNode: HTMLElement;
    let mockUiDialog: HTMLElement;

    beforeEach(() => {
        document.body.innerHTML = "";

        mockDomNode = document.createElement("div");
        mockDomNode.id = "test-dialog-node";
        document.body.appendChild(mockDomNode);

        mockUiDialog = document.createElement("div");
        mockUiDialog.className = "ui-dialog";
        mockUiDialog.appendChild(mockDomNode);
        document.body.appendChild(mockUiDialog);

        mockDialogInstance = {
            dialog: vi.fn((method?: string, opt?: any) => {
                if (method === "option" && typeof opt === "string") {
                    if (opt === "draggable") return true;
                    if (opt === "resizable") return true;
                    return undefined;
                }
                return mockDialogInstance;
            }),
            css: vi.fn((prop: string) => "absolute")
        };

        const mock$ = vi.fn((selector: any) => {
            const isDomNode = selector === mockDomNode || selector === document.body || selector === window;
            if (isDomNode || typeof selector === 'string') {
                return {
                    [0]: isDomNode ? selector : null,
                    length: 1,
                    dialog: vi.fn(() => mockDialogInstance),
                    data: vi.fn((key: string) => undefined),
                    removeData: vi.fn(),
                    width: vi.fn(() => 800),
                    height: vi.fn(() => 600),
                    scrollTop: vi.fn(),
                    closest: vi.fn((sel: string) => {
                        if (sel === '.ui-dialog') {
                            return {
                                length: 1,
                                [0]: mockUiDialog,
                                position: vi.fn(() => ({ left: 100, top: 50 })),
                                width: vi.fn(() => 800),
                                height: vi.fn(() => 600),
                                addClass: vi.fn(),
                                removeClass: vi.fn(),
                                css: vi.fn(() => "absolute"),
                                find: vi.fn(() => ({
                                    first: vi.fn(() => ({
                                        insertBefore: vi.fn(),
                                        appendTo: vi.fn()
                                    }))
                                })),
                                data: vi.fn((k: string) => undefined),
                                removeData: vi.fn()
                            } as any;
                        }
                        return { length: 0 };
                    })
                } as any;
            }
            return { length: 0 } as any;
        });
        mock$.fn = {};
        (window as any).jQuery = mock$;
    });

    afterEach(() => {
        delete (window as any).jQuery;
        document.body.innerHTML = "";
    });

    it("returns early if no jQuery is available", () => {
        delete (window as any).jQuery;
        handleUIDialogResponsive(mockDomNode);
        expect(true).toBe(true);
    });

    it("returns early if domNode has no dialog() method", () => {
        const noDialog$ = vi.fn(() => ({
            [0]: mockDomNode,
            length: 1,
            dialog: vi.fn(() => undefined as any),
            data: vi.fn(() => undefined),
            closest: vi.fn(() => ({
                length: 1,
                [0]: mockUiDialog
            }))
        })) as any;
        noDialog$.fn = {};
        (window as any).jQuery = noDialog$;

        handleUIDialogResponsive(mockDomNode);
        expect(true).toBe(true);
    });

    it("returns early if no ui-dialog wrapper found", () => {
        document.body.innerHTML = "";
        document.body.appendChild(mockDomNode);

        const simpleMock = vi.fn((sel: any) => ({
            [0]: sel,
            length: 1,
            dialog: vi.fn(() => mockDialogInstance),
            closest: vi.fn(() => ({ length: 0 })),
            data: vi.fn(() => undefined),
            width: vi.fn(() => 800),
            height: vi.fn(() => 600),
        })) as any;
        simpleMock.fn = {};
        (window as any).jQuery = simpleMock;

        handleUIDialogResponsive(mockDomNode);
        expect(true).toBe(true);
    });

    describe("mobile branch", () => {
        beforeEach(async () => {
            const compat = await import("../../compat");
            (compat.isMobileView as any).mockReturnValue(true);
        });

        it("saves responsive data on first entry", () => {
            handleUIDialogResponsive(mockDomNode);
            // dlg.dialog('option', 'draggable') should have been called
            expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "draggable");
            // dlg.dialog('option', 'resizable') should have been called
            expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "resizable");
            // Draggable should be disabled
            expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "draggable", false);
            // Resizable should be disabled
            expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "resizable", false);
        });

        it("adds mobile-layout class and sets fixed position", () => {
            const addClassSpy = vi.fn();
            const cssSpy = vi.fn();
            const mock$ = vi.fn((selector: any) => {
                const isBody = selector === document.body || selector === window;
                return {
                    [0]: isBody ? selector : mockDomNode,
                    length: 1,
                    dialog: vi.fn(() => mockDialogInstance),
                    data: vi.fn(() => undefined),
                    removeData: vi.fn(),
                    width: vi.fn(() => 800),
                    height: vi.fn(() => 600),
                    scrollTop: vi.fn(),
                    closest: vi.fn((sel: string) => {
                        if (sel === '.ui-dialog') {
                        return {
                            length: 1,
                            [0]: mockUiDialog,
                            position: vi.fn(() => ({ left: 100, top: 50 })),
                            width: vi.fn(() => 800),
                            height: vi.fn(() => 600),
                            addClass: addClassSpy,
                            removeClass: vi.fn(),
                            css: cssSpy,
                            find: vi.fn(() => ({
                                first: vi.fn(() => ({ insertBefore: vi.fn(), appendTo: vi.fn() }))
                            })),
                            data: vi.fn((k: string) => undefined),
                            removeData: vi.fn()
                        } as any;
                    }
                    return { length: 0 };
                })
            }
        }) as any;
            mock$.fn = {};
            (window as any).jQuery = mock$;

            handleUIDialogResponsive(mockDomNode);

            expect(addClassSpy).toHaveBeenCalledWith('mobile-layout');
            expect(cssSpy).toHaveBeenCalledWith({ left: '0px', top: '0px', width: expect.stringContaining('px'), height: expect.stringContaining('px'), position: 'fixed' });
        });

        it("skips save if responsiveData already exists", () => {
            const dataMock = vi.fn((key: string) => {
                if (key === 'responsiveData') return { draggable: true, resizable: true };
                return undefined;
            });
            const mock$ = vi.fn((selector: any) => {
                const isBody = selector === document.body || selector === window;
                return {
                    [0]: isBody ? selector : mockDomNode,
                    length: 1,
                    dialog: vi.fn(() => mockDialogInstance),
                    data: dataMock,
                    removeData: vi.fn(),
                    width: vi.fn(() => 800),
                    height: vi.fn(() => 600),
                    scrollTop: vi.fn(),
                    closest: vi.fn((sel: string) => {
                        if (sel === '.ui-dialog') {
                            return {
                                length: 1,
                                [0]: mockUiDialog,
                                position: vi.fn(() => ({ left: 100, top: 50 })),
                                width: vi.fn(() => 800),
                                height: vi.fn(() => 600),
                                addClass: vi.fn(),
                                removeClass: vi.fn(),
                                css: vi.fn(),
                                find: vi.fn(() => ({
                                    first: vi.fn(() => ({ insertBefore: vi.fn(), appendTo: vi.fn() }))
                                })),
                                data: dataMock,
                                removeData: vi.fn()
                            } as any;
                        }
                        return { length: 0 };
                    })
                } as any;
            }) as any;
            mock$.fn = {};
            (window as any).jQuery = mock$;

            handleUIDialogResponsive(mockDomNode);

            // draggable should NOT be disabled since responsiveData already existed
            expect(mockDialogInstance.dialog).not.toHaveBeenCalledWith("option", "draggable", false);
            expect(mockDialogInstance.dialog).not.toHaveBeenCalledWith("option", "resizable", false);
        });
    });

    describe("restore branch", () => {
        beforeEach(async () => {
            const compat = await import("../../compat");
            (compat.isMobileView as any).mockReturnValue(false);
        });

        it("restores saved responsive data", () => {
            const savedData = {
                draggable: true,
                resizable: true,
                position: 'absolute',
                left: 100,
                top: 50,
                width: 800,
                height: 600,
                contentHeight: 400
            };
            const dataMock = vi.fn((key: string) => {
                if (key === 'responsiveData') return savedData;
                return undefined;
            });
            const removeDataSpy = vi.fn();
            const removeClassSpy = vi.fn();
            const cssSpy = vi.fn();
            const heightSpy = vi.fn();

            const mock$ = vi.fn((selector: any) => {
                if (selector === mockDomNode) {
                    return {
                        [0]: mockDomNode,
                        length: 1,
                        dialog: vi.fn(() => mockDialogInstance),
                        data: dataMock,
                        removeData: removeDataSpy,
                        width: vi.fn(() => 800),
                        height: heightSpy,
                        closest: vi.fn((sel: string) => {
                            if (sel === '.ui-dialog') {
                                return {
                                    length: 1,
                                    [0]: mockUiDialog,
                                    position: vi.fn(() => ({ left: 100, top: 50 })),
                                    width: vi.fn(() => 800),
                                    height: vi.fn(() => 600),
                                    addClass: vi.fn(),
                                    removeClass: removeClassSpy,
                                    css: cssSpy,
                                    find: vi.fn(() => ({
                                        first: vi.fn(() => ({ insertBefore: vi.fn(), appendTo: vi.fn() }))
                                    })),
                                    data: dataMock,
                                    removeData: vi.fn()
                                } as any;
                            }
                            return { length: 0 };
                        })
                    } as any;
                }
                return { length: 0 } as any;
            });
            mock$.fn = {};
            (window as any).jQuery = mock$;

            handleUIDialogResponsive(mockDomNode);

            // Verify restore: draggable and resizable should be restored
            expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "draggable", true);
            expect(mockDialogInstance.dialog).toHaveBeenCalledWith("option", "resizable", true);
            // Verify CSS was restored
            expect(cssSpy).toHaveBeenCalled();
            // Verify height restored
            expect(heightSpy).toHaveBeenCalledWith(400);
            // Verify mobile-layout removed
            expect(removeClassSpy).toHaveBeenCalledWith('mobile-layout');
            // Verify data removed
            expect(removeDataSpy).toHaveBeenCalledWith('responsiveData');
        });
    });
});
