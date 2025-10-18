/** @jsxImportSource ../src */
import { describe, expect, it } from "vitest";

function testSignal<T>(initialValue: T) {
    const signal = {
        currentValue: initialValue,
        peek: vi.fn(() => signal.currentValue) as () => T,
        dispose: vi.fn() as () => void,
        subscribe: vi.fn(function (callback) { signal.callback = callback; return signal.dispose; }),
        get value() { return signal.currentValue },
        set value(val: T) { if (signal.currentValue !== val) { signal.currentValue = val; signal.callback?.(val); } },
        callback: (value: any) => void { }
    };
    return signal;
}

describe("signal integration", () => {

    it("supports signal like as attribute value", () => {
        const signal = testSignal("signal:initialvalue");
        const div = <div title={signal} />
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.getAttribute("title")).toBe("signal:initialvalue")
        signal.value = "signal:changedvalue";
        expect(div.getAttribute("title")).toBe("signal:changedvalue");
    });

    it("supports signal as sole content", () => {
        const signal = testSignal("signal:initialvalue");
        const div = <div>{signal}</div>
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.textContent).toBe("signal:initialvalue");
        signal.value = "signal:changedvalue";
        expect(div.textContent).toBe("signal:changedvalue");
    });

    it("supports signal inside a fragment", () => {
        const signal = testSignal("signal:initialvalue");
        const div = <div><><span>before</span>{signal}<span>after</span></></div>
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.textContent).toBe("beforesignal:initialvalueafter")
        div.firstChild.remove();
        signal.value = "signal:changedvalue";
        expect(div.textContent).toBe("signal:changedvalueafter");
    });

    it("supports signal as style property value", () => {
        const signal = testSignal("red");
        const div = <div style={{ color: signal }} />
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect((div.style).color).toBe("red");
        signal.value = "blue";
        expect((div.style).color).toBe("blue");
        signal.value = "green";
        expect((div.style).color).toBe("green");
    });

    it("supports a mix of signal and non-signal style property value", () => {
        const signal = testSignal("red");
        const div = <div style={{ color: signal, backgroundColor: "blue" }} />
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect((div.style).color).toBe("red");
        expect((div.style).backgroundColor).toBe("blue");
        signal.value = "green";
        expect((div.style).color).toBe("green");
        expect((div.style).backgroundColor).toBe("blue");
    });

    it("supports signal as style object", () => {
        const signal = testSignal({ color: "red", backgroundColor: "blue" });
        const div = <div style={signal} />
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect((div.style).color).toBe("red");
        expect((div.style).backgroundColor).toBe("blue");
        signal.value = { color: "green", backgroundColor: "yellow" };
        expect((div.style).color).toBe("green");
        expect((div.style).backgroundColor).toBe("yellow");
    });

    it("clears previously set style properties when signal style value set to false", () => {
        const signal = testSignal<any>({ color: "red", backgroundColor: "blue" });
        const div = <div style={signal} />
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect((div.style).color).toBe("red");
        expect((div.style).backgroundColor).toBe("blue");
        signal.value = false;
        expect((div.style).color).toBe("");
        expect((div.style).backgroundColor).toBe("");
    })
})