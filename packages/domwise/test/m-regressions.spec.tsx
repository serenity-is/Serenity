import { usePropBinding } from "../src";
import { addDisposingListener, dispatchDisposingEvent, invokeDisposingListeners, withLifecycleRoot } from "../src/disposing-listener";
import { appendChildren } from "../src/jsx-append-children";
import { assignClass } from "../src/jsx-assign-class";
import { assignProp, assignProps } from "../src/jsx-assign-props";
import { createElement } from "../src/compat-api";
import { derivedSignal, observeSignal } from "../src/signal-util";
import { signal } from "../src/signals";
import { Show } from "../src/show";
import { mockSignal } from "./mocks/mock-signal";

describe("M3: input value is a property", () => {
    it("follows the signal after the user edits the field and through null", () => {
        const v = mockSignal<any>("x");
        const input = <input value={v} /> as HTMLInputElement;
        document.body.appendChild(input);
        expect(input.value).toBe("x");

        input.value = "typed";
        v.value = "y";
        expect(input.value).toBe("y");

        v.value = null;
        expect(input.value).toBe("");
    });

    it("does not clear the field for a null value with no previous prop", () => {
        const input = document.createElement("input");
        input.value = "test value";
        assignProp(input, "value", null); // prev undefined -> no-op
        expect(input.value).toBe("test value");
    });

    it("reflects the initial input value as an attribute for serialization", () => {
        const input = <input value="18" /> as HTMLInputElement;
        expect(input.value).toBe("18");
        expect(input.getAttribute("value")).toBe("18");
        expect(input.outerHTML).toContain('value="18"');
    });
});

describe("M4: select value prop hook", () => {
    it("applies the prop-hook value after options exist", () => {
        const binding = usePropBinding("b");
        const select = <select value={binding}><option value="a">a</option><option value="b">b</option></select> as HTMLSelectElement;
        expect(select.value).toBe("b");
    });
});

describe("M8: class values", () => {
    it("accepts a non-array iterable", () => {
        const el = document.createElement("div");
        assignClass(el, new Set(["a", "b"]));
        expect(el.classList.contains("a")).toBe(true);
        expect(el.classList.contains("b")).toBe(true);
    });

    it("accepts multi-line whitespace-separated strings", () => {
        const el = document.createElement("div");
        assignClass(el, "btn\n  btn-primary");
        expect(el.classList.contains("btn")).toBe(true);
        expect(el.classList.contains("btn-primary")).toBe(true);
    });
});

describe("M9: createElement children normalization", () => {
    it("passes a lone child as the child itself", () => {
        let received: any;
        function Comp(props: any) { received = props.children; return <div />; }
        createElement(Comp, null, createElement("span", null, "yes"));
        expect((received as Element).tagName).toBe("SPAN");
    });

    it("passes no children as undefined", () => {
        let received: any = "sentinel";
        function Comp(props: any) { received = props.children; return <div />; }
        createElement(Comp, null);
        expect(received).toBeUndefined();
    });
});

describe("M10: fragment placeholder identity", () => {
    it("matches the end placeholder by identity, not text", () => {
        const sig = mockSignal<any>(null);
        const host = document.createElement("div");
        appendChildren(host, sig);

        const fragment = document.createDocumentFragment();
        fragment.append(document.createElement("b"));
        sig.value = fragment;

        const start = host.firstChild as Comment;
        const realEnd = host.lastChild as Comment;
        expect(start.data.startsWith("__domwisefrag_")).toBe(true);

        // decoy with identical text placed before the real end
        host.insertBefore(document.createComment(start.data), realEnd);

        sig.value = "done";
        expect(host.textContent).toBe("done");
        expect(host.childNodes.length).toBe(1);
    });
});

describe("M11: derivedSignal over a PrimitiveComputed", () => {
    it("keeps updating through a chained derived signal", () => {
        const src = mockSignal(1);
        const d1 = derivedSignal(src, v => v * 2);
        const d2 = derivedSignal(d1 as any, v => v + 1);
        expect(d1.value).toBe(2);
        expect(d2.value).toBe(3);

        src.value = 5;
        expect(d1.value).toBe(10);
        expect(d2.value).toBe(11);
    });
});

describe("M12: useLifecycleRoot", () => {
    it("registers the subscription disposer on the lifecycle root", () => {
        const root = document.createElement("div");
        const sig = signal(1);
        const cb = vi.fn();
        withLifecycleRoot(root, () => {
            observeSignal(sig, cb, { useLifecycleRoot: true });
        });
        cb.mockClear();

        sig.value = 2;
        expect(cb).toHaveBeenCalledTimes(1);

        invokeDisposingListeners(root);
        cb.mockClear();
        sig.value = 3;
        expect(cb).not.toHaveBeenCalled();
    });
});

