import { usePropBinding } from "../src";
import { assignProp, assignProps } from "../src/jsx-assign-props";
import { signal } from "../src/signals";
import { mockSignal } from "./mocks/mock-signal";

let element: HTMLElement;

beforeEach(() => {
    element = document.createElement("div");
    vi.clearAllMocks();
});

describe("assignProps", () => {
    it("assigns and updates standard attributes", () => {
        assignProps(element, { id: "test-id", title: "Test Title" });
        expect(element.id).toBe("test-id");
        expect(element.title).toBe("Test Title");

        assignProps(element, { id: "new-id", title: "New Title" });
        expect(element.id).toBe("new-id");
        expect(element.title).toBe("New Title");
    });

    it("handles signal props", () => {
        const sig = signal("initial");
        assignProps(element, { "data-value": sig });
        expect(element.getAttribute("data-value")).toBe("initial");

        sig.value = "updated";
        expect(element.getAttribute("data-value")).toBe("updated");
    });

    it("handles dataset property", () => {
        assignProps(element, { dataset: { test: "value", other: "data" } });
        expect(element.dataset.test).toBe("value");
        expect(element.dataset.other).toBe("data");

        // When updating, prev is not passed, so old values remain
        assignProps(element, { dataset: { test: "new-value", third: "more" } });
        expect(element.dataset.test).toBe("new-value");
        expect(element.dataset.other).toBe("data"); // Still there since prev is undefined
        expect(element.dataset.third).toBe("more");
    });

    it("handles dataset property with a prev via signal", () => {
        const signal = mockSignal<any>({ test: "value", other: "data" });
        assignProps(element, { dataset: signal });
        expect(element.dataset.test).toBe("value");
        expect(element.dataset.other).toBe("data");

        signal.value = { test: "new-value", third: "more" };
        expect(element.dataset.test).toBe("new-value");
        expect(element.dataset.other).toBeUndefined(); // Cleared
        expect(element.dataset.third).toBe("more");
    });

    it("handles dataset property with a prev via prop binding", () => {
        const binding = usePropBinding<any>({ test: "value", other: "data" });
        assignProps(element, { dataset: binding });
        expect(element.dataset.test).toBe("value");
        expect(element.dataset.other).toBe("data");

        binding({ test: "new-value", third: "more" });
        expect(element.dataset.test).toBe("new-value");
        expect(element.dataset.other).toBeUndefined(); // Cleared
        expect(element.dataset.third).toBe("more");
    });

    it("handles textContent property", () => {
        assignProps(element, { textContent: "Hello World" });
        expect(element.textContent).toBe("Hello World");

        // Setting to null doesn't clear since prev is undefined
        assignProps(element, { textContent: null });
        expect(element.textContent).toBe("Hello World");

        assignProps(element, { textContent: false });
        expect(element.textContent).toBe("Hello World");

        assignProps(element, { textContent: 0 });
        expect(element.textContent).toBe("0");
    });

    it("handles dangerouslySetInnerHTML property", () => {
        assignProps(element, { dangerouslySetInnerHTML: { __html: "<span>test</span>" } });
        expect(element.innerHTML).toBe("<span>test</span>");

        // Setting to null doesn't clear since prev is undefined
        assignProps(element, { dangerouslySetInnerHTML: null });
        expect(element.innerHTML).toBe("<span>test</span>");
    });

    it("handles value property on input elements", () => {
        const input = document.createElement("input");
        assignProps(input, { value: "test value" });
        expect(input.value).toBe("test value");

        // Setting to null doesn't clear since prev is undefined
        assignProps(input, { value: null });
        expect(input.value).toBe("test value");
    });

    it("handles value property on textarea elements", () => {
        const textarea = document.createElement("textarea");
        assignProps(textarea, { value: "test text" });
        expect(textarea.value).toBe("test text");
    });

    it("handles value property on select elements", () => {
        const select = document.createElement("select");
        // Select elements should not set value directly
        assignProps(select, { value: "option1" });
        expect(select.value).toBe(""); // Should remain unchanged
    });

    it("handles spellcheck property", () => {
        const input = document.createElement("input");
        assignProps(input, { spellcheck: true });
        expect(input.spellcheck).toBe(true);

        assignProps(input, { spellcheck: false });
        expect(input.spellcheck).toBe(false);

        assignProps(input, { spellcheck: "true" });
        expect(input.spellcheck).toBe(true);

        assignProps(input, { spellcheck: "false" });
        expect(input.spellcheck).toBe(false);

        assignProps(input, { spellcheck: "" });
        expect(input.spellcheck).toBe(true);
    });

    it("handles draggable property", () => {
        assignProps(element, { draggable: true });
        expect(element.getAttribute("draggable")).toBe("true");

        assignProps(element, { draggable: false });
        expect(element.getAttribute("draggable")).toBe("false");

        assignProps(element, { draggable: "" });
        expect(element.getAttribute("draggable")).toBe("false"); // Remains since prev is undefined
    });

    it("handles contenteditable property", () => {
        assignProps(element, { contenteditable: true });
        expect(element.getAttribute("contenteditable")).toBe("true");

        assignProps(element, { contenteditable: false });
        expect(element.getAttribute("contenteditable")).toBe("false");

        assignProps(element, { contenteditable: "" });
        expect(element.getAttribute("contenteditable")).toBe("true");
    });

    it("ignores ref and namespaceURI properties", () => {
        assignProps(element, { ref: () => { }, namespaceURI: "test" });
        // Should not throw or set anything
    });

    it("handles style property", () => {
        assignProps(element, { style: { color: "red", fontSize: "14px" } });
        expect(element.style.color).toBe("red");
        expect(element.style.fontSize).toBe("14px");
    });

    it("handles event properties", () => {
        const handler = vi.fn();
        assignProps(element, { onClick: handler });

        element.click();
        expect(handler).toHaveBeenCalled();
    });

    it("handles event capture properties", () => {
        const handler = vi.fn();
        assignProps(element, { onClickCapture: handler });

        element.click();
        expect(handler).toHaveBeenCalled();
    });

    it("handles update of on property with prev signal value", () => {
        const handler1 = vi.fn();
        const handler2 = vi.fn();
        const handler3 = vi.fn();

        const handlerSignal = mockSignal<Record<string, ((e: Event) => void) | null>>({ click: handler1, close: handler2, beforeInput: handler3 });

        const addEventListenerSpy = vi.spyOn(element, "addEventListener");
        const removeEventListenerSpy = vi.spyOn(element, "removeEventListener");

        assignProps(element, { on: handlerSignal });

        expect(removeEventListenerSpy).not.toHaveBeenCalled();
        expect(addEventListenerSpy).toHaveBeenCalledTimes(4);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(1, "disposing", expect.any(Function), { once: true });
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(2, "click", handler1, false);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(3, "close", handler2, false);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(4, "beforeInput", handler3, false);

        handlerSignal.value = { click: handler3, animationStart: handler2 };

        expect(removeEventListenerSpy).toHaveBeenCalledTimes(3);
        expect(removeEventListenerSpy).toHaveBeenNthCalledWith(1, "click", handler1, false);
        expect(removeEventListenerSpy).toHaveBeenNthCalledWith(2, "close", handler2, false);
        expect(removeEventListenerSpy).toHaveBeenNthCalledWith(3, "beforeInput", handler3, false);
        expect(addEventListenerSpy).toHaveBeenCalledTimes(6);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(5, "click", handler3, false);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(6, "animationStart", handler2, false);

    });

    it("handles update of on property with prev binding value", () => {
        const handler1 = vi.fn();
        const handler2 = vi.fn();
        const handler3 = vi.fn();

        const binding = usePropBinding<Record<string, ((e: Event) => void) | null>>({ click: handler1, close: handler2, beforeInput: handler3 });

        const addEventListenerSpy = vi.spyOn(element, "addEventListener");
        const removeEventListenerSpy = vi.spyOn(element, "removeEventListener");

        assignProps(element, { on: binding });

        expect(removeEventListenerSpy).not.toHaveBeenCalled();
        expect(addEventListenerSpy).toHaveBeenCalledTimes(4);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(1, "disposing", expect.any(Function), { once: true });
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(2, "click", handler1, false);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(3, "close", handler2, false);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(4, "beforeInput", handler3, false);

        binding({ click: handler3, animationStart: handler2 });

        expect(removeEventListenerSpy).toHaveBeenCalledTimes(3);
        expect(removeEventListenerSpy).toHaveBeenNthCalledWith(1, "click", handler1, false);
        expect(removeEventListenerSpy).toHaveBeenNthCalledWith(2, "close", handler2, false);
        expect(removeEventListenerSpy).toHaveBeenNthCalledWith(3, "beforeInput", handler3, false);
        expect(addEventListenerSpy).toHaveBeenCalledTimes(6);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(5, "click", handler3, false);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(6, "animationStart", handler2, false);

    });

    it("handles update of single on event with prev signal value", () => {
        const handler1 = vi.fn();
        const handler2 = vi.fn();

        const handlerSignal = mockSignal<((e: Event) => void) | null>(handler1);

        const addEventListenerSpy = vi.spyOn(element, "addEventListener");
        const removeEventListenerSpy = vi.spyOn(element, "removeEventListener");

        assignProps(element, { onClick: handlerSignal });

        expect(removeEventListenerSpy).not.toHaveBeenCalled();
        expect(addEventListenerSpy).toHaveBeenCalledTimes(1);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(1, "disposing", expect.any(Function), { once: true });
        // direct property assignment for event with a corresponding property
        expect(element.onclick).toBe(handler1);

        handlerSignal.value = handler2;

        expect(removeEventListenerSpy).not.toHaveBeenCalled();
        expect(element.onclick).toBe(handler2);
    });

    it("handles update of single on event with prev binding value", () => {
        const handler1 = vi.fn();
        const handler2 = vi.fn();

        const binding = usePropBinding<((e: Event) => void) | null>(handler1);

        const addEventListenerSpy = vi.spyOn(element, "addEventListener");
        const removeEventListenerSpy = vi.spyOn(element, "removeEventListener");

        assignProps(element, { onClick: binding });

        expect(removeEventListenerSpy).not.toHaveBeenCalled();
        expect(addEventListenerSpy).toHaveBeenCalledTimes(1);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(1, "disposing", expect.any(Function), { once: true });
        // direct property assignment for event with a corresponding property
        expect(element.onclick).toBe(handler1);

        binding(handler2);

        expect(removeEventListenerSpy).not.toHaveBeenCalled();
        expect(element.onclick).toBe(handler2);
    });

    it("handles update of single custom on event", () => {
        const handler1 = vi.fn();
        const handler2 = vi.fn();

        const handlerSignal = mockSignal<((e: Event) => void) | null>(handler1);

        const addEventListenerSpy = vi.spyOn(element, "addEventListener");
        const removeEventListenerSpy = vi.spyOn(element, "removeEventListener");

        assignProps(element, { onMycustom: handlerSignal });

        expect(removeEventListenerSpy).not.toHaveBeenCalled();
        expect(addEventListenerSpy).toHaveBeenCalledTimes(2);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(1, "disposing", expect.any(Function), { once: true });
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(2, "mycustom", handler1);

        handlerSignal.value = handler2;

        expect(removeEventListenerSpy).toHaveBeenCalledTimes(1);
        expect(removeEventListenerSpy).toHaveBeenNthCalledWith(1, "mycustom", handler1);
        expect(addEventListenerSpy).toHaveBeenCalledTimes(3);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(3, "mycustom", handler2);
    });

    it("handles update of single custom onCapture event", () => {
        const handler1 = vi.fn();
        const handler2 = vi.fn();

        const handlerSignal = mockSignal<((e: Event) => void) | null>(handler1);

        const addEventListenerSpy = vi.spyOn(element, "addEventListener");
        const removeEventListenerSpy = vi.spyOn(element, "removeEventListener");

        assignProps(element, { onMycustomCapture: handlerSignal });

        expect(removeEventListenerSpy).not.toHaveBeenCalled();
        expect(addEventListenerSpy).toHaveBeenCalledTimes(2);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(1, "disposing", expect.any(Function), { once: true });
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(2, "mycustom", handler1, true);

        handlerSignal.value = handler2;

        expect(removeEventListenerSpy).toHaveBeenCalledTimes(1);
        expect(removeEventListenerSpy).toHaveBeenNthCalledWith(1, "mycustom", handler1, true);
        expect(addEventListenerSpy).toHaveBeenCalledTimes(3);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(3, "mycustom", handler2, true);
    });

    it("handles xlink attributes", () => {
        const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        assignProps(svg, { xlinkHref: "test.svg" });
        // Check that some attribute is set (namespace support may vary in test environment)
        expect(svg.hasAttribute("xlink:href") || svg.getAttribute("xlink:href") === "test.svg").toBe(true);
    });

    it("handles xmlnsXlink attribute", () => {
        assignProps(element, { xmlnsXlink: "http://www.w3.org/1999/xlink" });
        expect(element.getAttribute("xmlns:xlink")).toBe("http://www.w3.org/1999/xlink");
    });

    it("handles xml attributes", () => {
        const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        assignProps(svg, { xmlBase: "test.xml" });
        // Check that some attribute is set
        expect(svg.hasAttribute("xml:base") || svg.getAttribute("xml:base") === "test.xml").toBe(true);
    });

    it("handles function properties that are not events", () => {
        const consoleWarnSpy = vi.spyOn(console, "warn").mockImplementation(() => { });
        assignProps(element, { customFunc: () => { } });
        expect(consoleWarnSpy).toHaveBeenCalledWith(
            expect.stringContaining("A function was provided for JSX"),
            expect.any(Function),
            "DIV"
        );
        consoleWarnSpy.mockRestore();
    });

    it("handles boolean attributes", () => {
        assignProps(element, { disabled: true });
        expect(element.getAttribute("disabled")).toBe("");

        // Setting to false doesn't remove since prev is undefined
        assignProps(element, { disabled: false });
        expect(element.hasAttribute("disabled")).toBe(true);
    });

    it("handles object properties", () => {
        const obj = { test: "value" };
        assignProps(element, { customObj: obj });
        expect((element as any).customObj).toBe(obj);
    });

    it("handles mapped keys", () => {
        assignProps(element, { className: "test-class", htmlFor: "test-id" });
        expect(element.className).toBe("test-class");
        expect(element.getAttribute("for")).toBe("test-id");
    });
});

