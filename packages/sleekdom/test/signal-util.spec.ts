import { computed, signal } from "@preact/signals-core";
import { derivedSignal } from "../src/signal-util";

describe("createDerivedSignal", () => {
    it("should create a derived signal from an existing read-write signal", () => {
        const original = signal(0);
        const derived = derivedSignal(original, value => value + 1);
        expect(derived.value).toBe(1);
        original.value = 2;
        expect(derived.value).toBe(3);
    });

    it("should create a derived signal from an existing read-only signal", () => {
        const source = signal(5);
        const original = computed(() => source.value * 2);
        const derived = derivedSignal(original, value => value + 3);
        expect(derived.value).toBe(13);
        source.value = 10;
        expect(derived.value).toBe(23);
    });
}); 