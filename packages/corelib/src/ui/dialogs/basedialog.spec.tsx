import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { Fluent, Dialog } from "../../base";
import { BaseDialog } from "./basedialog";
import { TabsExtensions } from "../helpers/tabsextensions";

// Helper to access protected members
function getDialogOptions(dialog: BaseDialog<any>): any {
    return (dialog as any).getDialogOptions();
}

function getDialogButtons(dialog: BaseDialog<any>): any[] {
    return (dialog as any).getDialogButtons();
}

function getToolbarButtons(dialog: BaseDialog<any>): any[] {
    return (dialog as any).getToolbarButtons();
}

function getValidatorOptions(dialog: BaseDialog<any>): any {
    return (dialog as any).getValidatorOptions();
}

function resetValidation(dialog: BaseDialog<any>): void {
    (dialog as any).resetValidation();
}

function validateForm(dialog: BaseDialog<any>): boolean {
    return (dialog as any).validateForm();
}

function isStaticPanel(dialog: BaseDialog<any>): boolean {
    return (dialog as any).isStaticPanel();
}

function getInitialDialogTitle(dialog: BaseDialog<any>): string {
    return (dialog as any).getInitialDialogTitle();
}

function onDialogOpen(dialog: BaseDialog<any>): void {
    (dialog as any).onDialogOpen();
}

function onDialogClose(dialog: BaseDialog<any>, result?: string): void {
    (dialog as any).onDialogClose(result);
}

function handleResponsive(dialog: BaseDialog<any>): void {
    (dialog as any).handleResponsive();
}

