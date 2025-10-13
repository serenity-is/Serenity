/** @jsxImportSource ../../src */
import { describe, expect, it } from "vitest"
import { useClassList, useRef, useText } from "../../src"

describe("hooks", () => {
    it("supports useText", () => {
        const [text, setText] = useText("Initial value")
        const div = <div>{text}</div>
        expect(div.children).to.include(text)
        expect(div.textContent).toBe("Initial value")
        setText("Second iteration")
        expect(text.textContent).toBe("Second iteration")
        expect(div.textContent).toBe("Second iteration")
    })

    it("supports useRef", () => {
        const ref = useRef<HTMLButtonElement>()
        expect(ref).toHaveProperty("current", null)

        const div = (
            <div>
                <button ref={ref} />
            </div>
        )
        expect(ref).not.toBeNull()
        expect(div.children[0]).toBe(ref.current)

        const ref2 = useRef<HTMLInputElement>();
        (<input ref={ref2} />).toString()
        expect(ref2).not.toBeNull()
        expect(ref2.current).toHaveProperty("tagName", "INPUT")
    })

    it("supports useClassList", () => {
        const cls = useClassList()
        cls.add("me")
        const div = <div class={cls} />
        expect(div.className).toBe("me")
        expect(cls.size).toBe(1)

        cls.add("second")
        expect(div.className).toBe("me second")
        expect(div.className).toBe(cls.value)
        expect(cls.size).toBe(2)

        cls.remove("me")
        expect(div.className).toBe("second")

        cls.add("container")
        expect(cls.contains("container")).toBe(true)

        cls.toggle("never")
        expect(cls.contains("never")).toBe(true)

        cls.toggle("never")
        expect(cls.contains("never")).toBe(false)

        cls.toggle("container", false)
        expect(cls.contains("container")).toBe(false)
    })

    it("SVGElement supports useClassList", () => {
        const cls = useClassList()
        cls.add("me")
        const svg = <svg class={cls} />
        expect(svg.classList.value).toBe("me")
        expect(cls.size).toBe(1)

        cls.add("second")
        expect(svg.classList.value).toBe("me second")
        expect(svg.classList.value).toBe(cls.value)
        expect(cls.size).toBe(2)

        cls.remove("me")
        expect(svg.classList.value).toBe("second")

        cls.add("container")
        expect(cls.contains("container")).toBe(true)

        cls.toggle("never")
        expect(cls.contains("never")).toBe(true)

        cls.toggle("never")
        expect(cls.contains("never")).toBe(false)

        cls.toggle("container", false)
        expect(cls.contains("container")).toBe(false)
    })

    it("supports signal like as attribute value", () => {
        const signal = { peek: vi.fn(() => "signal:initialvalue"), subscribe: vi.fn(function (callback) { this.callback = callback }), get value() { return "test" }, callback: (value: any) => void { } }
        const div = <div title={signal as any} />
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.getAttribute("title")).toBe("signal:initialvalue")
        expect(typeof signal.callback).toBe("function")
        signal.callback("signal:changedvalue")
        expect(div.getAttribute("title")).toBe("signal:changedvalue");
    })

    it("supports signal as sole content", () => {
        const signal = { peek: vi.fn(() => "signal:initialvalue"), subscribe: vi.fn(function (callback) { this.callback = callback }), get value() { return "test" }, callback: (value: any) => void { } }         
        const div = <div>{signal as any}</div>
        expect(signal.peek).toHaveBeenCalledOnce();
        expect(signal.subscribe).toHaveBeenCalledOnce();
        expect(div.textContent).toBe("signal:initialvalue")
        expect(typeof signal.callback).toBe("function")
        signal.callback("signal:changedvalue")
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
    })
})