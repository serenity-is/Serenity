import { addDisposingListener, getDisposingListeners, invokeDisposingListeners, removeDisposingListener, dispatchDisposingEvent, currentLifecycleRoot } from "../src/disposing-listener";

let el: HTMLElement;

beforeEach(() => {
    el = document.createElement("div");
    vi.spyOn(el, "addEventListener");
    vi.spyOn(el, "removeEventListener");
});

describe("addDisposingListener", () => {
    it("ignores null target or handler", () => {
        expect(addDisposingListener(null as any, () => { })).toBeNull();
        expect(addDisposingListener(el, null as any)).toBe(el);
    });

    it("does not fail if target has no addEventListener method", () => {
        const target = {} as EventTarget;
        expect(() => addDisposingListener(target, () => { })).not.toThrow();
    });

    it("should add a disposing listener to the target element", () => {
        const listener = vi.fn();
        addDisposingListener(el, listener);
        el.dispatchEvent(new Event("disposing"));
        expect(listener).toHaveBeenCalledOnce();
    });

    it("should not add the same listener multiple times", () => {
        const listener = vi.fn();
        addDisposingListener(el, listener);
        addDisposingListener(el, listener);
        el.dispatchEvent(new Event("disposing"));
        expect(listener).toHaveBeenCalledExactlyOnceWith(el);
    });

    it("only adds the disposing event listener once", () => {
        const listener1 = vi.fn();
        const listener2 = vi.fn();
        addDisposingListener(el, listener1);
        addDisposingListener(el, listener2);
        expect(el.addEventListener).toHaveBeenCalledOnce();
        expect(el.addEventListener).toHaveBeenCalledWith("disposing", expect.any(Function), { once: true });
    });

    it("should throw an error when adding the same listener with a different regKey", () => {
        const listener = vi.fn();
        addDisposingListener(el, listener, "key1");
        expect(() => {
            addDisposingListener(el, listener, "key2");
        }).toThrowError("A disposing listener with the same callback but different regKey is already registered on the target element.");
   });

    it("should update the regKey when adding the same listener with a new regKey", () => {
        const listener = vi.fn();
        addDisposingListener(el, listener);
        addDisposingListener(el, listener, "key1");
        const disposingListeners = (getDisposingListeners().get(el) || []);
        const registeredListener = disposingListeners.find(x => x.callback === listener);
        expect(registeredListener).toBeDefined();
        expect(registeredListener?.regKey).toBe("key1");
    });

});

