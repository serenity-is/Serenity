import { BasePanel, TemplatedPanel } from "./basepanel";
import { Toolbar } from "./toolbar";

describe("BasePanel", () => {
    it("creates without form, tabs, or toolbar elements", () => {
        const panel = new BasePanel({}).init();
        expect(panel.domNode).toBeInstanceOf(HTMLElement);
        expect(panel.domNode.tagName).toBe("DIV");
        panel.destroy();
    });

    it("initializes validator when Form element exists", () => {
        const panel = new BasePanel({}).init();
        // Without a Form element, validator should not be initialized
        expect((panel as any).validator).toBeFalsy();
        panel.destroy();
    });

    it("initializes validator with Form element", () => {
        const panelEl = document.createElement("div");
        const form = document.createElement("form");
        form.id = "test_Form";
        panelEl.appendChild(form);

        const panel = new BasePanel({ element: panelEl, idPrefix: "test_" } as any).init();
        expect((panel as any).validator).toBeTruthy();
        panel.destroy();
    });

    it("initializes tabs when Tabs element exists", () => {
        const panelEl = document.createElement("div");
        const tabs = document.createElement("div");
        tabs.id = "test_Tabs";
        panelEl.appendChild(tabs);

        const panel = new BasePanel({ element: panelEl, idPrefix: "test_" } as any).init();
        expect((panel as any).tabs).toBeTruthy();
        panel.destroy();
    });

    it("initializes toolbar when Toolbar element exists", () => {
        const panelEl = document.createElement("div");
        const toolbar = document.createElement("div");
        toolbar.id = "test_Toolbar";
        panelEl.appendChild(toolbar);

        const panel = new BasePanel({ element: panelEl, idPrefix: "test_" } as any).init();
        expect((panel as any).toolbar).toBeTruthy();
        expect((panel as any).toolbar).toBeInstanceOf(Toolbar);
        panel.destroy();
    });

    it("destroy cleans up tabs, toolbar, validator", () => {
        const panelEl = document.createElement("div");
        const form = document.createElement("form");
        form.id = "test_Form";
        const toolbar = document.createElement("div");
        toolbar.id = "test_Toolbar";
        const tabs = document.createElement("div");
        tabs.id = "test_Tabs";
        panelEl.appendChild(form);
        panelEl.appendChild(toolbar);
        panelEl.appendChild(tabs);

        const panel = new BasePanel({ element: panelEl, idPrefix: "test_" } as any).init();
        panel.destroy();

        expect((panel as any).tabs).toBeNull();
        expect((panel as any).toolbar).toBeNull();
        expect((panel as any).validator).toBeNull();
    });

    it("arrange triggers layout on visible .require-layout elements", () => {
        const panel = new BasePanel({}).init();
        const layoutEl = document.createElement("div");
        layoutEl.classList.add("require-layout");
        // Make the element appear visible by mocking getClientRects
        layoutEl.getClientRects = () => [{ top: 0, left: 0, right: 100, bottom: 100, width: 100, height: 100 } as any] as any;
        panel.domNode.appendChild(layoutEl);
        document.body.appendChild(panel.domNode);

        const handler = vi.fn();
        layoutEl.addEventListener("layout", handler);

        panel.arrange();
        expect(handler).toHaveBeenCalled();

        document.body.removeChild(panel.domNode);
        panel.destroy();
    });

    it("getToolbarButtons returns empty array", () => {
        const panel = new BasePanel({}).init();
        expect(panel["getToolbarButtons"]()).toEqual([]);
        panel.destroy();
    });

    it("getValidatorOptions returns empty object", () => {
        const panel = new BasePanel({}).init();
        expect(panel["getValidatorOptions"]()).toEqual({});
        panel.destroy();
    });

    it("resetValidation does nothing when no validator", () => {
        const panel = new BasePanel({}).init();
        expect(() => panel["resetValidation"]()).not.toThrow();
        panel.destroy();
    });

    it("resetValidation calls validator.resetAll", () => {
        const panelEl = document.createElement("div");
        const form = document.createElement("form");
        form.id = "test_Form";
        panelEl.appendChild(form);

        const panel = new BasePanel({ element: panelEl, idPrefix: "test_" } as any).init();
        const validator = (panel as any).validator;
        expect(validator).toBeTruthy();
        vi.spyOn(validator, "resetAll");

        panel["resetValidation"]();
        expect(validator.resetAll).toHaveBeenCalled();
        panel.destroy();
    });

    it("validateForm returns true when no validator", () => {
        const panel = new BasePanel({}).init();
        expect(panel["validateForm"]()).toBe(true);
        panel.destroy();
    });

    it("TemplatedPanel is an alias for BasePanel", () => {
        expect(TemplatedPanel).toBe(BasePanel);
    });
});
