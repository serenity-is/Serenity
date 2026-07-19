import { Widget } from "../widgets/widget";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator } from "./filteroperator";

describe("BaseEditorFiltering", () => {
    class TestWidget extends Widget<any> {
        static override [Symbol.typeInfo] = this.registerClass("Serenity.TestEditor");
        private _value = "testValue";
        get value() { return this._value; }
        set value(v: string) { this._value = v; }
        getEditValue(_, target: any) { target["_"] = this._value; }
    }

    class TestBaseEditorFiltering extends BaseEditorFiltering<TestWidget> {
        getOperators(): FilterOperator[] { return []; }
    }

    it("is registered with typeInfo", () => {
        expect(BaseEditorFiltering[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    describe("useEditor", () => {
        it("returns true for eq, ne, lt, le, gt, ge operators", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test" });
            ["eq", "ne", "lt", "le", "gt", "ge"].forEach(opKey => {
                filtering.set_operator({ key: opKey });
                expect(filtering["useEditor"]()).toBe(true);
            });
        });

        it("returns false for non-comparison operators", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test" });
            ["contains", "startswith", "isnull", "isnotnull", "true", "false"].forEach(opKey => {
                filtering.set_operator({ key: opKey });
                expect(filtering["useEditor"]()).toBe(false);
            });
        });
    });

    describe("createEditor", () => {
        it("creates editor widget when useEditor returns true", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "eq" });
            filtering.createEditor();

            expect((filtering as any).editor).toBeInstanceOf(TestWidget);
            expect(container.children.length).toBeGreaterThan(0);
        });

        it("sets editor to null and calls base when useEditor returns false", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "contains" });
            filtering.createEditor();

            expect((filtering as any).editor).toBeNull();
            expect(container.querySelector("input")).toBeTruthy();
        });

        it("throws for unknown operator when not using editor", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "custom_op" });
            expect(() => filtering.createEditor()).toThrow();
        });
    });

    describe("useIdField", () => {
        it("returns false by default", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            expect(filtering["useIdField"]()).toBe(false);
        });
    });

    describe("getCriteriaField", () => {
        it("returns filteringIdField when useEditor, useIdField and filteringIdField set", () => {
            const filtering = new (class extends BaseEditorFiltering<TestWidget> {
                getOperators(): FilterOperator[] { return []; }
                protected override useIdField() { return true; }
            })(TestWidget);
            filtering.set_field({ name: "TestField", filteringIdField: "TestId" });
            filtering.set_operator({ key: "eq" });
            expect(filtering["getCriteriaField"]()).toBe("TestId");
        });

        it("returns field name when useEditor returns false", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "TestField", filteringIdField: "TestId" });
            filtering.set_operator({ key: "contains" });
            expect(filtering["getCriteriaField"]()).toBe("TestField");
        });

        it("returns field name when filteringIdField is not set", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "TestField" });
            filtering.set_operator({ key: "eq" });
            expect(filtering["getCriteriaField"]()).toBe("TestField");
        });
    });

    describe("getEditorOptions", () => {
        it("returns empty object when no params", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test" });
            filtering.set_operator({ key: "eq" });
            const opts = filtering.getEditorOptions();
            expect(opts).toBeTruthy();
        });

        it("removes cascadeFrom from editorParams", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test", editorParams: { cascadeFrom: "Parent", maxLength: 100 } });
            filtering.set_operator({ key: "eq" });
            const opts = filtering.getEditorOptions();
            expect(opts.cascadeFrom).toBeUndefined();
            expect(opts.maxLength).toBe(100);
        });

        it("merges filteringParams", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test", filteringParams: { param1: "value1" } });
            filtering.set_operator({ key: "eq" });
            const opts = filtering.getEditorOptions();
            expect(opts.param1).toBe("value1");
        });
    });

    describe("loadState", () => {
        it("loads state into editor when useEditor is true", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "eq" });
            filtering.createEditor();

            filtering.loadState("newState");
            expect((filtering as any).editor).toBeTruthy();
        });

        it("does nothing when state is null and useEditor is true", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "eq" });
            filtering.createEditor();

            expect(() => filtering.loadState(null)).not.toThrow();
        });

        it("calls base loadState when not using editor", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "contains" });
            filtering.createEditor();

            filtering.loadState("stateValue");
            expect(container.querySelector("input").value).toBe("stateValue");
        });
    });

    describe("saveState", () => {
        it("returns editor value when useEditor is true", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test" });
            filtering.set_container(document.createElement("div"));
            filtering.set_operator({ key: "eq" });
            filtering.createEditor();

            const state = filtering.saveState();
            expect(state).toBeTruthy();
        });

        it("returns base saveState when not using editor", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "contains" });
            filtering.createEditor();
            container.querySelector("input").value = "baseValue";

            expect(filtering.saveState()).toBe("baseValue");
        });
    });

    describe("getEditorValue", () => {
        it("returns trimmed value from editor when useEditor is true", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test" });
            filtering.set_container(document.createElement("div"));
            filtering.set_operator({ key: "eq" });
            filtering.createEditor();

            const value = filtering.getEditorValue();
            expect(value).toBe("testValue");
        });

        it("throws if editor value is null", () => {
            class NullWidget extends Widget<any> {
                get value() { return null; }
                set value(v: string) { }
            }
            const filtering = new (class extends BaseEditorFiltering<NullWidget> {
                getOperators(): FilterOperator[] { return []; }
            })(NullWidget);
            filtering.set_field({ name: "Test" });
            filtering.set_container(document.createElement("div"));
            filtering.set_operator({ key: "eq" });
            filtering.createEditor();

            expect(() => filtering.getEditorValue()).toThrow();
        });

        it("calls base getEditorValue when not using editor", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test" });
            const container = document.createElement("div");
            filtering.set_container(container);
            filtering.set_operator({ key: "contains" });
            filtering.createEditor();
            container.querySelector("input").value = "baseVal";

            expect(filtering.getEditorValue()).toBe("baseVal");
        });
    });

    describe("initQuickFilter", () => {
        it("sets filter type to editorTypeRef", () => {
            const filtering = new TestBaseEditorFiltering(TestWidget);
            filtering.set_field({ name: "Test", title: "Test Field", filteringParams: { p: 1 }, quickFilterParams: { qp: 2 } });
            filtering.set_operator({ key: "eq" });
            const filter: any = { options: {} };
            filtering.initQuickFilter(filter);
            expect(filter.type).toBe(TestWidget);
            expect(filter.field).toBe("Test");
        });
    });
});
