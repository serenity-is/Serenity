import { describe, expect, it } from "vitest";
import { invokeDisposingListeners } from "../src/disposing-listener";
import { assignStyle } from "../src/jsx-assign-style";
import { derivedSignal } from "../src/signal-util";
import { signal } from "../src/signals";

describe("assignStyle", () => {
    it("merges style arrays left-to-right", () => {
        const el = <div style={[{ color: "red", fontSize: "12px" }, { fontSize: "14px" }]} /> as HTMLElement;
        expect(el.style.color).toBe("red");
        expect(el.style.fontSize).toBe("14px");
    });

    it("accepts numbers and CSS custom properties", () => {
        const el = <div style={{ opacity: 0.5, "--my-color": "blue" }} /> as HTMLElement;
        expect(el.style.opacity).toBe("0.5");
        expect(el.style.getPropertyValue("--my-color")).toBe("blue");
    });

    it("appends px only to non-unitless numeric properties", () => {
        const el = <div style={{ flexShrink: 0, flexGrow: 1, zIndex: 10, width: 100 }} /> as HTMLElement;
        expect(el.style.flexShrink).toBe("0");
        expect(el.style.flexGrow).toBe("1");
        expect(el.style.zIndex).toBe("10");
        expect(el.style.width).toBe("100px");
    });

    it("accepts a signal-valued number", () => {
        const size = signal(16);
        const el = <div style={{ fontSize: size }} /> as HTMLElement;
        expect(el.style.fontSize).toBe("16px");
        size.value = 20;
        expect(el.style.fontSize).toBe("20px");
    });

    it("compiles numeric, array and signal style values", () => {
        const numeric = <div style={{ flexShrink: 0, flexGrow: 1, zIndex: 10, width: 100, opacity: 0.5 }} />;
        expect(numeric).toBeDefined();

        const array = <div style={[{ flexShrink: 0 }, { flexGrow: 1 }]} />;
        expect(array).toBeDefined();

        const signals = <div style={{ flexShrink: signal(0), "--gap": signal(4) }} />;
        expect(signals).toBeDefined();
    });

    it("does not dispose a derived signal when the style is reassigned", () => {
        const el = document.createElement("div");
        const source = signal("red");
        const derived = derivedSignal(source, value => value);
        assignStyle(el, { color: derived });
        assignStyle(el, { color: derived, fontSize: "10px" }, { color: derived });

        source.value = "blue";
        expect(el.style.color).toBe("blue");
        expect(typeof (derived as any).derivedDisposer).toBe("function");
    });

    it("binds a style value assigned after the node was disposed", () => {
        const el = document.createElement("div");
        const first = signal("red");
        assignStyle(el, { color: first });
        invokeDisposingListeners(el);
        expect(el.style.color).toBe("red");

        const second = signal("green");
        assignStyle(el, { color: second });
        expect(el.style.color).toBe("green");

        invokeDisposingListeners(el);
        second.value = "blue";
        expect(el.style.color).toBe("green");
    });
});
