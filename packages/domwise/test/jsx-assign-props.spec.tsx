import { beforeEach, describe, expect, it, vi } from "vitest";
import { usePropBinding } from "../src";
import { getDisposingListeners, invokeDisposingListeners } from "../src/disposing-listener";
import { assignProp, assignProps } from "../src/jsx-assign-props";
import { derivedSignal } from "../src/signal-util";
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

    it("handles dataset property set to null", () => {
        assignProps(element, { dataset: { test: "value" } });
        expect(element.dataset.test).toBe("value");
        expect(() => assignProps(element, { dataset: null })).not.toThrow();
    });

    it("clears dataset when signal becomes null", () => {
        const sig = mockSignal<any>({ test: "value", other: "data" });
        assignProps(element, { dataset: sig });
        expect(element.dataset.test).toBe("value");
        expect(element.dataset.other).toBe("data");

        sig.value = null;
        expect(element.dataset.test).toBeUndefined();
        expect(element.dataset.other).toBeUndefined();
    });

    it("handles on / onCapture set to null", () => {
        expect(() => assignProps(element, { on: null })).not.toThrow();
        expect(() => assignProps(element, { on: undefined })).not.toThrow();
        expect(() => assignProps(element, { onCapture: null })).not.toThrow();
    });

    it("clears on listeners when signal becomes null", () => {
        const handler = vi.fn();
        const sig = mockSignal<any>({ click: handler });
        assignProps(element, { on: sig });
        element.click();
        expect(handler).toHaveBeenCalledTimes(1);

        sig.value = null;
        element.click();
        expect(handler).toHaveBeenCalledTimes(1);
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
        select.innerHTML = '<option value="opt1"></option><option value="opt2"></option>';
        assignProps(select, { value: "opt2" });
        expect(select.value).toBe("opt2");
    });

    it("updates select value reactively via signal", () => {
        const select = document.createElement("select");
        select.innerHTML = '<option value="a"></option><option value="b"></option><option value="c"></option>';
        const sig = signal("a");
        assignProps(select, { value: sig });
        expect(select.value).toBe("a");
        sig.value = "c";
        expect(select.value).toBe("c");
    });

    it("follows the input value signal after the user edits the field and through null", () => {
        const value = mockSignal<any>("x");
        const input = <input value={value} /> as HTMLInputElement;
        document.body.appendChild(input);
        expect(input.value).toBe("x");

        input.value = "typed";
        value.value = "y";
        expect(input.value).toBe("y");

        value.value = null;
        expect(input.value).toBe("");
    });

    it("reflects the initial input value as an attribute for serialization", () => {
        const input = <input value="18" /> as HTMLInputElement;
        expect(input.value).toBe("18");
        expect(input.getAttribute("value")).toBe("18");
        expect(input.outerHTML).toContain('value="18"');
    });

    it("tracks the input value attribute on signal updates", () => {
        const value = mockSignal<any>("x");
        const input = <input value={value} /> as HTMLInputElement;
        expect(input.getAttribute("value")).toBe("x");

        value.value = "y";
        expect(input.value).toBe("y");
        expect(input.getAttribute("value")).toBe("y");
    });

    it("does not clear the default selection when select value is null", () => {
        const select = document.createElement("select");
        select.innerHTML = '<option value="a">a</option><option value="b">b</option>';
        assignProps(select, { value: null } as any);
        expect(select.value).toBe("a");
    });

    it("applies a prop-hook value on a select after its options exist", () => {
        const binding = usePropBinding("b");
        const select = <select value={binding}><option value="a">a</option><option value="b">b</option></select> as HTMLSelectElement;
        expect(select.value).toBe("b");
    });

    it("keeps following the checked signal after the user toggles the checkbox", () => {
        const checked = mockSignal(false);
        const input = <input type="checkbox" checked={checked} /> as HTMLInputElement;
        document.body.appendChild(input);
        expect(input.checked).toBe(false);

        checked.value = true;
        expect(input.checked).toBe(true);

        input.click(); // user unchecks
        expect(input.checked).toBe(false);

        checked.value = false;
        checked.value = true;
        expect(input.checked).toBe(true);
    });

    it("accepts truthy non-boolean checked values", () => {
        const cb = document.createElement("input");
        cb.type = "checkbox";
        assignProps(cb, { checked: 1 } as any);
        expect(cb.checked).toBe(true);
        assignProps(cb, { checked: "yes" } as any);
        expect(cb.checked).toBe(true);
    });

    it("reflects checked to the attribute and survives form.reset()", () => {
        const form = document.createElement("form");
        const input = document.createElement("input");
        input.type = "checkbox";
        form.appendChild(input);
        document.body.appendChild(form);

        assignProps(input, { checked: true });
        expect(input.checked).toBe(true);
        expect(input.getAttribute("checked")).not.toBeNull();
        expect(input.defaultChecked).toBe(true);

        input.checked = false; // simulate user interaction
        form.reset();
        expect(input.checked).toBe(true);
    });

    it("falls back to the attribute for checked on non-input elements", () => {
        const div = document.createElement("div");
        assignProps(div, { checked: true } as any);
        expect(div.hasAttribute("checked")).toBe(true);
        expect((div as any).checked).toBeUndefined();
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

    it("removes the draggable attribute when the value becomes null", () => {
        const draggable = mockSignal<any>("true");
        assignProps(element, { draggable });
        draggable.value = false;
        expect(element.getAttribute("draggable")).toBe("false");
        draggable.value = null;
        expect(element.hasAttribute("draggable")).toBe(false);
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

    it("derives lowercase names for camelCase standard events", () => {
        const handler = vi.fn();
        assignProps(element, { onFocusIn: handler });
        element.dispatchEvent(new Event("focusin"));
        expect(handler).toHaveBeenCalledTimes(1);
    });

    it("does not treat gotpointercapture as a capture-phase event", () => {
        const handler = vi.fn();
        const addEventListenerSpy = vi.spyOn(element, "addEventListener");
        assignProps(element, { onGotPointerCapture: handler });

        expect(addEventListenerSpy).not.toHaveBeenCalledWith("gotpointer", handler, true);
        element.dispatchEvent(new Event("gotpointercapture"));
        expect(handler).toHaveBeenCalledTimes(1);
    });

    it("keeps capture phase for standard onClickCapture", () => {
        const handler = vi.fn();
        const addEventListenerSpy = vi.spyOn(element, "addEventListener");
        assignProps(element, { onClickCapture: handler });

        expect(addEventListenerSpy).toHaveBeenCalledWith("click", handler, true);
    });

    it("uses lowercase event names for the reported camelCase handlers", () => {
        const cases: [string, string][] = [
            ["onFocusIn", "focusin"],
            ["onFocusOut", "focusout"],
            ["onCompositionStart", "compositionstart"],
            ["onCompositionUpdate", "compositionupdate"],
            ["onCompositionEnd", "compositionend"],
            ["onFullscreenChange", "fullscreenchange"],
            ["onFullscreenError", "fullscreenerror"],
            ["onBeforeCopy", "beforecopy"],
            ["onTouchStart", "touchstart"],
            ["onAnimationStart", "animationstart"],
            ["onTransitionEnd", "transitionend"],
            ["onSelectionChange", "selectionchange"],
            ["onGotPointerCapture", "gotpointercapture"],
            ["onLostPointerCapture", "lostpointercapture"]
        ];
        for (const [attr, eventName] of cases) {
            const el = document.createElement("div");
            const handler = vi.fn();
            assignProp(el, attr, handler as any);
            el.dispatchEvent(new Event(eventName));
            expect(handler, `${attr} should listen to ${eventName}`).toHaveBeenCalledTimes(1);
        }
    });

    it("does not dispose a shared derived signal when one consumer is disposed", () => {
        const src = signal(1);
        const d = derivedSignal(src, v => "v" + v);
        const a = document.createElement("div");
        const b = document.createElement("div");
        assignProps(a, { title: d });
        assignProps(b, { title: d });
        expect(b.getAttribute("title")).toBe("v1");

        invokeDisposingListeners(a, { descendants: true });
        src.value = 2;
        expect(b.getAttribute("title")).toBe("v2");
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
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(1, "disposing", expect.any(Function));
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
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(1, "disposing", expect.any(Function));
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
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(1, "disposing", expect.any(Function));
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
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(1, "disposing", expect.any(Function));
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
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(1, "disposing", expect.any(Function));
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
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(1, "disposing", expect.any(Function));
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(2, "mycustom", handler1, true);

        handlerSignal.value = handler2;

        expect(removeEventListenerSpy).toHaveBeenCalledTimes(1);
        expect(removeEventListenerSpy).toHaveBeenNthCalledWith(1, "mycustom", handler1, true);
        expect(addEventListenerSpy).toHaveBeenCalledTimes(3);
        expect(addEventListenerSpy).toHaveBeenNthCalledWith(3, "mycustom", handler2, true);
    });

    it("clears a reactive property event handler when signal becomes null", () => {
        const handler = vi.fn();
        const handlerSignal = mockSignal<((e: Event) => void) | null>(handler);

        assignProps(element, { onClick: handlerSignal });
        element.click();
        expect(handler).toHaveBeenCalledTimes(1);
        expect(element.onclick).toBe(handler);

        handlerSignal.value = null;
        expect(element.onclick).toBeNull();
        element.click();
        expect(handler).toHaveBeenCalledTimes(1);
    });

    it("clears a reactive custom event handler when signal becomes null", () => {
        const handler = vi.fn();
        const handlerSignal = mockSignal<((e: Event) => void) | null>(handler);

        assignProps(element, { onMycustom: handlerSignal });
        element.dispatchEvent(new Event("mycustom"));
        expect(handler).toHaveBeenCalledTimes(1);

        handlerSignal.value = null;
        element.dispatchEvent(new Event("mycustom"));
        expect(handler).toHaveBeenCalledTimes(1);
    });

    it("clears a reactive capture event handler when signal becomes null", () => {
        const handler = vi.fn();
        const handlerSignal = mockSignal<((e: Event) => void) | null>(handler);

        assignProps(element, { onMycustomCapture: handlerSignal });
        element.dispatchEvent(new Event("mycustom"));
        expect(handler).toHaveBeenCalledTimes(1);

        handlerSignal.value = null;
        element.dispatchEvent(new Event("mycustom"));
        expect(handler).toHaveBeenCalledTimes(1);
    });

    it("does not accumulate disposing listeners on repeated event handler updates", () => {
        const button = document.createElement("button");
        const handlers = [vi.fn(), vi.fn(), vi.fn(), vi.fn()];
        const handlerSignal = mockSignal<any>(handlers[0]);

        assignProps(button, { onClick: handlerSignal });
        const baseline = getDisposingListeners().get(button)?.length ?? 0;

        for (let i = 1; i < handlers.length; i++)
            handlerSignal.value = handlers[i];

        expect(getDisposingListeners().get(button)?.length ?? 0).toBe(baseline);
        button.click();
        expect(handlers[0]).not.toHaveBeenCalled();
        expect(handlers[3]).toHaveBeenCalledTimes(1);
    });

    it("removes addEventListener-based handlers on dispose", () => {
        const handler = vi.fn();
        assignProps(element, { onMycustom: handler });
        element.dispatchEvent(new Event("mycustom"));
        expect(handler).toHaveBeenCalledTimes(1);

        invokeDisposingListeners(element);
        element.dispatchEvent(new Event("mycustom"));
        expect(handler).toHaveBeenCalledTimes(1);
    });

    it("nulls property event handlers on dispose", () => {
        const handler = vi.fn();
        assignProps(element, { onClick: handler });
        expect(element.onclick).toBe(handler);

        invokeDisposingListeners(element);
        expect(element.onclick).toBeNull();
    });

    it("removes on-map listeners when the element is disposed", () => {
        const handler = vi.fn();
        assignProps(element, { on: { click: handler } });
        element.dispatchEvent(new Event("click"));
        expect(handler).toHaveBeenCalledTimes(1);

        invokeDisposingListeners(element);
        element.dispatchEvent(new Event("click"));
        expect(handler).toHaveBeenCalledTimes(1);
    });

    it("does not accumulate disposing entries on repeated on-map assignments", () => {
        const handler = () => { };
        assignProps(element, { on: { click: handler } });
        const count = (getDisposingListeners().get(element) ?? []).length;

        assignProps(element, { on: { click: handler } });
        assignProps(element, { on: { click: handler } });
        expect((getDisposingListeners().get(element) ?? []).length).toBe(count);
    });

    it("handles xlink attributes", () => {
        const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        assignProps(svg, { xlinkHref: "test.svg" });
        // Check that some attribute is set (namespace support may vary in test environment)
        expect(svg.hasAttribute("xlink:href") || svg.getAttribute("xlink:href") === "test.svg").toBe(true);
    });

    it("removes the xlinkHref attribute when the value becomes null", () => {
        const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        const href = mockSignal<any>("#id");
        assignProps(svg, { xlinkHref: href });
        expect(svg.getAttributeNS("http://www.w3.org/1999/xlink", "href")).toBe("#id");

        href.value = null;
        expect(svg.getAttributeNS("http://www.w3.org/1999/xlink", "href")).toBeFalsy();
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

    it("compiles capture-phase handlers and boolean/number dataset values", () => {
        const capture = <div onTouchStartCapture={() => { }} onAnimationEndCapture={() => { }} onGotPointerCapture={() => { }} />;
        expect(capture).toBeDefined();

        const dataset = <div dataset={{ flag: true, count: 3 }} />;
        expect(dataset).toBeDefined();
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
        select.innerHTML = '<option value="opt1"></option><option value="opt2"></option>';
        assignProp(select, "value", "opt2");
        expect(select.value).toBe("opt2");
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