describe("removeDisposingListener", () => {
    it("ignores null target or handler or key", () => {
        expect(() => removeDisposingListener(null as any, () => { })).not.toThrow();
        expect(() => removeDisposingListener(el, null as any)).not.toThrow();
        expect(() => removeDisposingListener(el, () => { }, null as any)).not.toThrow();
    });

    it("ignores target with no listeners", () => {
        expect(() => removeDisposingListener(el, () => { })).not.toThrow();
    });

    it("ignores handler or key that is not registered", () => {
        const listener = vi.fn();
        addDisposingListener(el, listener);
        expect(() => removeDisposingListener(el, () => { })).not.toThrow();
        expect(() => removeDisposingListener(el, null as any, "nonexistentKey")).not.toThrow();
    });

    it("should remove a disposing listener from the target element", () => {
        const listener = vi.fn();
        addDisposingListener(el, listener);
        removeDisposingListener(el, listener);
        el.dispatchEvent(new Event("disposing"));
        expect(listener).not.toHaveBeenCalled();
    });

    it("should remove only the specified listener", () => {
        const listener1 = vi.fn();
        const listener2 = vi.fn();
        addDisposingListener(el, listener1);
        addDisposingListener(el, listener2);
        removeDisposingListener(el, listener1);
        el.dispatchEvent(new Event("disposing"));
        expect(listener1).not.toHaveBeenCalled();
        expect(listener2).toHaveBeenCalledOnce();
    });

    it("should remove the disposing event listener when no more disposing listeners remain", () => {
        const listener = vi.fn();
        addDisposingListener(el, listener);
        removeDisposingListener(el, listener);
        expect(el.removeEventListener).toHaveBeenCalledOnce();
        expect(el.removeEventListener).toHaveBeenCalledWith("disposing", expect.any(Function));
    });

    it("should remove the correct listener when regKey is used", () => {
        const listener1 = vi.fn();
        const listener2 = vi.fn();
        addDisposingListener(el, listener1, "key1");
        addDisposingListener(el, listener2, "key2");
        removeDisposingListener(el, null as any, "key1");
        el.dispatchEvent(new Event("disposing"));
        expect(listener1).not.toHaveBeenCalled();
        expect(listener2).toHaveBeenCalledOnce();
    });

    it("should remove all listeners with the same callback when regKey is not used", () => {
        const listener = vi.fn();
        addDisposingListener(el, listener, "key1");
        addDisposingListener(el, listener);
        removeDisposingListener(el, listener);
        el.dispatchEvent(new Event("disposing"));
        expect(listener).not.toHaveBeenCalled();
    });

    it("should remove the global listener when all listeners are removed", () => {
        const listener1 = vi.fn();
        const listener2 = vi.fn();
        addDisposingListener(el, listener1);
        addDisposingListener(el, listener2);
        removeDisposingListener(el, listener1);
        removeDisposingListener(el, listener2);
        expect(el.addEventListener).toHaveBeenCalledOnce();
        expect(el.removeEventListener).toHaveBeenCalledOnce();
        expect(el.removeEventListener).toHaveBeenCalledWith("disposing", (el.addEventListener as any).mock.calls[0][1]);
    });
});

describe("invokeDisposingListeners", () => {
    it("ignores null target", () => {
        expect(() => invokeDisposingListeners(null as any)).not.toThrow();
    });

    it("should call the callback when the disposing event is dispatched", () => {
        const callback = vi.fn();
        addDisposingListener(el, () => callback());
        invokeDisposingListeners(el);
        el.dispatchEvent(new Event("disposing"));
        expect(callback).toHaveBeenCalledOnce();
    });

    it("should not fail if there are no disposing listeners", () => {
        expect(() => invokeDisposingListeners(el)).not.toThrow();
    });

    it("should not fail if the target is not an EventTarget", () => {
        expect(() => invokeDisposingListeners({} as any)).not.toThrow();
    });

    it("removes global listener", () => {
        const listener1 = vi.fn();
        const listener2 = vi.fn();
        addDisposingListener(el, listener1);
        addDisposingListener(el, listener2);
        expect(el.addEventListener).toHaveBeenCalledOnce();
        invokeDisposingListeners(el);
        expect(el.removeEventListener).toHaveBeenCalledWith("disposing", (el.addEventListener as any).mock.calls[0][1]);
    });

    it("does not try to remove global listener if no listeners", () => {
        invokeDisposingListeners(el);
        expect(el.removeEventListener).not.toHaveBeenCalled();
    });

    it("still calls other listeners if one throws", () => {
        const listener1 = vi.fn(() => { throw new Error("Test error"); });
        const listener2 = vi.fn();
        addDisposingListener(el, listener1);
        addDisposingListener(el, listener2);
        expect(() => invokeDisposingListeners(el)).not.toThrow();
        expect(listener2).toHaveBeenCalledOnce();
    });

    it("does not stack overflow if disposing event is re-dispatched", () => {
        const listener1 = vi.fn(() => {
            invokeDisposingListeners(el);
        });
        const listener2 = vi.fn();
        addDisposingListener(el, listener1);
        addDisposingListener(el, listener2);
        expect(() => invokeDisposingListeners(el)).not.toThrow();
        expect(listener2).toHaveBeenCalledOnce();
    });

    it("still removes global listener if one listener throws", () => {
        const listener1 = vi.fn(() => { throw new Error("Test error"); });
        const listener2 = vi.fn();
        addDisposingListener(el, listener1);
        addDisposingListener(el, listener2);
        expect(el.addEventListener).toHaveBeenCalledOnce();
        invokeDisposingListeners(el);
        expect(el.removeEventListener).toHaveBeenCalledWith("disposing", (el.addEventListener as any).mock.calls[0][1]);
    });

    it("invokes listeners on descendants when opt.descendants is true", () => {
        const parent = document.createElement("div");
        const child = document.createElement("span");
        parent.appendChild(child);

        const parentListener = vi.fn();
        const childListener = vi.fn();

        addDisposingListener(parent, parentListener);
        addDisposingListener(child, childListener);

        invokeDisposingListeners(parent, { descendants: true });

        expect(childListener).toHaveBeenCalledOnce();
        expect(parentListener).toHaveBeenCalledOnce();
    });

    it("excludes self when opt.excludeSelf is true", () => {
        const listener = vi.fn();
        addDisposingListener(el, listener);
        invokeDisposingListeners(el, { excludeSelf: true });
        expect(listener).not.toHaveBeenCalled();
    });

    it("does not invoke descendants if node has no children", () => {
        const noChildEl = document.createElement("div");
        const listener = vi.fn();
        addDisposingListener(noChildEl, listener);
        invokeDisposingListeners(noChildEl, { descendants: true });
        expect(listener).toHaveBeenCalledOnce();
        // No descendants, so only self
    });

    it("does not invoke descendants if node is not an Element", () => {
        const nonElementTarget = new EventTarget();
        const listener = vi.fn();
        addDisposingListener(nonElementTarget, listener);
        invokeDisposingListeners(nonElementTarget, { descendants: true });
        expect(listener).toHaveBeenCalledOnce();
        // Not an Element, so no descendants logic
    });
});

