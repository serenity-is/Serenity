import { Tooltip } from "./tooltip";
import { mockJQuery, unmockBSAndJQuery } from "../mocks";

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
        const tooltipSpy = jest.fn();
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
            show: jest.fn(),
            hide: jest.fn()
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
            show: jest.fn(),
            hide: jest.fn()
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
            show: jest.fn(),
            hide: jest.fn()
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
            show: jest.fn(),
            hide: jest.fn()
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
            show: jest.fn(),
            hide: jest.fn(),
            _fixTitle: jest.fn(),
            tip: {
                querySelector: jest.fn().mockReturnValue({ textContent: "" })
            },
            update: jest.fn()
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
