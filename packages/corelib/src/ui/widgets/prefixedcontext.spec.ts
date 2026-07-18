import { PrefixedContext } from "./prefixedcontext";
import { Widget } from "./widget";

describe("PrefixedContext", () => {
    it("stores the idPrefix", () => {
        const ctx = new PrefixedContext("my_");
        expect(ctx.idPrefix).toBe("my_");
    });

    it("stores empty idPrefix", () => {
        const ctx = new PrefixedContext("");
        expect(ctx.idPrefix).toBe("");
    });

    describe("byId", () => {
        it("returns Fluent for element with prefixed id", () => {
            const div = document.createElement("div");
            div.id = "my_someId";
            document.body.appendChild(div);

            const ctx = new PrefixedContext("my_");
            const result = ctx.byId("someId");
            expect(result).toBeTruthy();
            expect(result.getNode()).toBe(div);

            document.body.removeChild(div);
        });

        it("returns Fluent even if element does not exist", () => {
            const ctx = new PrefixedContext("my_");
            const result = ctx.byId("nonexistent");
            expect(result).toBeTruthy();
            expect(result.getNode()).toBeNull();
        });
    });

    describe("w", () => {
        it("throws when no widget is associated with element", () => {
            const div = document.createElement("div");
            div.id = "my_widgetId";
            document.body.appendChild(div);

            const ctx = new PrefixedContext("my_");
            expect(() => ctx.w("widgetId", Widget)).toThrow();

            document.body.removeChild(div);
        });

        it("returns widget when element has associated widget", () => {
            const div = document.createElement("div");
            div.id = "my_testWidget";
            document.body.appendChild(div);

            class TestWidget extends Widget {}
            const widget = new TestWidget({ element: div });

            const ctx = new PrefixedContext("my_");
            const result = ctx.w<TestWidget>("testWidget", TestWidget);
            expect(result).toBe(widget);

            widget.destroy();
            document.body.removeChild(div);
        });
    });
});
