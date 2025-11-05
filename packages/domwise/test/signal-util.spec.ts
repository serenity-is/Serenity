import { derivedSignal, isSignalLike, isWritableSignal, isReadonlySignal, observeSignal } from "../src/signal-util";
import { computed, signal } from "../src/signals";
import { mockSignal } from "./mocks/mock-signal";

describe("isSignalLike", () => {
    it("returns true for signals", () => {
        const sig = signal(1);
        expect(isSignalLike(sig)).toBe(true);
    });

    it("returns true for computed signals", () => {
        const comp = computed(() => 1);
        expect(isSignalLike(comp)).toBe(true);
    });

    it("returns false for non-signal objects", () => {
        expect(isSignalLike({})).toBe(false);
        expect(isSignalLike(null)).toBe(false);
        expect(isSignalLike(1)).toBe(false);
        expect(isSignalLike({ subscribe: () => {}, peek: () => {} })).toBe(false); // missing value
    });
});

describe("isWritableSignal", () => {
    it("returns true for writable signals", () => {
        const sig = signal(1);
        expect(isWritableSignal(sig)).toBe(true);
    });

    it("returns false for computed signals", () => {
        const comp = computed(() => 1);
        expect(isWritableSignal(comp)).toBe(false);
    });

    it("returns false for non-signal objects", () => {
        expect(isWritableSignal({})).toBe(false);
        expect(isWritableSignal(null)).toBe(false);
    });
});

describe("isReadonlySignal", () => {
    it("returns true for computed signals", () => {
        const comp = computed(() => 1);
        expect(isReadonlySignal(comp)).toBe(true);
    });

    it("returns false for writable signals", () => {
        const sig = signal(1);
        expect(isReadonlySignal(sig)).toBe(false);
    });

    it("returns false for non-signal objects", () => {
        expect(isReadonlySignal({})).toBe(false);
        expect(isReadonlySignal(null)).toBe(false);
    });
});

describe("observeSignal", () => {
    it("calls callback on initial subscription", () => {
        const sig = signal(1);
        let capturedArgs: any;
        const callback = vi.fn((args) => {
            capturedArgs = { isInitial: args.isInitial, hasChanged: args.hasChanged, prevValue: args.prevValue, newValue: args.newValue };
        });
        observeSignal(sig, callback);
        expect(capturedArgs).toEqual({
            isInitial: true,
            hasChanged: false,
            prevValue: undefined,
            newValue: 1
        });
    });

    it("calls callback on value change", () => {
        const sig = signal(1);
        let capturedArgs: any;
        const callback = vi.fn((args) => {
            capturedArgs = { isInitial: args.isInitial, hasChanged: args.hasChanged, prevValue: args.prevValue, newValue: args.newValue };
        });
        observeSignal(sig, callback);
        callback.mockClear();
        capturedArgs = null;
        sig.value = 2;
        expect(capturedArgs).toEqual({
            isInitial: false,
            hasChanged: true,
            prevValue: 1,
            newValue: 2
        });
    });

    it("does not call callback if value unchanged", () => {
        const sig = signal(1);
        const callback = vi.fn();
        observeSignal(sig, callback);
        callback.mockClear();
        sig.value = 1; // same value
        expect(callback).not.toHaveBeenCalled();
    });

    it("returns disposer that stops observation", () => {
        const sig = signal(1);
        const callback = vi.fn();
        const disposer = observeSignal(sig, callback);
        callback.mockClear();
        disposer();
        sig.value = 2;
        expect(callback).not.toHaveBeenCalled();
    });
});

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

    it("should return a primitive signal instance if signal constructor is unavailable", () => {
        const signalMock = mockSignal(42);
        expect(signalMock.constructor).toBe({}.constructor);
        const derived = derivedSignal(signalMock, value => value * 2);
        expect(derived.value).toBe(84);
        expect(derived).toHaveProperty('derivedDisposer');
        expect(typeof derived.derivedDisposer).toBe('function');
        signalMock.value = 50;
        expect(derived.value).toBe(100);
    });
});
