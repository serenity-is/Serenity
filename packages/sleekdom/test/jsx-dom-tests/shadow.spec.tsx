/** @jsxImportSource ../../src */
import { createRef, ShadowRoot as Shadow } from "../../src"

describe("shadow", () => {

  it("ShadowRoot should not expose HTML", () => {
    expect(
      (
        <div>
          <Shadow mode="open">world</Shadow>
        </div>
      ).outerHTML
    ).toBe("<div></div>")
  })

  it("supports mode `open`", () => {
    const div = (
      <div>
        <Shadow mode="open">
          <i>world</i>
        </Shadow>
      </div>
    ) as HTMLDivElement

    expect(div.shadowRoot).to.exist
    expect(div.shadowRoot.mode).toBe("open")
    expect(div.shadowRoot.innerHTML).toBe("<i>world</i>")
  })

  it("supports mode `closed`", () => {
    const ref = createRef<ShadowRoot>()

    const div = (
      <div>
        <Shadow mode="closed" ref={ref}>
          <b>world</b>
        </Shadow>
      </div>
    ) as HTMLDivElement

    expect(div.shadowRoot).to.be.null
    expect(ref.current!.innerHTML).toBe("<b>world</b>")
  })

  it("supports ref on ShadowRoot", () => {
    const ref1 = createRef<ShadowRoot>()
    Object(
      <div>
        <Shadow ref={ref1} mode="open">
          world
        </Shadow>
      </div>
    )

    expect(ref1.current).to.be.instanceOf(ShadowRoot)

    const ref2 = vi.fn<any>()
    Object(
      <div>
        <Shadow ref={ref2} mode="open">
          world
        </Shadow>
      </div>
    )

    expect(ref2.mock.calls).toHaveLength(1)
    expect(ref2.mock.calls[0]).to.have.lengthOf(1)
    expect(ref2.mock.calls[0][0]).to.be.instanceOf(ShadowRoot)
  })
})
