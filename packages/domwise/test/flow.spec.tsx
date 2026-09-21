import type { SignalLike } from "#types";
import { describe, expect, it } from "vitest";
import { invokeDisposingListeners } from "../src/disposing-listener";
import { Show } from "../src/show";
import { isSignalLike } from "../src/signal-util";
import { mockSignal } from "./mocks/mock-signal";

describe("Show", () => {
    it("shows else content if when is false", () => {
        expect(<Show when={false} fallback={new Comment()}><div>Content</div></Show>).toBeInstanceOf(Comment);
    });

    it("shows content if when is true", () => {
        expect(<Show when={true} fallback={new Comment()}><div>Content</div></Show>).toBeInstanceOf(HTMLDivElement);
    });

    it("shows content if when is truthy", () => {
        expect(<Show when={"non-empty string"} fallback={new Comment()}><div>Content</div></Show>).toBeInstanceOf(HTMLDivElement);
        expect(<Show when={1} fallback={new Comment()}><div>Content</div></Show>).toBeInstanceOf(HTMLDivElement);
        expect(<Show when={{}} fallback={new Comment()}><div>Content</div></Show>).toBeInstanceOf(HTMLDivElement);
    });

    it("shows else content if when is falsy", () => {
        expect(<Show when={null} fallback={new Comment()}><div>Content</div></Show>).toBeInstanceOf(Comment);
        expect(<Show when={undefined} fallback={new Comment()}><div>Content</div></Show>).toBeInstanceOf(Comment);
        expect(<Show when={0} fallback={new Comment()}><div>Content</div></Show>).toBeInstanceOf(Comment);
        expect(<Show when={""} fallback={new Comment()}><div>Content</div></Show>).toBeInstanceOf(Comment);
    });

    it("shows else content if when is a signal with false value", () => {
        const result = <Show when={mockSignal(false)} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result)).toBe(true);
        expect((result as unknown as SignalLike<any>).value).toBeInstanceOf(Comment);
    });

    it("shows content if when is a signal with true value", () => {
        const result = <Show when={mockSignal(true)} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result)).toBe(true);
        expect((result as unknown as SignalLike<any>).value).toBeInstanceOf(HTMLDivElement);
    });

    it("shows content if when is a signal with truthy value", () => {
        const result1 = <Show when={mockSignal("non-empty string")} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result1)).toBe(true);
        expect((result1 as unknown as SignalLike<any>).value).toBeInstanceOf(HTMLDivElement);

        const result2 = <Show when={mockSignal(1)} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result2)).toBe(true);
        expect((result2 as unknown as SignalLike<any>).value).toBeInstanceOf(HTMLDivElement);

        const result3 = <Show when={mockSignal({})} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result3)).toBe(true);
        expect((result3 as unknown as SignalLike<any>).value).toBeInstanceOf(HTMLDivElement);
    });

    it("shows else content if when is a signal with falsy value", () => {
        const result1 = <Show when={mockSignal(null)} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result1)).toBe(true);
        expect((result1 as unknown as SignalLike<any>).value).toBeInstanceOf(Comment);

        const result2 = <Show when={mockSignal(undefined)} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result2)).toBe(true);
        expect((result2 as unknown as SignalLike<any>).value).toBeInstanceOf(Comment);

        const result3 = <Show when={mockSignal(0)} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result3)).toBe(true);
        expect((result3 as unknown as SignalLike<any>).value).toBeInstanceOf(Comment);

        const result4 = <Show when={mockSignal("")} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result4)).toBe(true);
        expect((result4 as unknown as SignalLike<any>).value).toBeInstanceOf(Comment);
    });

    it("updates content when signal changes", () => {
        const whenSignal = mockSignal<any>(false);
        const result = <Show when={whenSignal} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result)).toBe(true);
        const signalResult = result as unknown as SignalLike<any>;
        expect(signalResult.value).toBeInstanceOf(Comment);

        whenSignal.value = true;
        expect(signalResult.value).toBeInstanceOf(HTMLDivElement);

        whenSignal.value = 0;
        expect(signalResult.value).toBeInstanceOf(Comment);

        whenSignal.value = "hello";
        expect(signalResult.value).toBeInstanceOf(HTMLDivElement);
    });

    it("passes the resolved when value to function children", () => {
        const user = mockSignal<{ name: string } | null>({ name: "Alice" });
        let received: any;
        const result = <Show when={user}>{(u: any) => { received = u; return <b>{u?.name}</b>; }}</Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);

        expect(received).toEqual({ name: "Alice" });
        expect(host.textContent).toBe("Alice");

        user.value = { name: "Bob" };
        expect(received).toEqual({ name: "Bob" });
        expect(host.textContent).toBe("Bob");
    });

    it("passes the resolved when value to function fallback", () => {
        const user = mockSignal<string | null>(null);
        let received: any;
        const result = <Show when={user} fallback={(u: any) => { received = u; return <i>none</i>; }}><div>has</div></Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);

        expect(received).toBeNull();
        expect(host.textContent).toBe("none");
    });

    it("passes the static when value to function children", () => {
        let received: any;
        const result = <Show when={42}>{(v: any) => { received = v; return <span>{v}</span>; }}</Show>;
        expect(received).toBe(42);
        expect((result as unknown as Element).textContent).toBe("42");
    });

    it("should clean up subscriptions on dispose of children element", () => {
        const whenSignal = mockSignal<boolean>(true);
        const children = <div>Content</div>;
        const result = <Show when={whenSignal} fallback={new Comment()}>{children}</Show>;
        expect(isSignalLike(result)).toBe(true);
        const signalResult = result as unknown as SignalLike<any>;
        expect(signalResult.value).toBe(children);
        expect(whenSignal.listeners).toHaveLength(1);
        invokeDisposingListeners(signalResult.value as Element);
        expect(whenSignal.listeners).toHaveLength(0);
    });

    it("should clean up subscriptions on dispose of else content element", () => {
        const whenSignal = mockSignal<boolean>(false);
        const otherwise = new Comment();
        const result = <Show when={whenSignal} fallback={otherwise}><div>Content</div></Show>;
        expect(isSignalLike(result)).toBe(true);
        const signalResult = result as unknown as SignalLike<any>;
        expect(signalResult.value).toBe(otherwise);
        expect(whenSignal.listeners).toHaveLength(1);
        invokeDisposingListeners(signalResult.value as Element);
        expect(whenSignal.listeners).toHaveLength(0);
    });

    it("disposes previous function-slot content on switch", () => {
        const inner = mockSignal("a");
        const whenSignal = mockSignal<boolean>(true);
        const result = <Show when={whenSignal}>{() => <span>{inner}</span>}</Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);
        expect(inner.listeners).toHaveLength(1);

        whenSignal.value = false;
        expect(inner.listeners).toHaveLength(0);
    });

    it("does not dispose plain children on switch", () => {
        const inner = mockSignal("a");
        const whenSignal = mockSignal<boolean>(true);
        const children = <span>{inner}</span>;
        const result = <Show when={whenSignal} fallback={new Comment()}>{children}</Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);
        expect(inner.listeners).toHaveLength(1);

        whenSignal.value = false;
        expect(inner.listeners).toHaveLength(1);

        whenSignal.value = true;
        expect(inner.listeners).toHaveLength(1);
    });

    it("does not dispose function-slot content when autoDispose is false", () => {
        const inner = mockSignal("a");
        const whenSignal = mockSignal<boolean>(true);
        const result = <Show when={whenSignal} autoDispose={false}>{() => <span>{inner}</span>}</Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);
        expect(inner.listeners).toHaveLength(1);

        whenSignal.value = false;
        expect(inner.listeners).toHaveLength(1);
    });

    it("skips lifecycle management when autoDispose is false", () => {
        const whenSignal = mockSignal<boolean>(true);
        const children = <div>Content</div>;
        const result = <Show when={whenSignal} autoDispose={false}>{children}</Show>;
        const signalResult = result as unknown as SignalLike<any>;
        expect(whenSignal.listeners).toHaveLength(1);

        invokeDisposingListeners(signalResult.value as Element);
        expect(whenSignal.listeners).toHaveLength(1);
    });

    it("disposes hidden plain branches when the shown element is disposed", () => {
        const whenSignal = mockSignal<boolean>(true);
        const childrenSignal = mockSignal("child");
        const fallbackSignal = mockSignal("fallback");
        const children = <span>{childrenSignal}</span>;
        const fallback = <em>{fallbackSignal}</em>;
        const result = <Show when={whenSignal} fallback={fallback}>{children}</Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);
        expect(childrenSignal.listeners).toHaveLength(1);
        expect(fallbackSignal.listeners).toHaveLength(1);

        whenSignal.value = false;
        expect(childrenSignal.listeners).toHaveLength(1); // hidden, not disposed on switch
        expect(fallbackSignal.listeners).toHaveLength(1);

        invokeDisposingListeners(host, { descendants: true });
        expect(childrenSignal.listeners).toHaveLength(0); // hidden branch disposed with owner
        expect(fallbackSignal.listeners).toHaveLength(0);
    });

    it("does not dispose hidden branches on teardown when autoDispose is false", () => {
        const whenSignal = mockSignal<boolean>(true);
        const childrenSignal = mockSignal("child");
        const fallbackSignal = mockSignal("fallback");
        const children = <span>{childrenSignal}</span>;
        const fallback = <em>{fallbackSignal}</em>;
        const result = <Show when={whenSignal} fallback={fallback} autoDispose={false}>{children}</Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);

        whenSignal.value = false;
        invokeDisposingListeners(host, { descendants: true });
        expect(childrenSignal.listeners).toHaveLength(1); // Show held branch left untouched
        expect(fallbackSignal.listeners).toHaveLength(0); // shown branch cleaned by owner walk
    });

    it("disposes factory fragment branches on switch, including sibling nodes", () => {
        const a = mockSignal("a");
        const b = mockSignal("b");
        const whenSignal = mockSignal<boolean>(true);
        const result = <Show when={whenSignal}>{() => {
            const fragment = document.createDocumentFragment();
            fragment.append(<span>{a}</span>, <span>{b}</span>);
            return fragment;
        }}</Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);
        expect(a.listeners).toHaveLength(1);
        expect(b.listeners).toHaveLength(1);

        whenSignal.value = false;
        expect(a.listeners).toHaveLength(0);
        expect(b.listeners).toHaveLength(0);
    });

    it("disposes factory array branches on switch", () => {
        const a = mockSignal("a");
        const b = mockSignal("b");
        const whenSignal = mockSignal<boolean>(true);
        const result = <Show when={whenSignal}>{() => [<span>{a}</span>, <span>{b}</span>]}</Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);
        expect(a.listeners).toHaveLength(1);
        expect(b.listeners).toHaveLength(1);

        whenSignal.value = false;
        expect(a.listeners).toHaveLength(0);
        expect(b.listeners).toHaveLength(0);
    });

    it("disposes the when subscription for a primitive branch on host teardown", () => {
        const whenSignal = mockSignal<boolean>(true);
        const result = <Show when={whenSignal} fallback="off">on</Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);
        expect(host.textContent).toBe("on");
        expect(whenSignal.listeners).toHaveLength(1);

        invokeDisposingListeners(host, { descendants: true });
        expect(whenSignal.listeners).toHaveLength(0);
    });

    it("disposes the when subscription for a fragment branch on host teardown", () => {
        const whenSignal = mockSignal<boolean>(true);
        const result = <Show when={whenSignal} fallback="off">{<><b>x</b><i>y</i></>}</Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);
        expect(host.textContent).toBe("xy");
        expect(whenSignal.listeners).toHaveLength(1);

        invokeDisposingListeners(host, { descendants: true });
        expect(whenSignal.listeners).toHaveLength(0);
    });

    it("invokes a factory child once at mount", () => {
        let calls = 0;
        const whenSignal = mockSignal(true);
        const result = <Show when={whenSignal}>{() => { calls++; return <b>on</b>; }}</Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);
        expect(calls).toBe(1);
        expect(host.textContent).toBe("on");
    });

    it("does not accumulate nodes across repeated primitive switches", () => {
        const whenSignal = mockSignal<boolean>(true);
        const result = <Show when={whenSignal} fallback="off">on</Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);
        for (let i = 0; i < 50; i++)
            whenSignal.value = !whenSignal.value;
        expect(host.childNodes.length).toBe(1);
    });

    it("disposes iterable factory branches on teardown", () => {
        const whenSignal = mockSignal<boolean>(true);
        const a = mockSignal("a");
        const b = mockSignal("b");
        const result = <Show when={whenSignal}>{() => new Set([<span>{a}</span>, <span>{b}</span>])}</Show>;
        const host = <div>{result}</div>;
        document.body.appendChild(host);
        expect(a.listeners).toHaveLength(1);
        expect(b.listeners).toHaveLength(1);

        invokeDisposingListeners(host, { descendants: true });
        expect(a.listeners).toHaveLength(0);
        expect(b.listeners).toHaveLength(0);
    });

    it("compiles a fallback-only Show without a children prop", () => {
        const show = <Show when={mockSignal(true)} fallback="off" />;
        expect(show).toBeDefined();
    });
});
