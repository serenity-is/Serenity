
import { addDisposingListener, invokeDisposingListeners } from "../src/disposing-listener";
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

    it("appends iterable (Set) children", () => {
        const child1 = document.createElement("span");
        const child2 = document.createElement("b");
        appendChildren(container, new Set([child1, child2]) as any);
        expect(container.children.length).toBe(2);
        expect(container.contains(child1)).toBe(true);
        expect(container.contains(child2)).toBe(true);
    });

    it("does not throw for non-iterable array-like children", () => {
        expect(() => appendChildren(container, { length: 2 } as any)).not.toThrow();
        expect(container.childNodes.length).toBe(0);
    });

    it("expands a signal value that is an array and replaces the range on change", () => {
        const sig = mockSignal<any>(["a", "b"]);
        appendChildren(container, sig as any);
        expect(container.textContent).toBe("ab");
        expect(Array.from(container.childNodes).filter(n => n.nodeType === Node.TEXT_NODE).length).toBe(2);

        sig.value = ["c", "d", "e"];
        expect(container.textContent).toBe("cde");

        sig.value = "text";
        expect(container.textContent).toBe("text");

        sig.value = ["x", "y"];
        expect(container.textContent).toBe("xy");
    });

    it("expands a signal value that is a Set", () => {
        const sig = mockSignal<any>(new Set(["x", "y"]));
        appendChildren(container, sig as any);
        expect(container.textContent).toBe("xy");
    });

    it("expands a signal value that is an array of nodes", () => {
        const span = document.createElement("span");
        const b = document.createElement("b");
        const sig = mockSignal<any>([span, b]);
        appendChildren(container, sig as any);
        expect(container.contains(span)).toBe(true);
        expect(container.contains(b)).toBe(true);

        sig.value = [b];
        expect(container.contains(span)).toBe(false);
        expect(container.contains(b)).toBe(true);
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

describe("appendChildren signal child replacement disposal", () => {
    it("does not dispose caller-owned element values when replaced", () => {
        const inner = mockSignal("old");
        const reusable = <span>{inner}</span>;
        const host = document.createElement("div");
        const shown = mockSignal<any>(reusable);
        appendChildren(host, shown);
        expect(inner.listeners).toHaveLength(1);

        shown.value = <span>new</span>;
        expect(inner.listeners).toHaveLength(1); // caller-owned, kept alive
        expect(host.textContent).toBe("new");

        // reusing the instance later still works
        shown.value = reusable;
        expect(host.textContent).toBe("old");
        inner.value = "updated";
        expect(host.textContent).toBe("updated");
    });

    it("disposes a fragment range's subscriptions when replaced", () => {
        const inner = mockSignal("old");
        const fragment = document.createDocumentFragment();
        fragment.append(<b>{inner}</b>, document.createElement("i"));
        const host = document.createElement("div");
        const shown = mockSignal<any>(fragment);
        appendChildren(host, shown);
        expect(inner.listeners).toHaveLength(1);

        shown.value = "plain";
        expect(inner.listeners).toHaveLength(0);
        expect(host.textContent).toBe("plain");
    });

    it("keeps the signal observer alive across its own replacements", () => {
        const shown = mockSignal<any>("a");
        appendChildren(container, shown);
        shown.value = "b";
        shown.value = "c";
        expect(container.textContent).toBe("c");
    });

    it("matches a fragment end placeholder by identity, not text", () => {
        const shown = mockSignal<any>(null);
        const host = document.createElement("div");
        appendChildren(host, shown);

        const fragment = document.createDocumentFragment();
        fragment.append(document.createElement("b"));
        shown.value = fragment;

        const start = host.firstChild as Comment;
        const realEnd = host.lastChild as Comment;
        expect(start.data.startsWith("__domwisefrag_")).toBe(true);

        // decoy with identical text placed before the real end
        host.insertBefore(document.createComment(start.data), realEnd);

        shown.value = "done";
        expect(host.textContent).toBe("done");
        expect(host.childNodes.length).toBe(1);
    });

    it("keeps a caller-owned element's bindings alive across replacements", () => {
        const parent = document.createElement("div");
        const cls = mockSignal("c1");
        const a = <span class={cls}>A</span> as HTMLSpanElement;
        const b = document.createElement("span"); b.textContent = "B";
        // widget-teardown shape: disposing listener detaches its own node
        addDisposingListener(a, () => a.parentNode?.removeChild(a));

        const which = mockSignal<any>(a);
        appendChildren(parent, which);
        expect(parent.innerHTML).toBe('<span class="c1">A</span>');

        which.value = b;
        expect(parent.innerHTML).toBe("<span>B</span>");

        // the binding stays live even while the element is detached
        cls.value = "c1b";
        expect(a.className).toBe("c1b");

        which.value = a;
        expect(parent.innerHTML).toBe('<span class="c1b">A</span>');
    });
});

describe("appendChildren fragment disposal", () => {
    it("disposes a signal child rendered inside a fragment", () => {
        const s = mockSignal("a");
        const host = <div><>{s}</></div>;
        document.body.appendChild(host);
        expect(host.textContent).toBe("a");

        invokeDisposingListeners(host, { descendants: true });
        expect(s.listeners.length).toBe(0);
        s.value = "b";
        expect(host.textContent).toBe("a");
    });

    it("disposes a signal child inside a fragment with siblings", () => {
        const s = mockSignal("a");
        const host = <div>{<><i>x</i>{s}</>}</div>;
        document.body.appendChild(host);
        invokeDisposingListeners(host, { descendants: true });
        expect(s.listeners.length).toBe(0);
    });

    it("disposes an inner signal of a signal-valued collection in a fragment", () => {
        const inner = mockSignal("v");
        const outer = mockSignal<any>(["pre", inner]);
        const host = <div><>{outer}</></div>;
        document.body.appendChild(host);
        invokeDisposingListeners(host, { descendants: true });
        expect(inner.listeners.length).toBe(0);
        expect(outer.listeners.length).toBe(0);
    });
});
