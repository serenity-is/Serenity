import { derivedSignal, isReadonlySignal, isSignalLike, isWritableSignal, observeSignal, PrimitiveComputed } from "../src/signal-util";
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

    it("returns true for signals with value setter in prototype chain", () => {
        // Create a signal-like object with value setter in prototype
        const proto = {};
        Object.defineProperty(proto, 'value', {
            set() { },
            enumerable: true,
            configurable: true
        });

        const obj = Object.create(proto);
        obj.subscribe = () => {};
        obj.peek = () => {};
        obj.value = 42;

        expect(isWritableSignal(obj)).toBe(true);
    });

    it("returns false for signals with non-writable value in prototype chain", () => {
        // Create a signal-like object with non-writable value in prototype
        const proto = {};
        Object.defineProperty(proto, 'value', {
            writable: false,
            value: 42,
            enumerable: true,
            configurable: true
        });

        const obj = Object.create(proto);
        obj.subscribe = () => {};
        obj.peek = () => {};

        expect(isWritableSignal(obj)).toBe(false);
    });

    it("returns false for signal-like objects without value property in prototype chain", () => {
        // Create a signal-like object without any value property in prototype chain
        const obj: any = {};
        obj.subscribe = () => {};
        obj.peek = () => {};
        // Don't add value property anywhere

        expect(isWritableSignal(obj)).toBe(false);
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

    it("handles lifecycle node changes", () => {
        const sig = signal(1);
        const lifecycleNode = document.createElement('div');
        const callback = vi.fn();

        const disposer = observeSignal(sig, callback, { lifecycleNode });
        expect(disposer).toBeDefined();

        disposer();
    });

    it("handles useLifecycleRoot option", () => {
        const sig = signal(1);
        const callback = vi.fn();

        const disposer = observeSignal(sig, callback, { useLifecycleRoot: true });
        expect(disposer).toBeDefined();

        disposer();
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

    it("should throw error for non-signal input", () => {
        expect(() => derivedSignal({} as any, value => value)).toThrow("Input must be a SignalLike");
        expect(() => derivedSignal(null as any, value => value)).toThrow("Input must be a SignalLike");
        expect(() => derivedSignal(42 as any, value => value)).toThrow("Input must be a SignalLike");
    });

    it("should create derived signal using constructor when available", () => {
        // Create a mock signal with a working constructor
        class MockSignal {
            constructor(fn: () => any) {
                this.value = fn();
            }
            value: any;
            subscribe(callback: (value: any) => void) {
                callback(this.value);
                return () => {};
            }
            peek() {
                return this.value;
            }
        }

        const original = new MockSignal(() => 10);
        const derived = derivedSignal(original, value => value * 2);
        
        // Should be an instance of MockSignal
        expect(derived).toBeInstanceOf(MockSignal);
        expect(derived.value).toBe(20);
    });
});

describe("PrimitiveComputed", () => {
    it("should initialize with computed value", () => {
        let callCount = 0;
        const comp = new PrimitiveComputed(() => {
            callCount++;
            return callCount * 2;
        });
        expect(comp.value).toBe(2);
        expect(callCount).toBe(1);
    });

    it("should update value when update is called", () => {
        let base = 1;
        const comp = new PrimitiveComputed(() => base * 2);
        expect(comp.value).toBe(2);

        base = 3;
        comp.update();
        expect(comp.value).toBe(6);
    });

    it("should not update subscribers if value unchanged", () => {
        let base = 1;
        const comp = new PrimitiveComputed(() => base);
        const callback = vi.fn();
        comp.subscribe(callback);
        callback.mockClear();

        comp.update(); // value still 1
        expect(callback).not.toHaveBeenCalled();
    });

    it("should update subscribers if value changed", () => {
        let base = 1;
        const comp = new PrimitiveComputed(() => base);
        const callback = vi.fn();
        comp.subscribe(callback);
        callback.mockClear();

        base = 2;
        comp.update();
        expect(callback).toHaveBeenCalledWith(2);
    });

    it("should force update even if value unchanged when force=true", () => {
        const comp = new PrimitiveComputed(() => 42);
        const callback = vi.fn();
        comp.subscribe(callback);
        callback.mockClear();

        comp.update(true); // force update
        expect(callback).toHaveBeenCalledWith(42);
    });

    it("should return current value from peek", () => {
        const comp = new PrimitiveComputed(() => 123);
        expect(comp.peek()).toBe(123);
    });

    it("should handle multiple subscribers", () => {
        let value = 1;
        const comp = new PrimitiveComputed(() => value);
        const callback1 = vi.fn();
        const callback2 = vi.fn();

        const disposer1 = comp.subscribe(callback1);
        const disposer2 = comp.subscribe(callback2);

        value = 2;
        comp.update();

        expect(callback1).toHaveBeenCalledWith(2);
        expect(callback2).toHaveBeenCalledWith(2);

        disposer1();
        callback1.mockClear();
        callback2.mockClear();

        value = 3;
        comp.update();

        expect(callback1).not.toHaveBeenCalled();
        expect(callback2).toHaveBeenCalledWith(3);
    });
});
