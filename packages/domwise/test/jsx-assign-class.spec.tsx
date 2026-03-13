import { assignClass } from "../src/jsx-assign-class";
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
});