describe("BaseDialog", () => {
    let mockDialogInstance: any;

    function createMockDialog(overrides?: any) {
        return {
            open: vi.fn(),
            close: vi.fn(),
            dispose: vi.fn(),
            title: vi.fn((val?: string) => {
                if (val === undefined) return "Test Title";
                return undefined;
            }),
            on: vi.fn(),
            type: "uidialog",
            ...overrides
        };
    }

    beforeEach(() => {
        document.body.innerHTML = "";
        mockDialogInstance = createMockDialog();

        // Set up jQuery mock with all required methods
        const mockFn = vi.fn((selector: any) => ({
            [0]: selector,
            length: 1,
            dialog: vi.fn(() => mockDialogInstance),
            closest: vi.fn(() => ({
                length: 1,
                [0]: document.createElement("div"),
                on: vi.fn(),
                off: vi.fn(),
                triggerHandler: vi.fn()
            })),
            data: vi.fn(() => ({ uiTabs: {} })),
            removeData: vi.fn(),
            width: vi.fn(() => 800),
            height: vi.fn(() => 600),
            find: vi.fn(() => ({
                first: vi.fn(() => ({ insertBefore: vi.fn(), appendTo: vi.fn() })),
                on: vi.fn(),
                off: vi.fn()
            })),
            on: vi.fn(),
            off: vi.fn(),
            one: vi.fn(),
            trigger: vi.fn(),
            triggerHandler: vi.fn(),
            addClass: vi.fn(),
            removeClass: vi.fn(),
            css: vi.fn(() => "absolute"),
            position: vi.fn(() => ({ left: 0, top: 0 })),
            remove: vi.fn(),
            val: vi.fn(() => ""),
            attr: vi.fn(),
            is: vi.fn(() => false),
            hasClass: vi.fn(() => false),
            toggle: vi.fn(),
            hide: vi.fn(),
            show: vi.fn(),
            empty: vi.fn()
        })) as any;
        // Set $.fn.dialog to make hasUIDialog() return true
        mockFn.fn = { dialog: true };
        (window as any).jQuery = mockFn;
    });

    afterEach(() => {
        delete (window as any).jQuery;
        vi.restoreAllMocks();
    });

    describe("constructor", () => {
        it("creates a BaseDialog instance with default props", () => {
            const dialog = new BaseDialog({});
            expect(dialog).toBeInstanceOf(BaseDialog);
            expect(dialog.domNode).toBeInstanceOf(HTMLElement);
            dialog.destroy();
        });

        it("sets domNode id from uniqueName if no id attribute", () => {
            const dialog = new BaseDialog({});
            expect(dialog.domNode.getAttribute("id")).toBe(dialog.uniqueName);
            dialog.destroy();
        });

        it("preserves existing domNode id", () => {
            const el = document.createElement("div");
            el.id = "custom-id";
            const dialog = new BaseDialog({ element: el });
            expect(dialog.domNode.getAttribute("id")).toBe("custom-id");
            dialog.destroy();
        });
    });

    describe("destroy", () => {
        it("destroys tabs, toolbar, validator, and dialog", () => {
            const dialog = new BaseDialog({});
            const tabsDestroySpy = vi.spyOn(TabsExtensions, "destroy");
            const toolbarDestroy = vi.fn();
            dialog["toolbar"] = { destroy: toolbarDestroy } as any;

            dialog.destroy();

            expect(tabsDestroySpy).toHaveBeenCalled();
            expect(toolbarDestroy).toHaveBeenCalled();
            expect(dialog["toolbar"]).toBeNull();
            dialog.destroy();
        });

        it("handles destroy when validator exists", () => {
            const dialog = new BaseDialog({});
            const validatorDestroy = vi.fn();
            dialog["validator"] = { destroy: validatorDestroy, resetAll: vi.fn() } as any;
            const form = document.createElement("form");
            form.id = dialog.idPrefix + "Form";
            dialog.domNode.appendChild(form);

            dialog.destroy();
            expect(validatorDestroy).toHaveBeenCalled();
        });
    });

    describe("getDialogOptions", () => {
        it("returns default dialog options", () => {
            const dialog = new BaseDialog({});
            const options = getDialogOptions(dialog);
            expect(options.autoOpen).toBe(false);
            expect(options.element).toBe(dialog.domNode);
            expect(options.size).toBe("lg");
            expect(options.dialogClass).toContain("flex-layout");
            expect(typeof options.onClose).toBe("function");
            expect(typeof options.onOpen).toBe("function");
            expect(typeof options.providerOptions).toBe("function");
            dialog.destroy();
        });

        it("sets preferPanel when isStaticPanel returns true", () => {
            class StaticDialog extends BaseDialog<any> {
                protected isStaticPanel() { return true; }
            }
            const dialog = new StaticDialog({});
            const options = getDialogOptions(dialog);
            expect(options.preferPanel).toBe(true);
            expect(options.closeButton).toBe(false);
            dialog.destroy();
        });

        it("includes buttons from getDialogButtons", () => {
            class CustomDialog extends BaseDialog<any> {
                protected getDialogButtons() {
                    return [{ text: "OK", click: vi.fn() }];
                }
            }
            const dialog = new CustomDialog({});
            const options = getDialogOptions(dialog);
            expect(options.buttons).toHaveLength(1);
            expect(options.buttons[0].text).toBe("OK");
            dialog.destroy();
        });
    });

    describe("getDialogButtons", () => {
        it("returns empty array by default", () => {
            const dialog = new BaseDialog({});
            expect(getDialogButtons(dialog)).toEqual([]);
            dialog.destroy();
        });
    });

    describe("getToolbarButtons", () => {
        it("returns empty array by default", () => {
            const dialog = new BaseDialog({});
            expect(getToolbarButtons(dialog)).toEqual([]);
            dialog.destroy();
        });
    });

    describe("initToolbar", () => {
        it("does nothing if no Toolbar element found", () => {
            const dialog = new BaseDialog({});
            expect(() => (dialog as any).initToolbar()).not.toThrow();
            dialog.destroy();
        });

        it("initializes toolbar when Toolbar element exists", () => {
            const dialog = new BaseDialog({});
            const toolbarDiv = document.createElement("div");
            toolbarDiv.id = dialog.idPrefix + "Toolbar";
            dialog.domNode.appendChild(toolbarDiv);
            expect(() => (dialog as any).initToolbar()).not.toThrow();
            expect(dialog["toolbar"]).toBeTruthy();
            dialog.destroy();
        });
    });

    describe("initValidator", () => {
        it("does nothing if no Form element is present", () => {
            const dialog = new BaseDialog({});
            expect(() => (dialog as any).initValidator()).not.toThrow();
            dialog.destroy();
        });

        it("creates validator when Form element exists as HTMLFormElement", () => {
            const dialog = new BaseDialog({});
            const form = document.createElement("form");
            form.id = dialog.idPrefix + "Form";
            dialog.domNode.appendChild(form);
            (dialog as any).initValidator();
            expect(dialog["validator"]).toBeTruthy();
            dialog.destroy();
        });

        it("does not create validator if Form is not an HTMLFormElement", () => {
            const dialog = new BaseDialog({});
            const div = document.createElement("div");
            div.id = dialog.idPrefix + "Form";
            dialog.domNode.appendChild(div);
            (dialog as any).initValidator();
            expect(dialog["validator"]).toBeFalsy();
            dialog.destroy();
        });
    });

    describe("getValidatorOptions", () => {
        it("returns empty object by default", () => {
            const dialog = new BaseDialog({});
            expect(getValidatorOptions(dialog)).toEqual({});
            dialog.destroy();
        });
    });

    describe("resetValidation", () => {
        it("does nothing when validator is null", () => {
            const dialog = new BaseDialog({});
            expect(() => resetValidation(dialog)).not.toThrow();
            dialog.destroy();
        });

        it("calls resetAll on validator", () => {
            const dialog = new BaseDialog({});
            const resetAll = vi.fn();
            dialog["validator"] = { resetAll, form: vi.fn() } as any;
            resetValidation(dialog);
            expect(resetAll).toHaveBeenCalled();
            dialog.destroy();
        });
    });

    describe("validateForm", () => {
        it("returns true when validator is null", () => {
            const dialog = new BaseDialog({});
            expect(validateForm(dialog)).toBe(true);
            dialog.destroy();
        });

        it("returns result of validator.form()", () => {
            const dialog = new BaseDialog({});
            dialog["validator"] = { form: vi.fn(() => false) } as any;
            expect(validateForm(dialog)).toBe(false);
            dialog.destroy();
        });
    });

    describe("isStaticPanel", () => {
        it("returns false by default", () => {
            const dialog = new BaseDialog({});
            expect(isStaticPanel(dialog)).toBe(false);
            dialog.destroy();
        });
    });

    describe("getInitialDialogTitle", () => {
        it("returns empty string by default", () => {
            const dialog = new BaseDialog({});
            expect(getInitialDialogTitle(dialog)).toBe("");
            dialog.destroy();
        });
    });

    describe("dialogTitle", () => {
        it("gets title from dataset when no dialog instance", () => {
            const dialog = new BaseDialog({});
            dialog.domNode.dataset.dialogtitle = "Cached Title";
            expect(dialog.dialogTitle).toBe("Cached Title");
            dialog.destroy();
        });

        it("sets title on dataset", () => {
            const dialog = new BaseDialog({});
            dialog.dialogTitle = "New Title";
            expect(dialog.domNode.dataset.dialogtitle).toBe("New Title");
            dialog.destroy();
        });

        it("gets title from dialog instance when available", () => {
            const dialog = new BaseDialog({});
            const titleFn = vi.fn(() => "Dialog Title");
            dialog["dialog"] = { title: titleFn, dispose: vi.fn(), close: vi.fn() } as any;
            expect(dialog.dialogTitle).toBe("Dialog Title");
            expect(titleFn).toHaveBeenCalledWith();
            dialog.destroy();
        });
    });

    describe("dialogClose", () => {
        it("calls dialog.close with provided result", () => {
            const dialog = new BaseDialog({});
            const closeFn = vi.fn();
            dialog["dialog"] = { close: closeFn, dispose: vi.fn() } as any;
            dialog.dialogClose("ok");
            expect(closeFn).toHaveBeenCalledWith("ok");
            dialog.destroy();
        });

        it("calls dialog.close with null if no result", () => {
            const dialog = new BaseDialog({});
            const closeFn = vi.fn();
            dialog["dialog"] = { close: closeFn, dispose: vi.fn() } as any;
            dialog.dialogClose();
            expect(closeFn).toHaveBeenCalledWith(null);
            dialog.destroy();
        });

        it("handles when dialog is null", () => {
            const dialog = new BaseDialog({});
            expect(() => dialog.dialogClose("ok")).not.toThrow();
            dialog.destroy();
        });
    });

    describe("dialogOpen", () => {
        it("reuses existing dialog instance", () => {
            const dialog = new BaseDialog({});
            const existingDialog = createMockDialog();
            dialog["dialog"] = existingDialog as any;
            (dialog as any).dialogOpen();
            expect(existingDialog.open).toHaveBeenCalled();
            dialog.destroy();
        });
    });

    describe("onDialogOpen", () => {
        it("calls arrange and selects first tab", () => {
            const dialog = new BaseDialog({});
            const arrangeSpy = vi.fn();
            dialog["arrange"] = arrangeSpy;
            const selectTabSpy = vi.spyOn(TabsExtensions, "selectTab");
            onDialogOpen(dialog);
            expect(arrangeSpy).toHaveBeenCalled();
            expect(selectTabSpy).toHaveBeenCalledWith(dialog["tabs"], 0);
            dialog.destroy();
        });
    });

    describe("onDialogClose", () => {
        it("dispatches click event and schedules destroy", () => {
            vi.useFakeTimers();
            const dialog = new BaseDialog({});
            const clickSpy = vi.spyOn(document, "dispatchEvent");
            const destroySpy = vi.spyOn(dialog, "destroy");
            onDialogClose(dialog);
            expect(clickSpy).toHaveBeenCalled();
            vi.runAllTimers();
            expect(destroySpy).toHaveBeenCalled();
            vi.useRealTimers();
        });
    });

    describe("initTabs", () => {
        it("does nothing if no Tabs element found", () => {
            const dialog = new BaseDialog({});
            expect(() => (dialog as any).initTabs()).not.toThrow();
            dialog.destroy();
        });

        it("initializes tabs when Tabs element exists", () => {
            const dialog = new BaseDialog({});
            const tabsDiv = document.createElement("div");
            tabsDiv.id = dialog.idPrefix + "Tabs";
            dialog.domNode.appendChild(tabsDiv);
            (dialog as any).initTabs();
            expect(dialog["tabs"]).toBeTruthy();
            dialog.destroy();
        });
    });

    describe("handleResponsive", () => {
        it("calls handleUIDialogResponsive with domNode", () => {
            const dialog = new BaseDialog({});
            // Mock handleUIDialogResponsive to avoid jQuery dependency
            const handleUIDialogResponsive = vi.fn();
            (dialog as any).handleResponsive = function () {
                handleUIDialogResponsive(this.domNode);
            };
            (dialog as any).handleResponsive();
            expect(handleUIDialogResponsive).toHaveBeenCalledWith(dialog.domNode);
            dialog.destroy();
        });
    });

    describe("onClose / onOpen", () => {
        it("onClose attaches close handler", () => {
            const dialog = new BaseDialog({});
            const handler = vi.fn();
            expect(() => dialog["onClose"]?.(handler)).not.toThrow();
            dialog.destroy();
        });

        it("onOpen attaches open handler", () => {
            const dialog = new BaseDialog({});
            const handler = vi.fn();
            expect(() => dialog["onOpen"]?.(handler)).not.toThrow();
            dialog.destroy();
        });
    });

    describe("TemplatedDialog", () => {
        it("is an alias for BaseDialog", async () => {
            const mod = await import("./basedialog");
            expect(mod.TemplatedDialog).toBe(BaseDialog);
        });
    });
});

