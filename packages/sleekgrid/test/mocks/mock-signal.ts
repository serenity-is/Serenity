import type { SignalLike } from "@serenity-is/sleekdom";

export function mockSignal<T>(initialValue: T): SignalLike<T> & { callbacks?: Array<(value: T) => void> } {
    const signal = {
        currentValue: initialValue,
        peek: vi.fn(() => signal.currentValue) as () => T,
        subscribe: vi.fn(function (callback) {
            signal.listeners.push(callback); return () => {
                signal.listeners = signal.listeners.filter(cb => cb !== callback);
            }
        }),
        get value() { return signal.currentValue },
        set value(val: T) { if (signal.currentValue !== val) { signal.currentValue = val; signal.listeners.forEach(cb => cb.call(this, val)); } },
        listeners: [] as Array<(value: any) => void>
    };
    return signal;
}