describe("M16: bubbling disposing events", () => {
    it("does not disarm an ancestor listener", () => {
        const parent = document.createElement("div");
        const child = document.createElement("span");
        parent.appendChild(child);

        const cb = vi.fn();
        addDisposingListener(parent, cb);

        dispatchDisposingEvent(child, { bubbles: true });
        expect(cb).not.toHaveBeenCalled();

        dispatchDisposingEvent(parent);
        expect(cb).toHaveBeenCalledTimes(1);
    });
});

describe("M17: descendant walks", () => {
    it("reaches nodes inside a shadow root", () => {
        const host = document.createElement("div");
        const shadow = host.attachShadow({ mode: "open" });
        const inner = document.createElement("span");
        shadow.appendChild(inner);

        const cb = vi.fn();
        addDisposingListener(inner, cb);
        invokeDisposingListeners(host, { descendants: true });
        expect(cb).toHaveBeenCalledTimes(1);
    });

    it("reaches nodes inside template content", () => {
        const template = document.createElement("template");
        const inner = document.createElement("span");
        template.content.appendChild(inner);

        const cb = vi.fn();
        addDisposingListener(inner, cb);
        invokeDisposingListeners(template, { descendants: true });
        expect(cb).toHaveBeenCalledTimes(1);
    });
});

describe("M18: Show primitive branch hidden disposal", () => {
    it("disposes the hidden plain branch when a primitive branch is shown", () => {
        const inner = mockSignal("x");
        const when = mockSignal<boolean>(true);
        const host = <div><Show when={when} fallback="Please sign in"><span>{inner}</span></Show></div>;
        document.body.appendChild(host);
        expect(inner.listeners).toHaveLength(1);

        when.value = false;
        expect(inner.listeners).toHaveLength(1); // hidden, not disposed on switch

        invokeDisposingListeners(host, { descendants: true });
        expect(inner.listeners).toHaveLength(0);
    });
});

describe("M19: checked property", () => {
    it("keeps following the signal after the user toggles the checkbox", () => {
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
});

describe("M14/M15: style arrays, numbers and custom properties", () => {
    it("merges style arrays left-to-right", () => {
        const el = <div style={[{ color: "red", fontSize: "12px" }, { fontSize: "14px" }]} /> as HTMLElement;
        expect(el.style.color).toBe("red");
        expect(el.style.fontSize).toBe("14px");
    });

    it("accepts numbers and CSS custom properties", () => {
        const el = <div style={{ opacity: 0.5, "--my-color": "blue" }} /> as HTMLElement;
        expect(el.style.opacity).toBe("0.5");
        expect(el.style.getPropertyValue("--my-color")).toBe("blue");
    });

    it("accepts a signal-valued number", () => {
        const size = signal(16);
        const el = <div style={{ fontSize: size }} /> as HTMLElement;
        expect(el.style.fontSize).toBe("16px");
        size.value = 20;
        expect(el.style.fontSize).toBe("20px");
    });
});

describe("L15/L16/L18/L19", () => {
    it("L15: removes on-map listeners when the element is disposed", () => {
        const el = document.createElement("div");
        const handler = vi.fn();
        assignProps(el, { on: { click: handler } });
        el.dispatchEvent(new Event("click"));
        expect(handler).toHaveBeenCalledTimes(1);

        invokeDisposingListeners(el);
        el.dispatchEvent(new Event("click"));
        expect(handler).toHaveBeenCalledTimes(1);
    });

    it("L16: repeated primitive switches leave a single node", () => {
        const when = mockSignal<boolean>(true);
        const host = <div><Show when={when} fallback="off">on</Show></div>;
        document.body.appendChild(host);
        for (let i = 0; i < 50; i++)
            when.value = !when.value;
        expect(host.childNodes.length).toBe(1);
    });

    it("L18: draggable false then null removes the attribute", () => {
        const el = document.createElement("div");
        const sig = mockSignal<any>("true");
        assignProps(el, { draggable: sig });
        sig.value = false;
        expect(el.getAttribute("draggable")).toBe("false");
        sig.value = null;
        expect(el.hasAttribute("draggable")).toBe(false);
    });

    it("L19: xlinkHref null removes the namespaced attribute", () => {
        const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        const sig = mockSignal<any>("#id");
        assignProps(svg, { xlinkHref: sig });
        expect(svg.getAttributeNS("http://www.w3.org/1999/xlink", "href")).toBe("#id");

        sig.value = null;
        expect(svg.getAttributeNS("http://www.w3.org/1999/xlink", "href")).toBeFalsy();
    });
});
