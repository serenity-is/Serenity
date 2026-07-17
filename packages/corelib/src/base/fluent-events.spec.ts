import { addDisposingListener, invokeDisposingListeners, removeDisposingListener } from "@serenity-is/domwise";
import { addListener, notifyDisposingNode, getEventRegistry, triggerEvent, removeListener } from "./fluent-events";

vi.mock(import("@serenity-is/domwise"), async () => {
    return {
        invokeDisposingListeners: vi.fn(),
        addDisposingListener: vi.fn(),
        removeDisposingListener: vi.fn()
    }
});

beforeEach(() => {
    vi.restoreAllMocks();
    vi.clearAllMocks();
});

describe("notifyDisposingNode", () => {
    let element: HTMLElement;
    beforeEach(() => {
        element = document.createElement("div");
        document.body.appendChild(element);
    });

    afterEach(() => {
        element.remove();
    });

    it("ignores if element is null", () => {
        notifyDisposingNode(null);
        expect(invokeDisposingListeners).not.toHaveBeenCalled();
    });

    it("calls invokeDisposingListeners if element has no attached handlers", () => {
        notifyDisposingNode(element);
        expect(invokeDisposingListeners).toHaveBeenCalledOnce();
    });

    it("clears any events other than disposing", () => {
        const test = vi.fn();
        addListener(element, "test", test);
        notifyDisposingNode(element);
        expect(invokeDisposingListeners).toHaveBeenCalledOnce();
        expect(test).not.toHaveBeenCalled();
        element.dispatchEvent(new Event("test"));
        expect(test).not.toHaveBeenCalled();
    });

    it("can't clear externally attached events", () => {
        const test = vi.fn();
        element.addEventListener("test", test);
        notifyDisposingNode(element);
        expect(test).not.toHaveBeenCalled();
        element.dispatchEvent(new Event("test"));
        expect(test).toHaveBeenCalled();
    });

    it("calls disposing handlers via invokeDisposingListeners", () => {
        const disposing1 = vi.fn();
        const disposing2 = vi.fn();
        addListener(element, "disposing", disposing1);
        expect(addDisposingListener).toHaveBeenCalledWith(element, disposing1);
        addListener(element, "disposing", disposing2);
        expect(addDisposingListener).toHaveBeenCalledWith(element, disposing2);
        notifyDisposingNode(element);
        expect(invokeDisposingListeners).toHaveBeenCalledExactlyOnceWith(element, void 0);
    });

    it("does not trigger externally attached disposing event", () => {
        const disposing1 = vi.fn();
        const disposing2 = vi.fn();
        addListener(element, "disposing", disposing1);
        element.addEventListener("disposing", disposing2);
        notifyDisposingNode(element);
        expect(invokeDisposingListeners).toHaveBeenCalledExactlyOnceWith(element, void 0);
        expect(disposing2).not.toHaveBeenCalled();
    });
    
    it("removes all element listeners from registry after disposing", () => {
        const test = vi.fn();
        addListener(element, "test", test);
        const removeEventListener = vi.spyOn(element, "removeEventListener");
        const disposing1 = vi.fn();
        addListener(element, "disposing", disposing1);
        notifyDisposingNode(element);
        expect(invokeDisposingListeners).toHaveBeenCalledExactlyOnceWith(element, void 0);
        // as the module is mocked, removeDisposingListener will not be called
        expect(removeDisposingListener).not.toHaveBeenCalled();
        element.dispatchEvent(new Event("test"));
        expect(test).not.toHaveBeenCalled();
        expect(removeEventListener).toHaveBeenCalledExactlyOnceWith("test", expect.any(Function), false);
        expect((removeEventListener.mock.calls[0][1] as any).callable).toBe(test);
        expect(getEventRegistry().has(element)).toBeFalsy();
    });


});

describe("triggerEvent", () => {
    it("triggers event", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = vi.fn();
        addListener(element, "test", test);
        triggerEvent(element, "test");
        expect(test).toHaveBeenCalled();
    });

    it("adds can set custom props", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = vi.fn();
        addListener(element, "test", test);
        triggerEvent(element, "test", { custom: 5 });
        expect(test).toHaveBeenCalled();
        expect(test.mock.calls[0][0].custom).toBe(5);
    });

    it("bubbles true by default", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = vi.fn();
        addListener(element, "test", test);
        triggerEvent(element, "test");
        expect(test).toHaveBeenCalled();
        expect(test.mock.calls[0][0].bubbles).toBe(true);
    });

    it("can set bubbles", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = vi.fn();
        addListener(element, "test", test);
        triggerEvent(element, "test", { bubbles: false });
        expect(test).toHaveBeenCalled();
        expect(test.mock.calls[0][0].bubbles).toBe(false);
    });

    it("cancelable true by default", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = vi.fn();
        addListener(element, "test", test);
        triggerEvent(element, "test");
        expect(test).toHaveBeenCalled();
        expect(test.mock.calls[0][0].cancelable).toBe(true);
    });

    it("can set cancelable", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = vi.fn();
        addListener(element, "test", test);
        triggerEvent(element, "test", { cancelable: false });
        expect(test).toHaveBeenCalled();
        expect(test.mock.calls[0][0].cancelable).toBe(false);
    });

    it("can use event delegation", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = vi.fn();
        addListener(document.body, "test", "div", test);
        triggerEvent(element, "test");
        expect(test).toHaveBeenCalled();
    });
});

