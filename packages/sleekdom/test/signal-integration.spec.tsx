/** @jsxImportSource ../src */
import { describe, expect, it } from "vitest";
import { invokeDisposingListeners } from "../src/disposing-listener";
import { mockSignal } from "./mocks/mock-signal";
import { forAllDescendants } from "./mocks/test-helpers";

describe("signal integration", () => {

    it("supports signal like as attribute value", () => {
        const signal = mockSignal("signal:initialvalue");
        const div = <div title={signal} />
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.getAttribute("title")).toBe("signal:initialvalue")
        signal.value = "signal:changedvalue";
        expect(div.getAttribute("title")).toBe("signal:changedvalue");
    });

    it("supports signal as sole content", () => {
        const signal = mockSignal("signal:initialvalue");
        const div = <div>{signal}</div>
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.textContent).toBe("signal:initialvalue");
        signal.value = "signal:changedvalue";
        expect(div.textContent).toBe("signal:changedvalue");
    });

    it("supports signal inside a fragment", () => {
        const signal = mockSignal("signal:initialvalue");
        const div = <div><><span>before</span>{signal}<span>after</span></></div>
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.textContent).toBe("beforesignal:initialvalueafter")
        div.firstChild.remove();
        signal.value = "signal:changedvalue";
        expect(div.textContent).toBe("signal:changedvalueafter");
    });

    it("supports signal as style property value", () => {
        const signal = mockSignal("red");
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
        const signal = mockSignal("red");
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
        const signal = mockSignal({ color: "red", backgroundColor: "blue" });
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
        const signal = mockSignal<any>({ color: "red", backgroundColor: "blue" });
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
        const signal = mockSignal<any>({ color: "red" });
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
        const signal = mockSignal({ active: true, disabled: false, highlighted: true });
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
        const signalActive = mockSignal(true);
        const signalDisabled = mockSignal(false);
        const signalHighlighted = mockSignal(true);
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
        const signalActive = mockSignal(true);
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
        const signal = mockSignal(["active", "highlighted"]);
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
        const signalActive = mockSignal<string | boolean>("active");
        const signalDisabled = mockSignal<string | boolean>(false);
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
        const signal = mockSignal(["active"]);
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

    it("disposes signal subscriptions when element is disposed", () => {
        const signal = mockSignal("signal:initialvalue");
        const div = <div>{signal}</div>
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.textContent).toBe("signal:initialvalue");
        forAllDescendants(div, invokeDisposingListeners);
        expect(signal.unsubscribe).toHaveBeenCalledOnce();
    });
})