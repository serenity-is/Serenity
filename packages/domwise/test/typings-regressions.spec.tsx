import { Show, signal } from "../src";

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
    });
});
