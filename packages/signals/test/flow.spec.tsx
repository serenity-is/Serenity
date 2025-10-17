import { describe, expect, it } from "vitest";
import { Show } from "../src";
import { TestSignal } from "./test-signal";
import { isSignalLike } from "../src/util";
import { SignalLike } from "@serenity-is/sleekdom";

describe("Show", () => {
    it("shows fallback if when is false", () => {
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

    it("shows fallback if when is falsy", () => {
        expect(<Show when={null} fallback={new Comment()}><div>Content</div></Show>).toBeInstanceOf(Comment);
        expect(<Show when={undefined} fallback={new Comment()}><div>Content</div></Show>).toBeInstanceOf(Comment);
        expect(<Show when={0} fallback={new Comment()}><div>Content</div></Show>).toBeInstanceOf(Comment);
        expect(<Show when={""} fallback={new Comment()}><div>Content</div></Show>).toBeInstanceOf(Comment);
    });

    it("shows fallback if when is a signal with false value", () => {
        const result = <Show when={new TestSignal(false)} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result)).toBe(true);
        expect((result as unknown as SignalLike<any>).value).toBeInstanceOf(Comment);
    });

    it("shows content if when is a signal with true value", () => {
        const result = <Show when={new TestSignal(true)} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result)).toBe(true);
        expect((result as unknown as SignalLike<any>).value).toBeInstanceOf(HTMLDivElement);
    });

    it("shows content if when is a signal with truthy value", () => {
        const result1 = <Show when={new TestSignal("non-empty string")} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result1)).toBe(true);
        expect((result1 as unknown as SignalLike<any>).value).toBeInstanceOf(HTMLDivElement);

        const result2 = <Show when={new TestSignal(1)} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result2)).toBe(true);
        expect((result2 as unknown as SignalLike<any>).value).toBeInstanceOf(HTMLDivElement);

        const result3 = <Show when={new TestSignal({})} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result3)).toBe(true);
        expect((result3 as unknown as SignalLike<any>).value).toBeInstanceOf(HTMLDivElement);
    });

    it("shows fallback if when is a signal with falsy value", () => {
        const result1 = <Show when={new TestSignal(null)} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result1)).toBe(true);
        expect((result1 as unknown as SignalLike<any>).value).toBeInstanceOf(Comment);

        const result2 = <Show when={new TestSignal(undefined)} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result2)).toBe(true);
        expect((result2 as unknown as SignalLike<any>).value).toBeInstanceOf(Comment);

        const result3 = <Show when={new TestSignal(0)} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result3)).toBe(true);
        expect((result3 as unknown as SignalLike<any>).value).toBeInstanceOf(Comment);

        const result4 = <Show when={new TestSignal("")} fallback={new Comment()}><div>Content</div></Show>;
        expect(isSignalLike(result4)).toBe(true);
        expect((result4 as unknown as SignalLike<any>).value).toBeInstanceOf(Comment);
    });

    it("updates content when signal changes", () => {
        const whenSignal = new TestSignal<any>(false);
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
});