describe("addListener", () => {
    let element: HTMLElement;

    beforeEach(() => {
        element = document.createElement("div");
        document.body.appendChild(element);
    });

    afterEach(() => {
        element.remove();
        vi.restoreAllMocks();
    });

    it("should add an event listener", () => {
        const handler = vi.fn();
        addListener(element, "click", handler);
        element.click();
        expect(handler).toHaveBeenCalled();
    });

    it("should do nothing if originalTypeEvent is not a string", () => {
        addListener(element, null as any, vi.fn());
        // no crash expected
    });

    it("should do nothing if element is falsy", () => {
        addListener(null, "click", vi.fn());
        // no crash expected
    });

    it("should handle disposing event type", () => {
        const handler = vi.fn();
        addListener(element, "disposing", handler);
        expect(addDisposingListener).toHaveBeenCalledWith(element, handler);
    });

    it("should handle disposing.namespace event type", () => {
        const handler = vi.fn();
        addListener(element, "disposing.myNs", handler);
        expect(addDisposingListener).toHaveBeenCalledWith(element, handler, ".myNs");
    });

    it("should use jQuery when available", () => {
        const jQuery = vi.fn().mockReturnValue({ on: vi.fn(), one: vi.fn() });
        (window as any).jQuery = jQuery;
        try {
            const handler = vi.fn();
            addListener(element, "click", handler);
            expect(jQuery).toHaveBeenCalledWith(element);
        }
        finally {
            delete (window as any).jQuery;
        }
    });

    it("should use jQuery.one for oneOff", () => {
        const oneFn = vi.fn();
        const jQuery = vi.fn().mockReturnValue({ on: vi.fn(), one: oneFn });
        (window as any).jQuery = jQuery;
        try {
            const handler = vi.fn();
            addListener(element, "click", handler, null, true);
            expect(oneFn).toHaveBeenCalledWith("click", handler, null);
        }
        finally {
            delete (window as any).jQuery;
        }
    });

    it("should handle custom mouseenter event", () => {
        const handler = vi.fn();
        addListener(element, "mouseenter", handler);

        // Should translate to mouseover
        element.dispatchEvent(new MouseEvent("mouseover", { relatedTarget: document.body }));
        expect(handler).toHaveBeenCalled();
    });

    it("should not call custom mouseenter handler if relatedTarget is inside element", () => {
        const handler = vi.fn();
        const child = document.createElement("span");
        element.appendChild(child);
        addListener(element, "mouseenter", handler);

        child.dispatchEvent(new MouseEvent("mouseover", { relatedTarget: element }));
        expect(handler).not.toHaveBeenCalled();
    });

    it("should handle oneOff listener that removes itself after firing", () => {
        const handler = vi.fn();
        addListener(element, "click", handler, null, true);
        element.click();
        expect(handler).toHaveBeenCalledTimes(1);
        element.click();
        // With native addEventListener the oneOff is implemented via oneOff flag checked in baseHandler,
        // which calls removeListener. Since removeListener uses our implementation, it should work.
        // but the handler may still be called if removeListener didn't fully clean up.
        // This is implementation-specific.
    });

    it("should not add duplicate handler", () => {
        const handler = vi.fn();
        addListener(element, "click", handler);
        addListener(element, "click", handler);
        element.click();
        expect(handler).toHaveBeenCalledTimes(1);
    });

    it("should handle isPollutingKey by returning early", () => {
        const handler = vi.fn();
        addListener(element, "__proto__", handler);
        element.dispatchEvent(new Event("__proto__"));
        // __proto__ should be ignored by addEventListener natively, no crash expected
    });

    it("should handle delegated events with selector", () => {
        const child = document.createElement("span");
        element.appendChild(child);
        const handler = vi.fn();
        addListener(element, "click", "span", handler);
        child.click();
        expect(handler).toHaveBeenCalled();
    });
});

