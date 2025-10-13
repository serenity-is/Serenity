/** @jsxImportSource ../../src */
import { describe, expect, it } from "vitest";

describe("signal integration", () => {

    it("supports signal like as attribute value", () => {
        const signal = { peek: vi.fn(() => "signal:initialvalue"), subscribe: vi.fn(function (callback) { this.callback = callback }), get value() { return "test" }, callback: (value: any) => void { } }
        const div = <div title={signal as any} />
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.getAttribute("title")).toBe("signal:initialvalue")
        expect(typeof signal.callback).toBe("function");
        signal.callback("signal:changedvalue");
        expect(div.getAttribute("title")).toBe("signal:changedvalue");
    });

    it("supports signal as sole content", () => {
        const signal = { peek: vi.fn(() => "signal:initialvalue"), subscribe: vi.fn(function (callback) { this.callback = callback }), get value() { return "test" }, callback: (value: any) => void { } }         
        const div = <div>{signal as any}</div>
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.textContent).toBe("signal:initialvalue");
        expect(typeof signal.callback).toBe("function");
        signal.callback("signal:changedvalue");
        expect(div.textContent).toBe("signal:changedvalue");
    });

    it("supports signal inside a fragment", () => {
        const signal = { peek: vi.fn(() => "signal:initialvalue"), subscribe: vi.fn(function (callback) { this.callback = callback }), get value() { return "test" }, callback: (value: any) => void { } }         
        const div = <div><><span>before</span>{signal as any}<span>after</span></></div>
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.textContent).toBe("beforesignal:initialvalueafter")
        expect(typeof signal.callback).toBe("function")
        div.firstChild.remove();
        signal.callback("signal:changedvalue")
        expect(div.textContent).toBe("signal:changedvalueafter");
    });
})