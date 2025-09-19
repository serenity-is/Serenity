import { LayoutTimer, executeOnceWhenVisible, executeEverytimeWhenVisible } from "./layouttimer";
import { vi, beforeEach, afterEach, describe, it, expect } from "vitest";

let mockElement: HTMLElement;

beforeEach(() => {
    // Mock DOM element
    mockElement = {
        offsetWidth: 100,
        offsetHeight: 100
    } as HTMLElement;

    vi.useFakeTimers();
});

afterEach(() => {
    vi.clearAllMocks();
});

describe("LayoutTimeronSizeChange", () => {
    it("should register a size change handler", () => {
        const handler = vi.fn();
        const key = LayoutTimer.onSizeChange(() => mockElement, handler);

        expect(typeof key).toBe('number');
        expect(key).toBeGreaterThan(0);
    });

    it("should call handler when element size changes", () => {
        const handler = vi.fn();
        LayoutTimer.onSizeChange(() => mockElement, handler);

        // Change size
        (mockElement as any).offsetWidth = 200;
        (mockElement as any).offsetHeight = 150;

        // Wait for timeout to execute
        vi.runOnlyPendingTimers();

        expect(handler).toHaveBeenCalled();
    });

    it("should not call handler when size doesn't change", () => {
        const handler = vi.fn();
        LayoutTimer.onSizeChange(() => mockElement, handler);

        // Keep same size
        vi.runOnlyPendingTimers();

        expect(handler).not.toHaveBeenCalled();
    });

    it("should handle width-only monitoring", () => {
        const handler = vi.fn();
        LayoutTimer.onSizeChange(() => mockElement, handler, { width: true, height: false });

        (mockElement as any).offsetWidth = 200;
        vi.runOnlyPendingTimers();

        expect(handler).toHaveBeenCalled();
    });

    it("should handle height-only monitoring", () => {
        const handler = vi.fn();
        LayoutTimer.onSizeChange(() => mockElement, handler, { width: false, height: true });

        (mockElement as any).offsetHeight = 150;
        vi.runOnlyPendingTimers();

        expect(handler).toHaveBeenCalled();
    });

    it("should throw error for null handler", () => {
        expect(() => {
            LayoutTimer.onSizeChange(() => mockElement, null as any);
        }).toThrow("Layout handler can't be null!");
    });
});

describe("LayoutTimer.onWidthChange", () => {
    it("should register a width change handler", () => {
        const handler = vi.fn();
        const key = LayoutTimer.onWidthChange(() => mockElement, handler);

        expect(typeof key).toBe('number');
        expect(key).toBeGreaterThan(0);
    });

    it("should call handler when width changes", () => {
        const handler = vi.fn();
        LayoutTimer.onWidthChange(() => mockElement, handler);

        (mockElement as any).offsetWidth = 200;
        vi.runOnlyPendingTimers();

        expect(handler).toHaveBeenCalled();
    });

    it("should not call handler when only height changes", () => {
        const handler = vi.fn();
        LayoutTimer.onWidthChange(() => mockElement, handler);

        (mockElement as any).offsetHeight = 150;
        vi.runOnlyPendingTimers();

        expect(handler).not.toHaveBeenCalled();
    });
});

describe("LayoutTimer.onHeightChange", () => {
    it("should register a height change handler", () => {
        const handler = vi.fn();
        const key = LayoutTimer.onHeightChange(() => mockElement, handler);

        expect(typeof key).toBe('number');
        expect(key).toBeGreaterThan(0);
    });

    it("should call handler when height changes", () => {
        const handler = vi.fn();
        LayoutTimer.onHeightChange(() => mockElement, handler);

        (mockElement as any).offsetHeight = 150;
        vi.runOnlyPendingTimers();

        expect(handler).toHaveBeenCalled();
    });

    it("should not call handler when only width changes", () => {
        const handler = vi.fn();
        LayoutTimer.onHeightChange(() => mockElement, handler);

        (mockElement as any).offsetWidth = 200;
        vi.runOnlyPendingTimers();

        expect(handler).not.toHaveBeenCalled();
    });
});

describe("LayoutTimer.onShown", () => {
    it("should register a shown handler", () => {
        const handler = vi.fn();
        const key = LayoutTimer.onShown(() => mockElement, handler);

        expect(typeof key).toBe('number');
        expect(key).toBeGreaterThan(0);
    });

    it("should call handler when element becomes visible", () => {
        const handler = vi.fn();
        LayoutTimer.onShown(() => mockElement, handler);

        // Initially visible, should not call immediately
        vi.runOnlyPendingTimers();
        expect(handler).not.toHaveBeenCalled();

        // Make invisible then visible
        (mockElement as any).offsetWidth = 0;
        (mockElement as any).offsetHeight = 0;
        vi.runOnlyPendingTimers();

        (mockElement as any).offsetWidth = 100;
        (mockElement as any).offsetHeight = 100;
        vi.runOnlyPendingTimers();

        expect(handler).toHaveBeenCalled();
    });
});

