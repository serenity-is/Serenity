import { describe, expect, it } from "vitest";
import { invokeDisposingListeners } from "../../src/disposing-listener";
import { Show } from "../../src/flow/show";
import { isSignalLike } from "../../src/signal-util";
import type { SignalLike } from "../../src/types/signal-like";
import { mockSignal } from "../mocks/mock-signal";

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
});