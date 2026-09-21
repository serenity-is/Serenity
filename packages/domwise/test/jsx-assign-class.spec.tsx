import { beforeEach, describe, expect, it, vi } from "vitest";
import { invokeDisposingListeners } from "../src/disposing-listener";
import { assignClass } from "../src/jsx-assign-class";
import { signal } from "../src/signals";
import { mockSignal } from "./mocks/mock-signal";

let element: HTMLElement;

beforeEach(() => {
    element = document.createElement("div");
    vi.clearAllMocks();
});

describe("assignClass", () => {
    it("applies and replaces classes when prev is provided", () => {
        assignClass(element, "foo bar");
        expect(element.classList.contains("foo")).toBe(true);
        expect(element.classList.contains("bar")).toBe(true);

        assignClass(element, "baz", "foo bar");
        expect(element.classList.contains("baz")).toBe(true);
        expect(element.classList.contains("foo")).toBe(false);
        expect(element.classList.contains("bar")).toBe(false);
    });

    it("clears previous classes when value is null or false", () => {
        assignClass(element, "foo bar");
        expect(element.classList.contains("foo")).toBe(true);

        assignClass(element, null, "foo bar");
        expect(element.classList.contains("foo")).toBe(false);
        expect(element.classList.contains("bar")).toBe(false);

        assignClass(element, false, "foo bar");
        expect(element.classList.contains("foo")).toBe(false);
        expect(element.classList.contains("bar")).toBe(false);
    });

    it("removes classes based on prev signal values", () => {
        const prevSignal = mockSignal("foo bar");
        assignClass(element, "baz", prevSignal);

        expect(element.classList.contains("baz")).toBe(true);
        expect(element.classList.contains("foo")).toBe(false);
        expect(element.classList.contains("bar")).toBe(false);
    });

    it("supports arrays with signal items and updates when they change", () => {
        const sig = mockSignal("a");
        assignClass(element, [sig]);

        expect(element.classList.contains("a")).toBe(true);

        sig.value = "b";
        expect(element.classList.contains("b")).toBe(true);
        expect(element.classList.contains("a")).toBe(false);
    });

    it("supports object class mapping with signal values", () => {
        const sig = mockSignal(true);
        assignClass(element, { foo: sig, bar: false });

        expect(element.classList.contains("foo")).toBe(true);
        expect(element.classList.contains("bar")).toBe(false);

        sig.value = false;
        expect(element.classList.contains("foo")).toBe(false);

        sig.value = true;
        expect(element.classList.contains("foo")).toBe(true);
    });

    it("stops applying class signal after class is replaced", () => {
        const active = mockSignal(false);
        assignClass(element, { active });
        expect(element.classList.contains("active")).toBe(false);

        assignClass(element, "other", { active });
        expect(element.classList.contains("other")).toBe(true);

        active.value = true;
        expect(element.classList.contains("active")).toBe(false);
    });

    it("disposes per-key class signals when element is disposed", () => {
        const active = mockSignal(true);
        assignClass(element, { active });
        expect(active.listeners).toHaveLength(1);
        invokeDisposingListeners(element, { descendants: true });
        expect(active.listeners).toHaveLength(0);
    });

    it("clears previous classes when value is null, false, or true", () => {
        element.className = "test foo bar";
        assignClass(element, null, "foo bar");
        expect(element.classList.contains("foo")).toBe(false);
        expect(element.classList.contains("bar")).toBe(false);
        expect(element.classList.contains("test")).toBe(true);

        element.className = "test foo bar";
        assignClass(element, false, "foo bar");
        expect(element.classList.contains("foo")).toBe(false);
        expect(element.classList.contains("bar")).toBe(false);
        expect(element.classList.contains("test")).toBe(true);

        element.className = "test foo bar";
        assignClass(element, true, "foo bar");
        expect(element.classList.contains("foo")).toBe(false);
        expect(element.classList.contains("bar")).toBe(false);
        expect(element.classList.contains("test")).toBe(true);
    });

    it("does nothing when value is null, false, or true and prev is null, false, or true", () => {
        element.className = "test";
        assignClass(element, null, null);
        expect(element.classList.contains("test")).toBe(true);

        assignClass(element, false, true);
        expect(element.classList.contains("test")).toBe(true);

        assignClass(element, true, false);
        expect(element.classList.contains("test")).toBe(true);
    });

    it("handles deeply nested arrays with signals in prev", () => {
        const innerSig = mockSignal("x");
        // prev with nested array containing a signal
        assignClass(element, ["b"], [["a", innerSig]]);
        // Nested signal should be unsignalized — "a" and the peeked "x" should be removed
        expect(element.classList.contains("a")).toBe(false);
        expect(element.classList.contains("x")).toBe(false);
        expect(element.classList.contains("b")).toBe(true);
    });

    it("accepts a non-array iterable", () => {
        assignClass(element, new Set(["a", "b"]));
        expect(element.classList.contains("a")).toBe(true);
        expect(element.classList.contains("b")).toBe(true);
    });

    it("splits whitespace-separated class strings", () => {
        assignClass(element, "btn\n  btn-primary");
        expect(element.classList.contains("btn")).toBe(true);
        expect(element.classList.contains("btn-primary")).toBe(true);
    });

    it("compiles an array containing a signal entry", () => {
        const cls = <div class={["a", signal(false)]} />;
        expect(cls).toBeDefined();
    });
});
