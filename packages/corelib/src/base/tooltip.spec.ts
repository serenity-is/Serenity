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