import { Show, signal, type DerivedSignalLike, type ObserveSignalCallback, type SignalObserveArgs } from "../src";

describe("typing regressions", () => {
    it("compiles the fixed typings", () => {
        const when = signal(true);

        // L13: fallback-only Show (no children prop)
        const show = <Show when={when} fallback="off" />;
        expect(show).toBeDefined();

        // L11: class array containing a signal
        const cls = <div class={["a", signal(false)]} />;
        expect(cls).toBeDefined();

        // L10: boolean/number dataset values
        const ds = <div dataset={{ flag: true, count: 3 }} />;
        expect(ds).toBeDefined();

        // L9: generated capture-phase handlers
        const cap = <div onTouchStartCapture={() => { }} onAnimationEndCapture={() => { }} onGotPointerCapture={() => { }} />;
        expect(cap).toBeDefined();

        // L14: signal utility types are exported
        const cb: ObserveSignalCallback<number> = () => { };
        const args = null as unknown as SignalObserveArgs<number>;
        const derived = null as unknown as DerivedSignalLike<number>;
        expect(cb && args && derived).toBeDefined();
    });
});
