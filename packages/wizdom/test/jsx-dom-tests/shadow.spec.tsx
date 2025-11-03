/** @jsxImportSource ../../src */
import { createRef, ShadowRootNode } from "../../src"

describe("shadow", () => {

  it("ShadowRoot should not expose HTML", () => {
    expect(
      (
        <div>
          <ShadowRootNode mode="open">world</ShadowRootNode>
        </div>
      ).outerHTML
    ).toBe("<div></div>")
  })

  it("supports mode `open`", () => {
    const div = (
      <div>
        <ShadowRootNode mode="open">
          <i>world</i>
        </ShadowRootNode>
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
        <ShadowRootNode mode="closed" ref={ref}>
          <b>world</b>
        </ShadowRootNode>
      </div>
    ) as HTMLDivElement

    expect(div.shadowRoot).to.be.null
    expect(ref.current!.innerHTML).toBe("<b>world</b>")
  })

  it("supports ref on ShadowRoot", () => {
    const ref1 = createRef<ShadowRoot>()
    Object(
      <div>
        <ShadowRootNode ref={ref1} mode="open">
          world
        </ShadowRootNode>
      </div>
    )

    expect(ref1.current).to.be.instanceOf(ShadowRoot)

    const ref2 = vi.fn<any>()
    Object(
      <div>
        <ShadowRootNode ref={ref2} mode="open">
          world
        </ShadowRootNode>
      </div>
    )

    expect(ref2.mock.calls).toHaveLength(1)
    expect(ref2.mock.calls[0]).to.have.lengthOf(1)
    expect(ref2.mock.calls[0][0]).to.be.instanceOf(ShadowRoot)
  })
})
