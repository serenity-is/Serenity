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

    it("stores idPrefix and context from object", () => {
        const div = document.createElement("div");
        const ctx = new PrefixedContext({ idPrefix: "my_", domNode: div });
        expect(ctx.idPrefix).toBe("my_");
        expect(ctx.context).toBe(div);
    });

    it("stores explicit context when provided with string", () => {
        const div = document.createElement("div");
        const ctx = new PrefixedContext("my_", div);
        expect(ctx.idPrefix).toBe("my_");
        expect(ctx.context).toBe(div);
    });

    it("explicit context overrides domNode when using object", () => {
        const domNode = document.createElement("div");
        const contextDiv = document.createElement("div");
        const ctx = new PrefixedContext({ idPrefix: "my_", domNode }, contextDiv);
        expect(ctx.idPrefix).toBe("my_");
        expect(ctx.context).toBe(contextDiv);
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

        it("queries within context domNode when using object constructor", () => {
            const container = document.createElement("div");
            const inner = document.createElement("div");
            inner.id = "my_someId";
            container.appendChild(inner);
            document.body.appendChild(container);

            const ctx = new PrefixedContext({ idPrefix: "my_", domNode: container });
            const result = ctx.byId("someId");
            expect(result).toBeTruthy();
            expect(result.getNode()).toBe(inner);

            document.body.removeChild(container);
        });

        it("queries within explicit context when provided with string", () => {
            const container = document.createElement("div");
            const inner = document.createElement("div");
            inner.id = "my_someId";
            container.appendChild(inner);
            document.body.appendChild(container);

            const ctx = new PrefixedContext("my_", container);
            const result = ctx.byId("someId");
            expect(result).toBeTruthy();
            expect(result.getNode()).toBe(inner);

            document.body.removeChild(container);
        });

        it("does not find element outside context", () => {
            const container = document.createElement("div");
            const outside = document.createElement("div");
            outside.id = "my_outsideId";
            document.body.appendChild(outside);
            document.body.appendChild(container);

            const ctx = new PrefixedContext({ idPrefix: "my_", domNode: container });
            const result = ctx.byId("outsideId");
            expect(result).toBeTruthy();
            expect(result.getNode()).toBeNull();

            document.body.removeChild(outside);
            document.body.removeChild(container);
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

        it("uses domNode as context when using object constructor", () => {
            const container = document.createElement("div");
            const inner = document.createElement("div");
            inner.id = "my_testWidget";
            container.appendChild(inner);
            document.body.appendChild(container);

            class TestWidget extends Widget {}
            const widget = new TestWidget({ element: inner });

            const ctx = new PrefixedContext({ idPrefix: "my_", domNode: container });
            const result = ctx.w<TestWidget>("testWidget", TestWidget);
            expect(result).toBe(widget);

            widget.destroy();
            document.body.removeChild(container);
        });

        it("uses explicit context when provided with string", () => {
            const container = document.createElement("div");
            const inner = document.createElement("div");
            inner.id = "my_testWidget";
            container.appendChild(inner);
            document.body.appendChild(container);

            class TestWidget extends Widget {}
            const widget = new TestWidget({ element: inner });

            const ctx = new PrefixedContext("my_", container);
            const result = ctx.w<TestWidget>("testWidget", TestWidget);
            expect(result).toBe(widget);

            widget.destroy();
            document.body.removeChild(container);
        });
    });
});