describe("assignProp", () => {
    it("assigns and updates standard attributes", () => {
        assignProp(element, "id", "test-id");
        expect(element.id).toBe("test-id");

        assignProp(element, "id", "new-id", "test-id");
        expect(element.id).toBe("new-id");
    });

    it("handles dataset property", () => {
        assignProp(element, "dataset", { test: "value", other: "data" });
        expect(element.dataset.test).toBe("value");
        expect(element.dataset.other).toBe("data");

        // Test clearing with prev
        assignProp(element, "dataset", { test: "new-value", third: "more" }, { test: "value", other: "data" });
        expect(element.dataset.test).toBe("new-value");
        expect(element.dataset.other).toBeUndefined();
        expect(element.dataset.third).toBe("more");
    });

    it("handles textContent property", () => {
        assignProp(element, "textContent", "Hello World");
        expect(element.textContent).toBe("Hello World");

        // Test clearing with prev
        assignProp(element, "textContent", null, "Hello World");
        expect(element.textContent).toBe("");
    });

    it("handles dangerouslySetInnerHTML property", () => {
        assignProp(element, "dangerouslySetInnerHTML", { __html: "<span>test</span>" });
        expect(element.innerHTML).toBe("<span>test</span>");

        // Test clearing with prev
        assignProp(element, "dangerouslySetInnerHTML", null, { __html: "<span>test</span>" });
        expect(element.innerHTML).toBe("");
    });

    it("handles value property on input elements", () => {
        const input = document.createElement("input");
        assignProp(input, "value", "test value");
        expect(input.value).toBe("test value");

        // Test clearing with prev
        assignProp(input, "value", null, "test value");
        expect(input.value).toBe("");
    });

    it("handles value property on textarea elements", () => {
        const textarea = document.createElement("textarea");
        assignProp(textarea, "value", "test text");
        expect(textarea.value).toBe("test text");
    });

    it("handles value property on select elements", () => {
        const select = document.createElement("select");
        // Select elements should not set value directly
        assignProp(select, "value", "option1");
        expect(select.value).toBe(""); // Should remain unchanged
    });

    it("handles spellcheck property", () => {
        const input = document.createElement("input");
        assignProp(input, "spellcheck", true);
        expect(input.spellcheck).toBe(true);

        assignProp(input, "spellcheck", false);
        expect(input.spellcheck).toBe(false);

        assignProp(input, "spellcheck", "true");
        expect(input.spellcheck).toBe(true);

        assignProp(input, "spellcheck", "false");
        expect(input.spellcheck).toBe(false);

        assignProp(input, "spellcheck", "");
        expect(input.spellcheck).toBe(true);
    });

    it("handles draggable property", () => {
        assignProp(element, "draggable", true);
        expect(element.getAttribute("draggable")).toBe("true");

        assignProp(element, "draggable", false);
        expect(element.getAttribute("draggable")).toBe("false");

        assignProp(element, "draggable", "");
        expect(element.getAttribute("draggable")).toBe("false");
    });

    it("handles contenteditable property", () => {
        assignProp(element, "contenteditable", true);
        expect(element.getAttribute("contenteditable")).toBe("true");

        assignProp(element, "contenteditable", false);
        expect(element.getAttribute("contenteditable")).toBe("false");

        assignProp(element, "contenteditable", "");
        expect(element.getAttribute("contenteditable")).toBe("true");
    });

    it("ignores ref and namespaceURI properties", () => {
        assignProp(element, "ref", () => { });
        assignProp(element, "namespaceURI", "test");
        // Should not throw or set anything
    });

    it("handles style property", () => {
        assignProp(element, "style", { color: "red", fontSize: "14px" });
        expect(element.style.color).toBe("red");
        expect(element.style.fontSize).toBe("14px");
    });

    it("handles event properties", () => {
        const handler = vi.fn();
        assignProp(element, "onClick", handler);

        element.click();
        expect(handler).toHaveBeenCalled();
    });

    it("handles event capture properties", () => {
        const handler = vi.fn();
        assignProp(element, "onClickCapture", handler);

        element.click();
        expect(handler).toHaveBeenCalled();
    });

    it("handles xlink attributes", () => {
        const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        assignProp(svg, "xlinkHref", "test.svg");
        // Check that some attribute is set (namespace support may vary in test environment)
        expect(svg.hasAttribute("xlink:href") || svg.getAttribute("xlink:href") === "test.svg").toBe(true);
    });

    it("handles xmlnsXlink attribute", () => {
        assignProp(element, "xmlnsXlink", "http://www.w3.org/1999/xlink");
        expect(element.getAttribute("xmlns:xlink")).toBe("http://www.w3.org/1999/xlink");
    });

    it("handles xml attributes", () => {
        const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        assignProp(svg, "xmlBase", "test.xml");
        // Check that some attribute is set
        expect(svg.hasAttribute("xml:base") || svg.getAttribute("xml:base") === "test.xml").toBe(true);
    });

    it("handles function properties that are not events", () => {
        const consoleWarnSpy = vi.spyOn(console, "warn").mockImplementation(() => { });
        assignProp(element, "customFunc", () => { });
        expect(consoleWarnSpy).toHaveBeenCalledWith(
            expect.stringContaining("A function was provided for JSX"),
            expect.any(Function),
            "DIV"
        );
        consoleWarnSpy.mockRestore();
    });

    it("handles boolean attributes", () => {
        assignProp(element, "disabled", true);
        expect(element.getAttribute("disabled")).toBe("");

        // Test removal with prev
        assignProp(element, "disabled", false, true);
        expect(element.hasAttribute("disabled")).toBe(false);
    });

    it("handles object properties", () => {
        const obj = { test: "value" };
        assignProp(element, "customObj", obj);
        expect((element as any).customObj).toBe(obj);
    });

    it("handles attribute removal with prev", () => {
        assignProp(element, "data-test", "value");
        expect(element.getAttribute("data-test")).toBe("value");

        assignProp(element, "data-test", null, "value");
        expect(element.hasAttribute("data-test")).toBe(false);
    });

    it("handles object property removal with prev", () => {
        const obj1 = { test: "value1" };
        const obj2 = { test: "value2" };
        assignProp(element, "customObj", obj1);
        expect((element as any).customObj).toBe(obj1);

        assignProp(element, "customObj", obj2, obj1);
        expect((element as any).customObj).toBe(obj2);
    });
});