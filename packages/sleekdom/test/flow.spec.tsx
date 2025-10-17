/** @jsxImportSource ../src */
import { describe, expect, it } from "vitest";
import { Show, PlainSignal } from "../src";

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
        const result = <Show when={new PlainSignal(false)} fallback={new Comment()}><div>Content</div></Show>;
        expect(result).toBeInstanceOf(PlainSignal);
        expect((result as unknown as PlainSignal).value).toBeInstanceOf(Comment);
    });

    it("shows content if when is a signal with true value", () => {
        const result = <Show when={new PlainSignal(true)} fallback={new Comment()}><div>Content</div></Show>;
        expect(result).toBeInstanceOf(PlainSignal);
        expect((result as unknown as PlainSignal).value).toBeInstanceOf(HTMLDivElement);
    });

    it("shows content if when is a signal with truthy value", () => {
        const result1 = <Show when={new PlainSignal("non-empty string")} fallback={new Comment()}><div>Content</div></Show>;
        expect(result1).toBeInstanceOf(PlainSignal);
        expect((result1 as unknown as PlainSignal).value).toBeInstanceOf(HTMLDivElement);

        const result2 = <Show when={new PlainSignal(1)} fallback={new Comment()}><div>Content</div></Show>;
        expect(result2).toBeInstanceOf(PlainSignal);
        expect((result2 as unknown as PlainSignal).value).toBeInstanceOf(HTMLDivElement);

        const result3 = <Show when={new PlainSignal({})} fallback={new Comment()}><div>Content</div></Show>;
        expect(result3).toBeInstanceOf(PlainSignal);
        expect((result3 as unknown as PlainSignal).value).toBeInstanceOf(HTMLDivElement);
    });

    it("shows fallback if when is a signal with falsy value", () => {
        const result1 = <Show when={new PlainSignal(null)} fallback={new Comment()}><div>Content</div></Show>;
        expect(result1).toBeInstanceOf(PlainSignal);
        expect((result1 as unknown as PlainSignal).value).toBeInstanceOf(Comment);

        const result2 = <Show when={new PlainSignal(undefined)} fallback={new Comment()}><div>Content</div></Show>;
        expect(result2).toBeInstanceOf(PlainSignal);
        expect((result2 as unknown as PlainSignal).value).toBeInstanceOf(Comment);

        const result3 = <Show when={new PlainSignal(0)} fallback={new Comment()}><div>Content</div></Show>;
        expect(result3).toBeInstanceOf(PlainSignal);
        expect((result3 as unknown as PlainSignal).value).toBeInstanceOf(Comment);

        const result4 = <Show when={new PlainSignal("")} fallback={new Comment()}><div>Content</div></Show>;
        expect(result4).toBeInstanceOf(PlainSignal);
        expect((result4 as unknown as PlainSignal).value).toBeInstanceOf(Comment);
    });

    it("updates content when signal changes", () => {
        const whenSignal = new PlainSignal<any>(false);
        const result = <Show when={whenSignal} fallback={new Comment()}><div>Content</div></Show>;
        expect(result).toBeInstanceOf(PlainSignal);
        const signalResult = result as unknown as PlainSignal;
        expect(signalResult.value).toBeInstanceOf(Comment);

        whenSignal.value = true;
        expect(signalResult.value).toBeInstanceOf(HTMLDivElement);

        whenSignal.value = 0;
        expect(signalResult.value).toBeInstanceOf(Comment);

        whenSignal.value = "hello";
        expect(signalResult.value).toBeInstanceOf(HTMLDivElement);
    });
});