import { addListener, disposeElement, triggerEvent } from "./fluent-events";

beforeEach(() => {
    jest.restoreAllMocks();
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
        disposeElement(null);
    });

    it("ignores if element has no attached handlers", () => {
        disposeElement(element);
    });

    it("clears any events other than disposing", () => {
        const test = jest.fn();
        addListener(element, "test", test); 
        disposeElement(element);
        expect(test).not.toHaveBeenCalled();
        element.dispatchEvent(new Event("test"));
        expect(test).not.toHaveBeenCalled();
    });

    it("can't clear externally attached events", () => {
        const test = jest.fn();
        element.addEventListener("test", test); 
        disposeElement(element);
        expect(test).not.toHaveBeenCalled();
        element.dispatchEvent(new Event("test"));
        expect(test).toHaveBeenCalled();
    });

    it("calls disposing handlers", () => {
        const disposing1 = jest.fn();
        const disposing2 = jest.fn();
        addListener(element, "disposing", disposing1); 
        addListener(element, "disposing", disposing2); 
        disposeElement(element);
        expect(disposing1).toHaveBeenCalled();
        expect(disposing2).toHaveBeenCalled();
    });

    it("does not trigger externally attached disposing event", () => {
        const disposing1 = jest.fn();
        const disposing2 = jest.fn();
        addListener(element, "disposing", disposing1); 
        element.addEventListener("disposing", disposing2); 
        disposeElement(element);
        expect(disposing1).toHaveBeenCalled();
        expect(disposing2).not.toHaveBeenCalled();
    });

});

describe("triggerEvent", () => {
    it("triggers event", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = jest.fn();
        addListener(element, "test", test); 
        triggerEvent(element, "test");
        expect(test).toHaveBeenCalled();
    });

    it("adds can set custom props", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = jest.fn();
        addListener(element, "test", test); 
        triggerEvent(element, "test",  { custom: 5});
        expect(test).toHaveBeenCalled();
        expect(test.mock.calls[0][0].custom).toBe(5);
    });

    it("bubbles true by default", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = jest.fn();
        addListener(element, "test", test); 
        triggerEvent(element, "test");
        expect(test).toHaveBeenCalled();
        expect(test.mock.calls[0][0].bubbles).toBe(true);
    });

    it("can set bubbles", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = jest.fn();
        addListener(element, "test", test); 
        triggerEvent(element, "test",  { bubbles: false });
        expect(test).toHaveBeenCalled();
        expect(test.mock.calls[0][0].bubbles).toBe(false);
    });

    it("cancelable true by default", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = jest.fn();
        addListener(element, "test", test); 
        triggerEvent(element, "test");
        expect(test).toHaveBeenCalled();
        expect(test.mock.calls[0][0].cancelable).toBe(true);
    });

    it("can set cancelable", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = jest.fn();
        addListener(element, "test", test); 
        triggerEvent(element, "test",  { cancelable: false });
        expect(test).toHaveBeenCalled();
        expect(test.mock.calls[0][0].cancelable).toBe(false);
    });

    it("can use event delegation", () => {
        const element = document.createElement("div");
        document.body.appendChild(element);

        const test = jest.fn();
        addListener(document.body, "test", "div", test); 
        triggerEvent(element, "test");
        expect(test).toHaveBeenCalled();
    });

});