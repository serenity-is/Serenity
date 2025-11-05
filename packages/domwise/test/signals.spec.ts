import { isWritableSignal } from "../src/signal-util";
import { useSignal } from "../src/signals";

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