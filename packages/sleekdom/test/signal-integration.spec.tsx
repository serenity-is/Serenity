/** @jsxImportSource ../src */
import { describe, expect, it } from "vitest";
import type { SignalLike } from "../src/types/signal-like";

function testSignal<T>(initialValue: T): SignalLike<T> {
    const signal = {
        currentValue: initialValue,
        peek: vi.fn(() => signal.currentValue) as () => T,
        dispose: vi.fn() as () => void,
        subscribe: vi.fn(function (callback) { signal.callback = callback; return signal.dispose; }),
        get value() { return signal.currentValue },
        set value(val: T) { if (signal.currentValue !== val) { signal.currentValue = val; signal.callback?.(val); } },
        callback: (value: any) => void {}
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

    it("does not touch manually set properties when signal value changes", () => {
        const signal = testSignal<any>({ color: "red" });
        const div = <div style={signal} />
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect((div.style).color).toBe("red");
        div.style.backgroundColor = "blue";
        signal.value = { color: "green" };
        expect((div.style).color).toBe("green");
        expect((div.style).backgroundColor).toBe("blue");
    });

    it("supports signal like as classname value", () => {
        const signal = testSignal({ active: true, disabled: false, highlighted: true });
        const div = <div class={signal} />
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.classList.contains("active")).toBe(true);
        expect(div.classList.contains("disabled")).toBe(false);
        expect(div.classList.contains("highlighted")).toBe(true);
        signal.value = { active: false, disabled: true, highlighted: true };
        expect(div.classList.contains("active")).toBe(false);
        expect(div.classList.contains("disabled")).toBe(true);
        expect(div.classList.contains("highlighted")).toBe(true);
    });

    it("supports signal like as classname individual entries", () => {
        const signalActive = testSignal(true);
        const signalDisabled = testSignal(false);
        const signalHighlighted = testSignal(true);
        const div = <div class={{ active: signalActive, disabled: signalDisabled, highlighted: signalHighlighted }} />
        expect(signalActive.peek).toHaveBeenCalledOnce();
        expect(signalActive.subscribe).toHaveBeenCalledOnce();
        expect(signalDisabled.peek).toHaveBeenCalledOnce();
        expect(signalDisabled.subscribe).toHaveBeenCalledOnce();
        expect(signalHighlighted.peek).toHaveBeenCalledOnce();
        expect(signalHighlighted.subscribe).toHaveBeenCalledOnce();
        expect(div.classList.contains("active")).toBe(true);
        expect(div.classList.contains("disabled")).toBe(false);
        expect(div.classList.contains("highlighted")).toBe(true);
        signalActive.value = false;
        signalDisabled.value = true;
        signalHighlighted.value = false;
        expect(div.classList.contains("active")).toBe(false);
        expect(div.classList.contains("disabled")).toBe(true);
        expect(div.classList.contains("highlighted")).toBe(false);
    });

    it("supports a mix of signal and non-signal classname entries", () => {
        const signalActive = testSignal(true);
        const div = <div class={{ active: signalActive, disabled: false, highlighted: true }} />
        expect(signalActive.peek).toHaveBeenCalledOnce();
        expect(signalActive.subscribe).toHaveBeenCalledOnce();
        expect(div.classList.contains("active")).toBe(true);
        expect(div.classList.contains("disabled")).toBe(false);
        expect(div.classList.contains("highlighted")).toBe(true);
        signalActive.value = false;
        expect(div.classList.contains("active")).toBe(false);
        expect(div.classList.contains("disabled")).toBe(false);
        expect(div.classList.contains("highlighted")).toBe(true);
    });

    it("supports signal like array as classname value", () => {
        const signal = testSignal(["active", "highlighted"]);
        const div = <div class={signal} />
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.classList.contains("active")).toBe(true);
        expect(div.classList.contains("highlighted")).toBe(true);
        expect(div.classList.contains("disabled")).toBe(false);
        signal.value = ["disabled"];
        expect(div.classList.contains("active")).toBe(false);
        expect(div.classList.contains("highlighted")).toBe(false);
        expect(div.classList.contains("disabled")).toBe(true);
    });

    it("supports an array of signal and non-signal classname entries", () => {
        const signalActive = testSignal<string | boolean>("active");
        const signalDisabled = testSignal<string | boolean>(false);
        const div = <div class={ [signalDisabled as any, signalActive as any, "highlighted"] } />
        expect(signalActive.peek).toHaveBeenCalledOnce();
        expect(signalActive.subscribe).toHaveBeenCalledOnce();
        expect(signalDisabled.peek).toHaveBeenCalledOnce();
        expect(signalDisabled.subscribe).toHaveBeenCalledOnce();
        expect(div.classList.contains("active")).toBe(true);
        expect(div.classList.contains("highlighted")).toBe(true);
        expect(div.classList.contains("disabled")).toBe(false);
        signalActive.value = false;
        signalDisabled.value = "disabled";
        expect(div.classList.contains("active")).toBe(false);
        expect(div.classList.contains("highlighted")).toBe(true);
        expect(div.classList.contains("disabled")).toBe(true);
    });

    it("does not touch manually added classnames when signal value changes", () => {
        const signal = testSignal(["active"]);
        const div = <div class={signal} />
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.classList.contains("active")).toBe(true);
        div.classList.add("highlighted");
        signal.value = ["disabled"];
        expect(div.classList.contains("active")).toBe(false);
        expect(div.classList.contains("disabled")).toBe(true);
        expect(div.classList.contains("highlighted")).toBe(true);
    });
})