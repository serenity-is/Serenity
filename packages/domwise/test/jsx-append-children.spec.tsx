
import { appendChildren } from "../src/jsx-append-children";
import { mockSignal } from "./mocks/mock-signal";

let container: HTMLElement;

beforeEach(() => {
    container = document.createElement("div");
    vi.clearAllMocks();
});

describe("appendChildren", () => {
    it("appends child elements to the container", () => {
        const child1 = document.createElement("span");
        const child2 = document.createElement("span");
        appendChildren(container, [child1, child2]);
        expect(container.children.length).toBe(2);
        expect(container.contains(child1)).toBe(true);
        expect(container.contains(child2)).toBe(true);
    });

    it("appends string children as text nodes", () => {
        appendChildren(container, "hello world");
        expect(container.textContent).toBe("hello world");
        expect(container.childNodes.length).toBe(1);
        expect(container.childNodes[0]).toBeInstanceOf(Text);
    });

    it("appends number children as text nodes", () => {
        appendChildren(container, 42);
        expect(container.textContent).toBe("42");
        expect(container.childNodes.length).toBe(1);
        expect(container.childNodes[0]).toBeInstanceOf(Text);
    });

    it("appends zero as text node", () => {
        appendChildren(container, 0);
        expect(container.textContent).toBe("0");
    });

    it("ignores null children", () => {
        appendChildren(container, null);
        expect(container.childNodes.length).toBe(0);
    });

    it("ignores undefined children", () => {
        appendChildren(container, undefined);
        expect(container.childNodes.length).toBe(0);
    });

    it("ignores false children", () => {
        appendChildren(container, false);
        expect(container.childNodes.length).toBe(0);
    });

    it("ignores true children", () => {
        appendChildren(container, true);
        expect(container.childNodes.length).toBe(0);
    });

    it("handles arrays of mixed children", () => {
        const element = document.createElement("div");
        appendChildren(container, ["text", 123, element, null, false]);
        expect(container.childNodes.length).toBe(3);
        expect(container.childNodes[0].textContent).toBe("text");
        expect(container.childNodes[1].textContent).toBe("123");
        expect(container.childNodes[2]).toBe(element);
    });

    it("handles nested arrays", () => {
        appendChildren(container, [["a", "b"], ["c", "d"]]);
        expect(container.textContent).toBe("abcd");
        expect(container.childNodes.length).toBe(4);
    });

    it("handles DocumentFragment", () => {
        const fragment = document.createDocumentFragment();
        fragment.appendChild(document.createTextNode("frag1"));
        fragment.appendChild(document.createTextNode("frag2"));
        appendChildren(container, fragment);
        expect(container.textContent).toBe("frag1frag2");
        expect(container.childNodes.length).toBe(2);
    });

    it("handles Text nodes", () => {
        const textNode = document.createTextNode("direct text");
        appendChildren(container, textNode);
        expect(container.textContent).toBe("direct text");
        expect(container.childNodes[0]).toBe(textNode);
    });

    it("handles Comment nodes", () => {
        const comment = document.createComment("test comment");
        appendChildren(container, comment);
        expect(container.childNodes[0]).toBe(comment);
    });

    it("handles NodeList", () => {
        const div = document.createElement("div");
        div.innerHTML = "<span>1</span><span>2</span>";
        appendChildren(container, div.childNodes);
        expect(container.children.length).toBe(2);
        expect(container.children[0].tagName).toBe("SPAN");
        expect(container.children[1].tagName).toBe("SPAN");
    });

    it("handles HTMLCollection", () => {
        const div = document.createElement("div");
        div.innerHTML = "<span>1</span><span>2</span>";
        appendChildren(container, div.children);
        expect(container.children.length).toBe(2);
        expect(container.children[0].tagName).toBe("SPAN");
        expect(container.children[1].tagName).toBe("SPAN");
    });

    it("handles signal children", () => {
        const signal = mockSignal("initial");
        appendChildren(container, signal);
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(container.textContent).toBe("initial");
        
        signal.value = "updated";
        expect(container.textContent).toBe("updated");
    });

    it("handles signal with element", () => {
        const element1 = document.createElement("div");
        element1.textContent = "first";
        const element2 = document.createElement("div");
        element2.textContent = "second";
        
        const signal = mockSignal(element1);
        appendChildren(container, signal);
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(container.children[0]).toBe(element1);
        
        signal.value = element2;
        expect(container.children[0]).toBe(element2);
        expect(container.contains(element1)).toBe(false);
    });

    it("handles signal with fragment", () => {
        const fragment = document.createDocumentFragment();
        fragment.appendChild(document.createTextNode("frag text"));
        
        const signal = mockSignal(fragment);
        appendChildren(container, signal);
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(container.textContent).toBe("frag text");
    });

    it("replaces children when signal value changes from string to string", () => {
        const signal = mockSignal("initial");
        appendChildren(container, signal);
        expect(container.textContent).toBe("initial");
        
        signal.value = "updated";
        expect(container.textContent).toBe("updated");
        expect(container.childNodes.length).toBe(1);
        expect(container.childNodes[0]).toBeInstanceOf(Text);
    });

    it("replaces children when signal value changes from element to element", () => {
        const element1 = document.createElement("div");
        element1.textContent = "first";
        const element2 = document.createElement("div");
        element2.textContent = "second";
        
        const signal = mockSignal(element1);
        appendChildren(container, signal);
        expect(container.children[0]).toBe(element1);
        expect(container.textContent).toBe("first");
        
        signal.value = element2;
        expect(container.children[0]).toBe(element2);
        expect(container.textContent).toBe("second");
        expect(container.contains(element1)).toBe(false);
    });

    it("replaces children when signal value changes from string to element", () => {
        const signal = mockSignal<any>("text content");
        appendChildren(container, signal);
        expect(container.textContent).toBe("text content");
        
        const element = document.createElement("span");
        element.textContent = "element content";
        signal.value = element;
        expect(container.children[0]).toBe(element);
        expect(container.textContent).toBe("element content");
    });

    it("replaces children when signal value changes from element to string", () => {
        const element = document.createElement("span");
        element.textContent = "element content";
        const signal = mockSignal<any>(element);
        appendChildren(container, signal);
        expect(container.children[0]).toBe(element);
        
        signal.value = "text content";
        expect(container.textContent).toBe("text content");
        expect(container.children.length).toBe(0);
        expect(container.contains(element)).toBe(false);
    });

    it("replaces children when signal value changes to fragment", () => {
        const signal = mockSignal<any>("initial text");
        appendChildren(container, signal);
        expect(container.textContent).toBe("initial text");
        
        const fragment = document.createDocumentFragment();
        fragment.appendChild(document.createTextNode("frag"));
        fragment.appendChild(document.createElement("br"));
        fragment.appendChild(document.createTextNode("ment"));
        
        signal.value = fragment;
        expect(container.textContent).toBe("fragment");
        expect(container.children.length).toBe(1); // the br element
        // Fragment gets placeholder comments added, so 3 original + 2 comments = 5 nodes
        expect(container.childNodes.length).toBe(5);
    });

    it("replaces children when signal value changes from fragment to string", () => {
        const fragment = document.createDocumentFragment();
        fragment.appendChild(document.createTextNode("initial"));
        fragment.appendChild(document.createElement("br"));
        
        const signal = mockSignal<any>(fragment);
        appendChildren(container, signal);
        expect(container.textContent).toBe("initial");
        expect(container.children.length).toBe(1); // the br
        
        signal.value = "new text";
        expect(container.textContent).toBe("new text");
        expect(container.children.length).toBe(0);
        expect(container.childNodes.length).toBe(1);
    });

    it("replaces children when signal value changes between fragments", () => {
        const fragment1 = document.createDocumentFragment();
        fragment1.appendChild(document.createTextNode("first"));
        const element1 = document.createElement("div");
        element1.textContent = "element1";
        fragment1.appendChild(element1);
        
        const signal = mockSignal<any>(fragment1);
        appendChildren(container, signal);
        expect(container.textContent).toBe("firstelement1");
        expect(container.children[0]).toBe(element1);
        
        const fragment2 = document.createDocumentFragment();
        fragment2.appendChild(document.createTextNode("second"));
        const element2 = document.createElement("span");
        element2.textContent = "element2";
        fragment2.appendChild(element2);
        
        signal.value = fragment2;
        expect(container.textContent).toBe("secondelement2");
        expect(container.children[0]).toBe(element2);
        expect(container.contains(element1)).toBe(false);
    });

    it("handles multiple signal value changes", () => {
        const signal = mockSignal<any>("first");
        appendChildren(container, signal);
        expect(container.textContent).toBe("first");
        
        signal.value = "second";
        expect(container.textContent).toBe("second");
        
        const element = document.createElement("div");
        element.textContent = "third";
        signal.value = element;
        expect(container.children[0]).toBe(element);
        expect(container.textContent).toBe("third");
        
        signal.value = "fourth";
        expect(container.textContent).toBe("fourth");
        expect(container.children.length).toBe(0);
    });

    it("handles signal changing to null/undefined", () => {
        const signal = mockSignal<any>("initial");
        appendChildren(container, signal);
        expect(container.textContent).toBe("initial");
        
        signal.value = null;
        // null creates a comment node for empty content
        expect(container.childNodes.length).toBe(1);
        expect(container.childNodes[0]).toBeInstanceOf(Comment);
        expect(container.textContent).toBe("");
        
        signal.value = "back";
        expect(container.textContent).toBe("back");
    });

    it("handles signal changing to true", () => {
        const signal = mockSignal<any>("initial");
        appendChildren(container, signal);
        expect(container.textContent).toBe("initial");

        signal.value = "test";
        expect(container.textContent).toBe("test");
        
        signal.value = true;
        // true creates a comment node for empty content
        expect(container.childNodes.length).toBe(1);
        expect(container.childNodes[0]).toBeInstanceOf(Comment);
        expect(container.textContent).toBe("");

        signal.value = "back";
        expect(container.textContent).toBe("back");
    });

    it("handles signal changing to false", () => {
        const signal = mockSignal<any>("initial");
        appendChildren(container, signal);
        expect(container.textContent).toBe("initial");

        signal.value = "test";
        expect(container.textContent).toBe("test");

        signal.value = false;
        
        // false creates a comment node for empty content
        expect(container.childNodes.length).toBe(1);
        expect(container.childNodes[0]).toBeInstanceOf(Comment);
        expect(container.textContent).toBe("");

        signal.value = "back";
        expect(container.textContent).toBe("back");
    });

    it("handles signal changing to number", () => {
        const signal = mockSignal<any>("initial");
        appendChildren(container, signal);
        expect(container.textContent).toBe("initial");

        signal.value = "test";
        expect(container.textContent).toBe("test");

        signal.value = 25;

        expect(container.textContent).toBe("25");

        signal.value = "back";
        expect(container.textContent).toBe("back");
    });

    it("handles empty array", () => {
        appendChildren(container, []);
        expect(container.childNodes.length).toBe(0);
    });

    it("handles array with only null/undefined/false", () => {
        appendChildren(container, [null, undefined, false]);
        expect(container.childNodes.length).toBe(0);
    });

});