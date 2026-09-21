import { signal } from "@preact/signals-core";
import { addDisposingListener, invokeDisposingListeners } from "../src/disposing-listener";
import { appendChildren } from "../src/jsx-append-children";
import { bindThis } from "../src/bind-this";
import { assignStyle } from "../src/jsx-assign-style";
import { derivedSignal } from "../src/signal-util";
import { useClassList } from "../src/hooks";
import { ShadowRootNode } from "../src/shadow";
import { Show } from "../src/show";
import { mockSignal } from "./mocks/mock-signal";

describe("range regression: R1 fragment signal child disposal", () => {
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

describe("range regression: R2 derivedSignal single evaluation", () => {
    it("calls the transform exactly once at creation", () => {
        const src = signal(1);
        let calls = 0;
        const d = derivedSignal(src, v => { calls++; return v * 2; });
        expect(calls).toBe(1);
        expect(d.value).toBe(2);

        src.value = 2;
        expect(d.value).toBe(4);
        expect(calls).toBe(2);
    });

    it("invokes a Show factory child once at mount", () => {
        let calls = 0;
        const w = signal(true);
        const host = <div><Show when={w}>{() => { calls++; return <b>on</b>; }}</Show></div>;
        document.body.appendChild(host);
        expect(calls).toBe(1);
        expect(host.textContent).toBe("on");
    });
});

describe("range regression: R3 reassignment keeps derived signals alive", () => {
    it("does not dispose the derivedDisposer when style is re-assigned", () => {
        const n = document.createElement("div");
        const src = signal("red");
        const d = derivedSignal(src, v => v);
        assignStyle(n, { color: d });
        assignStyle(n, { color: d, fontSize: "10px" }, { color: d });
        src.value = "blue";
        expect(n.style.color).toBe("blue");
        expect(typeof (d as any).derivedDisposer).toBe("function");
    });
});

describe("range regression: R4 scoped owner re-bridged", () => {
    it("releases a style binding bound after the node was disposed", () => {
        const n = document.createElement("div");
        const s1 = signal("red");
        assignStyle(n, { color: s1 });
        invokeDisposingListeners(n);
        expect(n.style.color).toBe("red");

        const s2 = signal("green");
        assignStyle(n, { color: s2 });
        expect(n.style.color).toBe("green");
        invokeDisposingListeners(n);
        s2.value = "blue";
        expect(n.style.color).toBe("green");
    });
});

describe("range regression: R5 Show disposes iterable branches", () => {
    it("disposes a factory branch that returns a Set of nodes", () => {
        const w = signal(true);
        const el = document.createElement("b");
        const spy = vi.fn();
        addDisposingListener(el, spy);

        const host = <div><Show when={w}>{() => new Set([el])}</Show></div>;
        document.body.appendChild(host);
        expect(host.textContent).toBe("");

        invokeDisposingListeners(host, { descendants: true });
        expect(spy).toHaveBeenCalled();
    });
});

describe("range regression: R6 ShadowRootNode in signal collection", () => {
    it("does not throw when a ShadowRootNode has no host element", () => {
        const container = document.createElement("div");
        const s = signal<any>([ShadowRootNode({ mode: "open", children: "x" })]);
        expect(() => appendChildren(container, s)).not.toThrow();
    });
});

describe("range regression: R7 bindThis inherited accessors", () => {
    it("writes through an inherited accessor instead of shadowing it", () => {
        const el = document.createElement("div");
        let first = 0, second = 0;
        el.onclick = () => { first++; };
        const proxy = bindThis(el);
        void (proxy as any).onclick;
        el.onclick = () => { second++; };
        el.dispatchEvent(new MouseEvent("click"));
        expect(second).toBe(1);
        expect(first).toBe(0);
    });
});

describe("range regression: R8 defaultProps prototype collisions", () => {
    it("applies a default whose key collides with Object.prototype", () => {
        function Comp(props: any) {
            return <div>{String(props.toString)}</div>;
        }
        (Comp as any).defaultProps = { toString: "X" };
        const node = <Comp /> as unknown as HTMLElement;
        expect(node.textContent).toBe("X");
    });
});

describe("range regression: R9 useClassList.toggle", () => {
    it("returns a boolean", () => {
        const cls = useClassList("a");
        expect(cls.toggle("b")).toBe(true);
        expect(cls.toggle("b")).toBe(false);
    });
});
