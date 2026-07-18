import { setScriptData } from "../../base";
import { mockFetch, unmockFetch } from "../../test/mocks";
import { PropertyGrid } from "./propertygrid";
import { PropertyPanel } from "./propertypanel";

// Register form data for the base PropertyPanel class which has formKey = ""
// (because its typeFullName is "Serenity." from nsSerenity)
beforeEach(() => {
    setScriptData("Form.", {
        items: [
            { name: "DummyField", editorType: "String" }
        ]
    });
    // Also mock any XHR/fetch requests for form data as a fallback
    mockFetch({
        "*": () => ({ items: [] })
    });
});

afterEach(() => {
    unmockFetch();
});

describe("PropertyPanel", () => {
    it("creates without PropertyGrid element", () => {
        const panel = new PropertyPanel({}).init();
        expect(panel.domNode).toBeInstanceOf(HTMLElement);
        expect((panel as any).propertyGrid).toBeFalsy();
        panel.destroy();
    });

    it("initializes PropertyGrid when PropertyGrid element exists", () => {
        const panelEl = document.createElement("div");
        const pgDiv = document.createElement("div");
        pgDiv.id = "test_PropertyGrid";
        panelEl.appendChild(pgDiv);

        const panel = new PropertyPanel({ element: panelEl, idPrefix: "test_" } as any).init();
        expect((panel as any).propertyGrid).toBeTruthy();
        expect((panel as any).propertyGrid).toBeInstanceOf(PropertyGrid);
        panel.destroy();
    });

    it("loads initial entity after PropertyGrid init", () => {
        const panelEl = document.createElement("div");
        const pgDiv = document.createElement("div");
        pgDiv.id = "test_PropertyGrid";
        panelEl.appendChild(pgDiv);

        const panel = new PropertyPanel({ element: panelEl, idPrefix: "test_" } as any).init();
        expect((panel as any).propertyGrid).toBeTruthy();
        panel.destroy();
    });

    it("getFormKey returns type name with namespace stripped", () => {
        class TestPanel extends PropertyPanel<any, {}> {
            static override[Symbol.typeInfo] = this.registerClass("SomeNamespace.TestPanel");
        }
        const panel = new TestPanel({});
        const formKey = (panel as any).getFormKey();
        expect(formKey).toBe("TestPanel");
        panel.destroy();
    });

    it("getFormKey strips 'Panel' suffix when no namespace", () => {
        class SomePanel extends PropertyPanel<any, {}> {}
        const panel = new SomePanel({});
        const formKey = (panel as any).getFormKey();
        expect(formKey).toBe("Some");
        panel.destroy();
    });

    it("getPropertyGridOptions returns default options", () => {
        const panel = new PropertyPanel({});
        const options = (panel as any).getPropertyGridOptions();
        expect(options.idPrefix).toBe(panel.idPrefix);
        expect(options.mode).toBe(1); // PropertyGridMode.insert
        expect(options.localTextPrefix).toContain("Forms.");
        expect(Array.isArray(options.items)).toBe(true);
        panel.destroy();
    });

    it("getSaveEntity returns entity from propertyGrid", () => {
        const panelEl = document.createElement("div");
        const pgDiv = document.createElement("div");
        pgDiv.id = "test_PropertyGrid";
        panelEl.appendChild(pgDiv);

        const panel = new PropertyPanel({ element: panelEl, idPrefix: "test_" } as any).init();
        const entity = (panel as any).getSaveEntity();
        expect(entity).toBeTruthy();
        panel.destroy();
    });

    it("entity getter returns _entity", () => {
        const panel = new PropertyPanel({});
        expect(panel.entity).toBeUndefined();
        panel.destroy();
    });

    it("entityId getter returns _entityId", () => {
        const panel = new PropertyPanel({});
        expect(panel.entityId).toBeUndefined();
        panel.destroy();
    });

    it("destroy with Form element cleans up validator", () => {
        const panelEl = document.createElement("div");
        const form = document.createElement("form");
        form.id = "test_Form";
        panelEl.appendChild(form);

        const panel = new PropertyPanel({ element: panelEl, idPrefix: "test_" } as any).init();
        const validator = (panel as any).validator;
        expect(validator).toBeTruthy();
        panel.destroy();
        expect((panel as any).validator).toBeNull();
    });

    it("destroy cleans up propertyGrid and validator", () => {
        const panelEl = document.createElement("div");
        const pgDiv = document.createElement("div");
        pgDiv.id = "test_PropertyGrid";
        panelEl.appendChild(pgDiv);

        const panel = new PropertyPanel({ element: panelEl, idPrefix: "test_" } as any).init();
        panel.destroy();
        expect((panel as any).propertyGrid).toBeNull();
    });
});

describe("PropertyPanel - protected members", () => {
    class ExposedPanel extends PropertyPanel<any, {}> {
        public setEntity(value: any) { this.entity = value; }
        public setEntityId(value: any) { this.entityId = value; }
        public callValidateBeforeSave() { return this["validateBeforeSave"](); }
    }

    it("entity setter sets _entity", () => {
        const panel = new ExposedPanel({});
        panel.setEntity({ id: 1 });
        expect(panel.entity).toEqual({ id: 1 });
        panel.destroy();
    });

    it("entity setter defaults null to new Object", () => {
        const panel = new ExposedPanel({});
        panel.setEntity(null);
        expect(panel.entity).toBeTruthy();
        panel.destroy();
    });

    it("entityId setter sets _entityId", () => {
        const panel = new ExposedPanel({});
        panel.setEntityId(42);
        expect(panel.entityId).toBe(42);
        panel.destroy();
    });

    it("validateBeforeSave throws when no validator", () => {
        const panel = new ExposedPanel({});
        expect(() => panel.callValidateBeforeSave()).toThrow();
        panel.destroy();
    });
});