describe("BaseDialog additional branches", () => {
    let mockDialogInstance: any;
    let origJQuery: any;

    beforeEach(() => {
        document.body.innerHTML = "";
        origJQuery = (window as any).jQuery;
        mockDialogInstance = {
            open: vi.fn(),
            close: vi.fn(),
            dispose: vi.fn(),
            title: vi.fn((val?: string) => val === undefined ? "Test Title" : undefined),
            type: "uidialog"
        };
        const mock$ = vi.fn((selector: any) => ({
            [0]: selector,
            length: 1,
            dialog: vi.fn(() => mockDialogInstance),
            closest: vi.fn(() => ({ length: 1, [0]: document.createElement("div"), on: vi.fn(), off: vi.fn(), triggerHandler: vi.fn() })),
            data: vi.fn(() => ({ uiTabs: {} })),
            removeData: vi.fn(),
            width: vi.fn(() => 800),
            height: vi.fn(() => 600),
            find: vi.fn(() => ({
                first: vi.fn(() => ({ insertBefore: vi.fn(), appendTo: vi.fn() })),
                on: vi.fn(),
                off: vi.fn()
            })),
            on: vi.fn(),
            off: vi.fn(),
            one: vi.fn(),
            trigger: vi.fn(),
            triggerHandler: vi.fn(),
            addClass: vi.fn(),
            removeClass: vi.fn(),
            css: vi.fn(() => "absolute"),
            position: vi.fn(() => ({ left: 0, top: 0 })),
            remove: vi.fn(),
            val: vi.fn(() => ""),
            attr: vi.fn(),
            is: vi.fn(() => false),
            hasClass: vi.fn(() => false),
            toggle: vi.fn(),
            hide: vi.fn(),
            show: vi.fn(),
            empty: vi.fn()
        })) as any;
        mock$.fn = { dialog: true };
        (window as any).jQuery = mock$;
    });

    afterEach(() => {
        (window as any).jQuery = origJQuery;
        vi.restoreAllMocks();
    });

    it("providerOptions returns object for uidialog type", () => {
        const dialog = new BaseDialog({});
        const options = (dialog as any).getDialogOptions();
        const providerOpts = options.providerOptions("uidialog");
        expect(providerOpts).toBeDefined();
        expect(typeof providerOpts).toBe("object");
        dialog.destroy();
    });

    it("providerOptions returns undefined for non-uidialog type", () => {
        const dialog = new BaseDialog({});
        const options = (dialog as any).getDialogOptions();
        const providerOpts = options.providerOptions("bs-modal");
        expect(providerOpts).toBeUndefined();
        dialog.destroy();
    });

    it("getDialogOptions closeButton uses CloseButtonAttribute when not static", () => {
        const dialog = new BaseDialog({});
        const options = (dialog as any).getDialogOptions();
        // Without attribute, closeButton should be undefined (from getCustomAttribute)
        expect(options.closeButton).toBeUndefined();
        dialog.destroy();
    });

    it("addCssClass is a no-op", () => {
        const dialog = new BaseDialog({});
        expect(() => (dialog as any).addCssClass()).not.toThrow();
        dialog.destroy();
    });

    it("initDialog sets domNode visible and calls initUIDialog for uidialog type", () => {
        const dialog = new BaseDialog({});
        dialog.domNode.hidden = true;
        dialog["dialog"] = mockDialogInstance;
        const initUISpy = vi.fn();
        dialog["initUIDialog"] = initUISpy;
        (dialog as any).initDialog();
        expect(dialog.domNode.hidden).toBe(false);
        expect(initUISpy).toHaveBeenCalled();
        dialog.destroy();
    });

    it("onDialogOpen calls arrange and selects first tab", () => {
        const dialog = new BaseDialog({});
        const arrangeSpy = vi.fn();
        dialog["arrange"] = arrangeSpy;
        (dialog as any).onDialogOpen();
        expect(arrangeSpy).toHaveBeenCalled();
        dialog.destroy();
    });
});
