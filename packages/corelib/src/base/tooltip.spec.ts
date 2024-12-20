import { Tooltip } from "./tooltip";
import { mockJQuery, unmockBSAndJQuery } from "../test/mocks";

let element: HTMLElement;
let tooltip: Tooltip;

beforeEach(() => {
    element = document.createElement("div");
    document.body.appendChild(element);
});

afterEach(() => {
    unmockBSAndJQuery();
    tooltip?.dispose();
    element.remove();
});

describe("Tooltip constructor", () => {

    it("ignores null elements", () => {
        tooltip = new Tooltip(null);
        expect(tooltip).toBeDefined();
    });

    it("should create a tooltip instance with default options", () => {
        tooltip = new Tooltip(element);
        expect(tooltip).toBeDefined();
    });

    it("should create a tooltip instance with custom options", () => {
        const options = {
            title: "Custom Tooltip"
        };
        tooltip = new Tooltip(element, options);

        expect(tooltip).toBeDefined();
        expect(element.title).toBe(options.title);
    });

    it("creates a tooltip via jQuery if available", () => {
        const tooltipSpy = vi.fn();
        mockJQuery({
            data: () => { return {} },
            tooltip: tooltipSpy
        });
        tooltip = new Tooltip(element);
        expect(tooltip).toBeDefined();
        expect(tooltipSpy).toHaveBeenCalledTimes(1);
    });
});

describe("Tooltip.getInstance", () => {
    it("returns null if no instance yet", () => {
        const instance = Tooltip.getInstance(element);
        expect(instance).toBeNull();
    });

    it("returns null if BS tooltip not available", () => {
        new Tooltip(element, { title: "Test" });
        const instance = Tooltip.getInstance(element);
        expect(instance).toBeNull();
    });    
});

describe("Tooltip.toggle", () => {
    it("should show the tooltip when toggle(true) is called", () => {
        const instance = {
            show: vi.fn(),
            hide: vi.fn()
        }
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return instance;
            }
        };
        (bs.Tooltip as any).getInstance = () => instance;
        tooltip = new Tooltip(element);
        tooltip.toggle(true);
        expect(instance.show).toHaveBeenCalledTimes(1);
        expect(instance.hide).not.toHaveBeenCalled();
    });

    it("should show hide tooltip when toggle(false) is called", () => {
        const instance = {
            show: vi.fn(),
            hide: vi.fn()
        }
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return instance;
            }
        };
        (bs.Tooltip as any).getInstance = () => instance;
        tooltip = new Tooltip(element);
        tooltip.toggle(false);
        expect(instance.hide).toHaveBeenCalledTimes(1);
        expect(instance.show).not.toHaveBeenCalled();
    });    

    it("should not throw an error if element is null", () => {
        tooltip = new Tooltip(null);
        expect(() => tooltip.toggle(true)).not.toThrow();
        expect(() => tooltip.toggle(false)).not.toThrow();
    });

    it("should not show or hide if no existing instance", () => {
        tooltip = new Tooltip(element);
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return null as any;
            }
        };
        (bs.Tooltip as any).getInstance = () => null as any;
        tooltip = new Tooltip(element);
        tooltip.toggle(true);
    });
});

describe("Tooltip.show", () => {
    it("should show the tooltip", () => {
        const instance = {
            show: vi.fn(),
            hide: vi.fn()
        }
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return instance;
            }
        };
        (bs.Tooltip as any).getInstance = () => instance;
        tooltip = new Tooltip(element);
        tooltip.show();
        expect(instance.show).toHaveBeenCalledTimes(1);
        expect(instance.hide).not.toHaveBeenCalled();
    });

    it("should not throw an error if element is null", () => {
        tooltip = new Tooltip(null);
        expect(() => tooltip.show()).not.toThrow();
    });

    it("should not show if no existing instance", () => {
        tooltip = new Tooltip(element);
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return null as any;
            }
        };
        (bs.Tooltip as any).getInstance = () => null as any;
        tooltip.show();
    });
});

describe("Tooltip.hide", () => {
    it("should hide the tooltip", () => {
        const instance = {
            show: vi.fn(),
            hide: vi.fn()
        }
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return instance;
            }
        };
        (bs.Tooltip as any).getInstance = () => instance;
        tooltip = new Tooltip(element);
        tooltip.hide();
        expect(instance.hide).toHaveBeenCalledTimes(1);
        expect(instance.show).not.toHaveBeenCalled();
    });

    it("should not throw an error if element is null", () => {
        tooltip = new Tooltip(null);
        expect(() => tooltip.hide()).not.toThrow();
    });

    it("should not hide if no existing instance", () => {
        tooltip = new Tooltip(element);
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return null as any;
            }
        };
        (bs.Tooltip as any).getInstance = () => null as any;
        tooltip.hide();
    });
});

describe("Tooltip.setTitle", () => {
    it("should set the title attribute on the element", () => {
        tooltip = new Tooltip(element);
        tooltip.setTitle("New Title");
        expect(element.getAttribute("title")).toBe("New Title");
    });

    it("should update the tooltip instance with the new title", () => {
        const instance = {
            show: vi.fn(),
            hide: vi.fn(),
            _fixTitle: vi.fn(),
            tip: {
                querySelector: vi.fn().mockReturnValue({ textContent: "" })
            },
            update: vi.fn()
        };
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return instance;
            }
        };
        (bs.Tooltip as any).getInstance = () => instance;
        tooltip = new Tooltip(element);
        tooltip.setTitle("Updated Title");
        expect(instance._fixTitle).toHaveBeenCalled();
        expect(instance.tip.querySelector).toHaveBeenCalledWith(".tooltip-inner");
        expect(instance.tip.querySelector().textContent).toBe("Updated Title");
        expect(instance.update).toHaveBeenCalled();
    });

    it("should not throw an error if element is null", () => {
        tooltip = new Tooltip(null);
        expect(() => tooltip.setTitle("Title")).not.toThrow();
    });

    it("should not update if no existing instance", () => {
        tooltip = new Tooltip(element);
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return null as any;
            }
        };
        (bs.Tooltip as any).getInstance = () => null as any;
        tooltip.setTitle("Title");
    });
});

