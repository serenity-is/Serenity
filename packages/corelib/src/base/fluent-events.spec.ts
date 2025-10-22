import { addDisposingListener, invokeDisposingListeners, removeDisposingListener } from "@serenity-is/sleekdom";
import { addListener, notifyDisposingNode, getEventRegistry, triggerEvent } from "./fluent-events";

vi.mock(import("@serenity-is/sleekdom"), async () => {
    return {
        invokeDisposingListeners: vi.fn(),
        addDisposingListener: vi.fn(),
        removeDisposingListener: vi.fn()
    }
});

beforeEach(() => {
    vi.restoreAllMocks();
});

describe("disposeElement", () => {
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
        expect(invokeDisposingListeners).toHaveBeenCalledExactlyOnceWith(element);
    });

    it("does not trigger externally attached disposing event", () => {
        const disposing1 = vi.fn();
        const disposing2 = vi.fn();
        addListener(element, "disposing", disposing1);
        element.addEventListener("disposing", disposing2);
        notifyDisposingNode(element);
        expect(invokeDisposingListeners).toHaveBeenCalledExactlyOnceWith(element);
        expect(disposing2).not.toHaveBeenCalled();
    });
    
    it("removes all element listeners from registry after disposing", () => {
        const test = vi.fn();
        addListener(element, "test", test);
        const removeEventListener = vi.spyOn(element, "removeEventListener");
        const disposing1 = vi.fn();
        addListener(element, "disposing", disposing1);
        notifyDisposingNode(element);
        expect(invokeDisposingListeners).toHaveBeenCalledExactlyOnceWith(element);
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