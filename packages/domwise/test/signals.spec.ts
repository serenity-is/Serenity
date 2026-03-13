import { isWritableSignal } from "../src/signal-util";
import { useSignal, useUpdatableComputed } from "../src/signals";

describe("useSignal", () => {
    it("creates a signal with the initial value", () => {
        const initialValue = 42;
        const sig = useSignal(initialValue);
        expect(sig.value).toBe(initialValue);
    });

    it("updates the signal value", () => {
        const sig = useSignal(0);
        sig.value = 100;
        expect(sig.value).toBe(100);
    });

    it("maintains state across multiple calls", () => {
        const sig = useSignal('initial');
        expect(sig.value).toBe('initial');
        sig.value = 'updated';
        expect(sig.value).toBe('updated');
    });

    it("returns a signal that is identified as writable", () => {
        const sig = useSignal(true);
        expect(isWritableSignal(sig)).toBe(true);
    });
});

describe("useUpdatableComputed", () => {
    it("recomputes value only when update() is called", () => {
        const { computed, update } = useUpdatableComputed();
        let calls = 0;
        const c = computed(() => ++calls);

        expect(c.value).toBe(1);
        expect(c.value).toBe(1); // cached value

        update();
        expect(c.value).toBe(2);
        expect(c.value).toBe(2); // cached again
    });

    it("updates derived values based on external state when update() is called", () => {
        const { computed, update } = useUpdatableComputed();
        let base = 0;
        const derived = computed(() => base * 10);

        expect(derived.value).toBe(0);
        base = 3;

        // Without calling update, the computed value should not reflect the changed base state
        expect(derived.value).toBe(0);

        update();
        expect(derived.value).toBe(30);
    });
});