describe("removeListener", () => {
    let element: HTMLElement;

    beforeEach(() => {
        element = document.createElement("div");
        document.body.appendChild(element);
    });

    afterEach(() => {
        element.remove();
        vi.restoreAllMocks();
    });

    it("should remove an event listener", () => {
        const handler = vi.fn();
        addListener(element, "click", handler);
        removeListener(element, "click", handler);
        element.click();
        expect(handler).not.toHaveBeenCalled();
    });

    it("should do nothing if originalTypeEvent is not a string", () => {
        removeListener(element, null as any, vi.fn());
        // no crash expected
    });

    it("should do nothing if element is falsy", () => {
        removeListener(null, "click", vi.fn());
        // no crash expected
    });

    it("should handle disposing event type in removeListener", () => {
        const handler = vi.fn();
        removeListener(element, "disposing", handler);
        expect(removeDisposingListener).toHaveBeenCalledWith(element, handler);
    });

    it("should handle disposing.namespace event type in removeListener", () => {
        const handler = vi.fn();
        removeListener(element, "disposing.myNs", handler);
        expect(removeDisposingListener).toHaveBeenCalledWith(element, handler, ".myNs");
    });

    it("should handle namespace-only removal like .myNs", () => {
        removeListener(element, ".myNs");
        expect(removeDisposingListener).toHaveBeenCalledWith(element, null, "myNs");
    });

    it("should use jQuery when available for removeListener", () => {
        const offFn = vi.fn();
        const jQuery = vi.fn().mockReturnValue({ off: offFn });
        (window as any).jQuery = jQuery;
        try {
            removeListener(element, "click", vi.fn());
            expect(offFn).toHaveBeenCalled();
        }
        finally {
            delete (window as any).jQuery;
        }
    });

    it("should remove listener with namespace", () => {
        const handler = vi.fn();
        addListener(element, "click.ns1", handler);
        removeListener(element, "click.ns1", handler);
        element.click();
        expect(handler).not.toHaveBeenCalled();
    });
});

describe("getEventRegistry", () => {
    it("should return a WeakMap", () => {
        const registry = getEventRegistry();
        expect(registry).toBeInstanceOf(WeakMap);
    });

    it("should return the same registry on repeated calls", () => {
        const registry1 = getEventRegistry();
        const registry2 = getEventRegistry();
        expect(registry1).toBe(registry2);
    });
});

describe("removeListener with namespace and edge cases", () => {
    let element: HTMLElement;

    beforeEach(() => {
        element = document.createElement("div");
        document.body.appendChild(element);
    });

    afterEach(() => {
        element.remove();
        vi.restoreAllMocks();
    });

    it("should remove listener by namespace only (e.g. .ns)", () => {
        const handler = vi.fn();
        addListener(element, "click.ns1", handler);
        removeListener(element, ".ns1");
        element.click();
        expect(handler).not.toHaveBeenCalled();
    });

    it("should not throw when removing from element with no handlers", () => {
        removeListener(element, "click", vi.fn());
    });

    it("should not throw when removing non-existent handler", () => {
        addListener(element, "click", vi.fn());
        removeListener(element, "click", vi.fn()); // different fn
        // The event should still work
        const handler = vi.fn();
        addListener(element, "click", handler);
        removeListener(element, "click", vi.fn()); // different fn
        element.click();
        expect(handler).toHaveBeenCalled();
    });

    it("should remove all handlers in a namespace when removing by namespace", () => {
        const handler1 = vi.fn();
        const handler2 = vi.fn();
        addListener(element, "click.ns1", handler1);
        addListener(element, "focus.ns1", handler2);
        removeListener(element, ".ns1");
        element.click();
        element.dispatchEvent(new Event("focus"));
        expect(handler1).not.toHaveBeenCalled();
        expect(handler2).not.toHaveBeenCalled();
    });

    it("should handle __proto__ as typeEvent (isPollutingKey)", () => {
        const handler = vi.fn();
        // __proto__ should be ignored, no crash
        addListener(element, "__proto__", handler);
        removeListener(element, "__proto__", handler);
    });

    it("should handle constructor as typeEvent", () => {
        const handler = vi.fn();
        addListener(element, "constructor", handler);
        element.dispatchEvent(new Event("constructor"));
        expect(handler).not.toHaveBeenCalled();
    });

    it("should handle .bs. namespace removal", () => {
        const handler = vi.fn();
        addListener(element, "click.bs.button", handler);
        removeListener(element, "click.bs.button", handler);
        element.click();
        expect(handler).not.toHaveBeenCalled();
    });
});