describe("store", () => {
    it("should store current element dimensions", () => {
        const handler = vi.fn();
        const key = LayoutTimer.onSizeChange(() => mockElement, handler);

        (mockElement as any).offsetWidth = 200;
        (mockElement as any).offsetHeight = 150;

        LayoutTimer.store(key);

        // Should not trigger handler since dimensions are stored
        vi.runOnlyPendingTimers();
        expect(handler).not.toHaveBeenCalled();
    });

    it("should do nothing for invalid key", () => {
        expect(() => {
            LayoutTimer.store(999);
        }).not.toThrow();
    });
});

describe("LayoutTimer.trigger", () => {
    it("should manually trigger handler if element is visible", () => {
        const handler = vi.fn();
        const key = LayoutTimer.onSizeChange(() => mockElement, handler);

        LayoutTimer.trigger(key);

        expect(handler).toHaveBeenCalled();
    });

    it("should not trigger handler if element is not visible", () => {
        const handler = vi.fn();
        const key = LayoutTimer.onSizeChange(() => mockElement, handler);

        (mockElement as any).offsetWidth = 0;
        (mockElement as any).offsetHeight = 0;

        LayoutTimer.trigger(key);

        expect(handler).not.toHaveBeenCalled();
    });

    it("should do nothing for invalid key", () => {
        expect(() => {
            LayoutTimer.trigger(999);
        }).not.toThrow();
    });
});

describe("LayoutTimer.off", () => {
    it("should unregister handler", () => {
        const handler = vi.fn();
        const key = LayoutTimer.onSizeChange(() => mockElement, handler);

        LayoutTimer.off(key);

        (mockElement as any).offsetWidth = 200;
        vi.runOnlyPendingTimers();

        expect(handler).not.toHaveBeenCalled();
    });

    it("should return 0 for invalid key", () => {
        const result = LayoutTimer.off(999);
        expect(result).toBe(0);
    });
});

describe("executeOnceWhenVisible", () => {
    let mockElement: HTMLElement;

    beforeEach(() => {
        mockElement = {
            offsetWidth: 0,
            offsetHeight: 0
        } as HTMLElement;
    });

    it("should execute callback immediately if element is visible", () => {
        (mockElement as any).offsetWidth = 100;
        (mockElement as any).offsetHeight = 100;

        const callback = vi.fn();
        executeOnceWhenVisible(mockElement, callback);

        expect(callback).toHaveBeenCalled();
    });

    it("should register LayoutTimer.onShown if element is not visible", () => {
        const callback = vi.fn();
        executeOnceWhenVisible(mockElement, callback);

        expect(callback).not.toHaveBeenCalled();

        // Make element visible
        (mockElement as any).offsetWidth = 100;
        (mockElement as any).offsetHeight = 100;

        // Simulate timer execution
        vi.runOnlyPendingTimers();

        expect(callback).toHaveBeenCalled();
    });

    it("should handle array-like element input", () => {
        const arrayLike = [mockElement];
        const callback = vi.fn();
        executeOnceWhenVisible(arrayLike, callback);

        expect(callback).not.toHaveBeenCalled();
    });

    it("should do nothing for null element", () => {
        const callback = vi.fn();
        executeOnceWhenVisible(null as any, callback);

        expect(callback).not.toHaveBeenCalled();
    });
});

describe("executeEverytimeWhenVisible", () => {
    let mockElement: HTMLElement;

    beforeEach(() => {
        mockElement = {
            offsetWidth: 0,
            offsetHeight: 0
        } as HTMLElement;
    });

    it("should execute callback immediately if callNowIfVisible is true and element is visible", () => {
        (mockElement as any).offsetWidth = 100;
        (mockElement as any).offsetHeight = 100;

        const callback = vi.fn();
        executeEverytimeWhenVisible(mockElement, callback, true);

        expect(callback).toHaveBeenCalled();
    });

    it("should register LayoutTimer.onShown for visibility changes", () => {
        const callback = vi.fn();
        executeEverytimeWhenVisible(mockElement, callback, false);

        expect(callback).not.toHaveBeenCalled();

        // Make element visible
        (mockElement as any).offsetWidth = 100;
        (mockElement as any).offsetHeight = 100;

        vi.runOnlyPendingTimers();

        expect(callback).toHaveBeenCalled();
    });

    it("should handle array-like element input", () => {
        const arrayLike = [mockElement];
        const callback = vi.fn();
        executeEverytimeWhenVisible(arrayLike, callback, false);

        expect(callback).not.toHaveBeenCalled();
    });

    it("should do nothing for null element", () => {
        const callback = vi.fn();
        executeEverytimeWhenVisible(null as any, callback, false);

        expect(callback).not.toHaveBeenCalled();
    });
});