describe("dispatchDisposingEvent", () => {
    it("dispatches a disposing event on the target", () => {
        const dispatchSpy = vi.spyOn(el, "dispatchEvent");
        dispatchDisposingEvent(el);
        expect(dispatchSpy).toHaveBeenCalledWith(expect.any(CustomEvent));
        const event = dispatchSpy.mock.calls[0][0] as CustomEvent;
        expect(event.type).toBe("disposing");
        expect(event.bubbles).toBe(false);
        expect(event.cancelable).toBe(false);
    });

    it("dispatches with custom options", () => {
        const dispatchSpy = vi.spyOn(el, "dispatchEvent");
        dispatchDisposingEvent(el, { bubbles: true, cancelable: true });
        expect(dispatchSpy).toHaveBeenCalledWith(expect.any(CustomEvent));
        const event = dispatchSpy.mock.calls[0][0] as CustomEvent;
        expect(event.bubbles).toBe(true);
        expect(event.cancelable).toBe(true);
    });

    it("does nothing if target is null", () => {
        expect(() => dispatchDisposingEvent(null as any)).not.toThrow();
    });

    it("does nothing if CustomEvent is not available", () => {
        const originalCustomEvent = globalThis.CustomEvent;
        delete (globalThis as any).CustomEvent;
        expect(() => dispatchDisposingEvent(el)).not.toThrow();
        globalThis.CustomEvent = originalCustomEvent;
    });
});

describe("currentLifecycleRoot", () => {
    it("returns null when no root is set", () => {
        expect(currentLifecycleRoot()).toBeNull();
    });

    it("sets and returns the lifecycle root", () => {
        const root = document.createElement("div");
        const result = currentLifecycleRoot(root);
        expect(result).toBeNull(); // previous was null
        expect(currentLifecycleRoot()).toBe(root);
    });

    it("returns the previous root when setting a new one", () => {
        const root1 = document.createElement("div");
        const root2 = document.createElement("div");
        currentLifecycleRoot(root1);
        const result = currentLifecycleRoot(root2);
        expect(result).toBe(root1);
        expect(currentLifecycleRoot()).toBe(root2);
    });

    it("sets to null when passed null", () => {
        const root = document.createElement("div");
        currentLifecycleRoot(root);
        currentLifecycleRoot(null as any);
        expect(currentLifecycleRoot()).toBeNull();
    });
});