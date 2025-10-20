import { isSignalLike, onElementDisposing } from "@serenity-is/sleekdom";
import { describe, expect, it } from "vitest";
import { IfElse } from "../src/if-else";
import type { Signal } from "../src/signals";
import { MockSignal } from "./mocks/mock-signal";

describe("Show", () => {
    it("shows else content if when is false", () => {
        expect(<IfElse when={false} else={new Comment()}><div>Content</div></IfElse>).toBeInstanceOf(Comment);
    });

    it("shows content if when is true", () => {
        expect(<IfElse when={true} else={new Comment()}><div>Content</div></IfElse>).toBeInstanceOf(HTMLDivElement);
    });

    it("shows content if when is truthy", () => {
        expect(<IfElse when={"non-empty string"} else={new Comment()}><div>Content</div></IfElse>).toBeInstanceOf(HTMLDivElement);
        expect(<IfElse when={1} else={new Comment()}><div>Content</div></IfElse>).toBeInstanceOf(HTMLDivElement);
        expect(<IfElse when={{}} else={new Comment()}><div>Content</div></IfElse>).toBeInstanceOf(HTMLDivElement);
    });

    it("shows else content if when is falsy", () => {
        expect(<IfElse when={null} else={new Comment()}><div>Content</div></IfElse>).toBeInstanceOf(Comment);
        expect(<IfElse when={undefined} else={new Comment()}><div>Content</div></IfElse>).toBeInstanceOf(Comment);
        expect(<IfElse when={0} else={new Comment()}><div>Content</div></IfElse>).toBeInstanceOf(Comment);
        expect(<IfElse when={""} else={new Comment()}><div>Content</div></IfElse>).toBeInstanceOf(Comment);
    });

    it("shows else content if when is a signal with false value", () => {
        const result = <IfElse when={new MockSignal(false)} else={new Comment()}><div>Content</div></IfElse>;
        expect(isSignalLike(result)).toBe(true);
        expect((result as unknown as Signal<any>).value).toBeInstanceOf(Comment);
    });

    it("shows content if when is a signal with true value", () => {
        const result = <IfElse when={new MockSignal(true)} else={new Comment()}><div>Content</div></IfElse>;
        expect(isSignalLike(result)).toBe(true);
        expect((result as unknown as Signal<any>).value).toBeInstanceOf(HTMLDivElement);
    });

    it("shows content if when is a signal with truthy value", () => {
        const result1 = <IfElse when={new MockSignal("non-empty string")} else={new Comment()}><div>Content</div></IfElse>;
        expect(isSignalLike(result1)).toBe(true);
        expect((result1 as unknown as Signal<any>).value).toBeInstanceOf(HTMLDivElement);

        const result2 = <IfElse when={new MockSignal(1)} else={new Comment()}><div>Content</div></IfElse>;
        expect(isSignalLike(result2)).toBe(true);
        expect((result2 as unknown as Signal<any>).value).toBeInstanceOf(HTMLDivElement);

        const result3 = <IfElse when={new MockSignal({})} else={new Comment()}><div>Content</div></IfElse>;
        expect(isSignalLike(result3)).toBe(true);
        expect((result3 as unknown as Signal<any>).value).toBeInstanceOf(HTMLDivElement);
    });

    it("shows else content if when is a signal with falsy value", () => {
        const result1 = <IfElse when={new MockSignal(null)} else={new Comment()}><div>Content</div></IfElse>;
        expect(isSignalLike(result1)).toBe(true);
        expect((result1 as unknown as Signal<any>).value).toBeInstanceOf(Comment);

        const result2 = <IfElse when={new MockSignal(undefined)} else={new Comment()}><div>Content</div></IfElse>;
        expect(isSignalLike(result2)).toBe(true);
        expect((result2 as unknown as Signal<any>).value).toBeInstanceOf(Comment);

        const result3 = <IfElse when={new MockSignal(0)} else={new Comment()}><div>Content</div></IfElse>;
        expect(isSignalLike(result3)).toBe(true);
        expect((result3 as unknown as Signal<any>).value).toBeInstanceOf(Comment);

        const result4 = <IfElse when={new MockSignal("")} else={new Comment()}><div>Content</div></IfElse>;
        expect(isSignalLike(result4)).toBe(true);
        expect((result4 as unknown as Signal<any>).value).toBeInstanceOf(Comment);
    });

    it("updates content when signal changes", () => {
        const whenSignal = new MockSignal<any>(false);
        const result = <IfElse when={whenSignal} else={new Comment()}><div>Content</div></IfElse>;
        expect(isSignalLike(result)).toBe(true);
        const signalResult = result as unknown as Signal<any>;
        expect(signalResult.value).toBeInstanceOf(Comment);

        whenSignal.value = true;
        expect(signalResult.value).toBeInstanceOf(HTMLDivElement);

        whenSignal.value = 0;
        expect(signalResult.value).toBeInstanceOf(Comment);

        whenSignal.value = "hello";
        expect(signalResult.value).toBeInstanceOf(HTMLDivElement);
    });

    it("should clean up subscriptions on dispose of children element", () => {
        const whenSignal = new MockSignal<boolean>(true);
        const children = <div>Content</div>;
        const result = <IfElse when={whenSignal} else={new Comment()}>{children}</IfElse>;
        expect(isSignalLike(result)).toBe(true);
        const signalResult = result as unknown as Signal<any>;
        expect(signalResult.value).toBe(children);
        expect(whenSignal.listeners).toHaveLength(1);
        onElementDisposing(signalResult.value as Element);
        expect(whenSignal.listeners).toHaveLength(0);
    });

    it("should clean up subscriptions on dispose of else content element", () => {
        const whenSignal = new MockSignal<boolean>(false);
        const otherwise = new Comment();
        const result = <IfElse when={whenSignal} else={otherwise}><div>Content</div></IfElse>;
        expect(isSignalLike(result)).toBe(true);
        const signalResult = result as unknown as Signal<any>;
        expect(signalResult.value).toBe(otherwise);
        expect(whenSignal.listeners).toHaveLength(1);
        onElementDisposing(signalResult.value as Element);
        expect(whenSignal.listeners).toHaveLength(0);
    });
});