describe("Tooltip.dispose", () => {
    it("should dispose the tooltip instance", () => {
        const instance = {
            destroy: vi.fn(),
            dispose: vi.fn()
        };
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return instance;
            }
        };
        (bs.Tooltip as any).getInstance = () => instance;
        tooltip = new Tooltip(element);
        tooltip.dispose();
        expect(instance.dispose).toHaveBeenCalledTimes(1);
    });

    it("should not throw an error if element is null", () => {
        tooltip = new Tooltip(null);
        expect(() => tooltip.dispose()).not.toThrow();
    });

    it("should not dispose if no existing instance", () => {
        tooltip = new Tooltip(element);
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return null as any;
            }
        };
        (bs.Tooltip as any).getInstance = () => null as any;
        tooltip.dispose();
    });
});

describe("Tooltip.delayedDispose", () => {
    beforeEach(() => {
        vi.useFakeTimers();
    });

    afterEach(() => {
        vi.useRealTimers();
    });

    it("should dispose the tooltip instance after the specified delay", () => {
        const instance = {
            destroy: vi.fn(),
            dispose: vi.fn()
        };
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return instance;
            }
        };
        (bs.Tooltip as any).getInstance = () => instance;
        tooltip = new Tooltip(element);
        tooltip.delayedDispose(1000);

        vi.advanceTimersByTime(1000);
        expect(instance.dispose).toHaveBeenCalledTimes(1);
    });

    it("should not throw an error if element is null", () => {
        tooltip = new Tooltip(null);
        expect(() => tooltip.delayedDispose(1000)).not.toThrow();
    });

    it("should not dispose if no existing instance", () => {
        tooltip = new Tooltip(element);
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return null as any;
            }
        };
        (bs.Tooltip as any).getInstance = () => null as any;
        tooltip.delayedDispose(1000);

        vi.advanceTimersByTime(1000);
        expect(() => tooltip.dispose()).not.toThrow();
    });

    it("should use default delay if no delay is specified", () => {
        const instance = {
            destroy: vi.fn(),
            dispose: vi.fn()
        };
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return instance;
            }
        };
        (bs.Tooltip as any).getInstance = () => instance;
        tooltip = new Tooltip(element);
        tooltip.delayedDispose();

        vi.advanceTimersByTime(5000);
        expect(instance.dispose).toHaveBeenCalledTimes(1);
    });
});

describe("Tooltip.delayedHide", () => {
    beforeEach(() => {
        vi.useFakeTimers();
    });

    afterEach(() => {
        vi.useRealTimers();
    });

    it("should hide the tooltip instance after the specified delay", () => {
        const instance = {
            show: vi.fn(),
            hide: vi.fn()
        };
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return instance;
            }
        };
        (bs.Tooltip as any).getInstance = () => instance;
        tooltip = new Tooltip(element);
        tooltip.delayedHide(1000);

        vi.advanceTimersByTime(1000);
        expect(instance.hide).toHaveBeenCalledTimes(1);
    });

    it("should not throw an error if element is null", () => {
        tooltip = new Tooltip(null);
        expect(() => tooltip.delayedHide(1000)).not.toThrow();
    });

    it("should not hide if no existing instance", () => {
        tooltip = new Tooltip(element);
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return null as any;
            }
        };
        (bs.Tooltip as any).getInstance = () => null as any;
        tooltip.delayedHide(1000);

        vi.advanceTimersByTime(1000);
        expect(() => tooltip.hide()).not.toThrow();
    });

    it("should use default delay if no delay is specified", () => {
        const instance = {
            show: vi.fn(),
            hide: vi.fn()
        };
        const bs = (window as any)["bootstrap"] = {
            Tooltip: function() {
                return instance;
            }
        };
        (bs.Tooltip as any).getInstance = () => instance;
        tooltip = new Tooltip(element);
        tooltip.delayedHide();

        vi.advanceTimersByTime(5000);
        expect(instance.hide).toHaveBeenCalledTimes(1);
    });
});

describe("Tooltip.isAvailable", () => {
    it("should return true if bootstrap tooltip is available", () => {
        const bs = (window as any)["bootstrap"] = {
            Tooltip: vi.fn()
        };
        expect(Tooltip.isAvailable).toBe(true);
    });

    it("should return true if jQuery tooltip is available", () => {
        mockJQuery({
            tooltip: vi.fn()
        });
        expect(Tooltip.isAvailable).toBe(true);
    });

    it("should return false if neither bootstrap nor jQuery tooltip is available", () => {
        (window as any)["bootstrap"] = undefined;
        (window as any)["jQuery"] = undefined;
        expect(Tooltip.isAvailable).toBe(false);
    });

    it("should return false if only bootstrap is defined but tooltip is not available", () => {
        const bs = (window as any)["bootstrap"] = {};
        expect(Tooltip.isAvailable).toBe(false);
    });

    it("should return false if only jQuery is defined but tooltip is not available", () => {
        const $ = (window as any)["jQuery"] = vi.fn().mockReturnValue({
            fn: {}
        });
        expect(Tooltip.isAvailable).toBe(false);
    });
});
