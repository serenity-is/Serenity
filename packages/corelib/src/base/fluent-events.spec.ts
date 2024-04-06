import { addListener, triggerRemoveAndClearAll } from "./fluent-events";

beforeEach(() => {
    jest.restoreAllMocks();
});

describe("triggerRemoveAndClearAll", () => {
    let element: HTMLElement;
    beforeEach(() => {
        element = document.createElement("div");
        document.body.appendChild(element);
    });

    afterEach(() => {
        element.remove();
    });

    it("ignores if element is null", () => {
        triggerRemoveAndClearAll(null);
    });

    it("ignores if element has no attached handlers", () => {
        triggerRemoveAndClearAll(element);
    });

    it("clears any events other than remove", () => {
        const test = jest.fn();
        addListener(element, "test", test); 
        triggerRemoveAndClearAll(element);
        expect(test).not.toHaveBeenCalled();
        element.dispatchEvent(new Event("test"));
        expect(test).not.toHaveBeenCalled();
    });

    it("can't clear externally attached events", () => {
        const test = jest.fn();
        element.addEventListener("test", test); 
        triggerRemoveAndClearAll(element);
        expect(test).not.toHaveBeenCalled();
        element.dispatchEvent(new Event("test"));
        expect(test).toHaveBeenCalled();
    });

    it("calls remove handlers", () => {
        const remove1 = jest.fn();
        const remove2 = jest.fn();
        addListener(element, "remove", remove1); 
        addListener(element, "remove", remove2); 
        triggerRemoveAndClearAll(element);
        expect(remove1).toHaveBeenCalled();
        expect(remove2).toHaveBeenCalled();
    });

    it("does not trigger externally attached remove event", () => {
        const remove1 = jest.fn();
        const remove2 = jest.fn();
        addListener(element, "remove", remove1); 
        element.addEventListener("remove", remove2); 
        triggerRemoveAndClearAll(element);
        expect(remove1).toHaveBeenCalled();
        expect(remove2).not.toHaveBeenCalled();
    });

});