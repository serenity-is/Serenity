/** @jsxImportSource ../../src */
import { describe, expect, it } from "vitest"
import * as lib from "../../src"

describe("tests from jsx-dom", () => {
    it("creates a <div> element", () => {
        expect((<div id="hello">world</div>).outerHTML).toBe('<div id="hello">world</div>')
    })

    describe("supports publicly declared APIs", () => {
        it("supports createFactory", () => {
            expect(lib.createElement).to.be.a("function")
            expect(lib.createFactory).to.be.a("function")
            expect(lib.createRef).to.be.a("function")
            expect(lib.jsx).to.be.a("function")
            expect(lib.useCallback).to.be.a("function")
            expect(lib.useMemo).to.be.a("function")
            expect(lib.useRef).to.be.a("function")
            expect(lib.useText).to.be.a("function")

            const Div = lib.createFactory("div")
            expect((<Div>div tag</Div>).tagName).toBe("DIV")

            const element = lib.jsx("div", { children: "test" })

            function CustomComponent(props: any): any {
                if (!new.target) return new (CustomComponent as any)(props)
            }
            expect(<CustomComponent />).toBeInstanceOf(CustomComponent)
        })
    })

    it("supports functional components", () => {
        function Component(props: { a: 1; b: 2; c: 3 }) {
            expect(props.a).toBe(1)
            expect(props.b).toBe(2)
            expect(props.c).toBe(3)
            expect((props as any).children).toBeUndefined();
            return <div>{props.a + props.b + props.c}</div>
        }

        expect((<Component a={1} b={2} c={3} />).innerHTML).toBe("6")
    })

    it("supports class components", () => {
        class MyComponent extends lib.Component<{ a: number; b: number; c: number }> {
            constructor(props: { a: 1; b: 2; c: 3 }) {
                super(props)
            }

            render() {
                expect(this.props.a).toBe(1)
                expect(this.props.b).toBe(2)
                expect(this.props.c).toBe(3)
                expect(this.props.children).toBeUndefined();
                return <div>{this.props.a + this.props.b + this.props.c}</div>
            }
        }

        expect((<MyComponent a={1} b={2} c={3} />).innerHTML).toBe("6")
    })

    it("supports dynamic tagname", () => {
        const TagName = "div"
        expect((<TagName />).tagName).toBe(TagName.toUpperCase())
    })

    it("supports React automatic runtime", () => {
        expect(
            lib.jsx("div", {
                className: "className",
            }).outerHTML
        ).toBe('<div class="className"></div>')

        expect(
            lib.jsx("div", {
                className: "className",
                children: "One child",
            }).outerHTML
        ).toBe('<div class="className">One child</div>')

        expect(
            lib.jsx("div", {
                className: "className",
                children: ["One child", lib.jsx("div", { className: "child", children: "Two children" })],
            }).outerHTML
        ).toBe('<div class="className">One child<div class="child">Two children</div></div>')
    })

    describe("childNodes", () => {
        it("ignores `null`, `undefined`, `true` and `false`", () => {
            expect((<div>{null}</div>).children).to.be.empty
            expect((<div>{undefined}</div>).children).to.be.empty
            expect((<div>{true}</div>).children).to.be.empty
            expect((<div>{false}</div>).children).to.be.empty
        })

        it("supports deep nested childNodes", () => {
            expect((<div>{[2, 3]}</div>).textContent).toBe("23")
            expect((<div>{[2, [2, "3", null, false, [4]]]}</div>).textContent).toBe("2234")
        })

        it("supports DOM elements as childNode", () => {
            const img = document.createElement("img")
            const node = <div>{img}</div>
            expect(node.children).to.have.lengthOf(1)
            expect(node.firstElementChild).toBe(img)
        })

        it("supports string as childNode", () => {
            expect((<div>{"text"}</div>).textContent).toBe("text")
        })


        it("supports Node as childNode", () => {
            const span = document.createElement("span")
            const img = document.createElement("img")
            span.append(img)

            const node = <div>{span.firstChild}</div>
            expect(node.children).to.have.lengthOf(1)
            expect(node.firstElementChild).toBe(img)
        })

        it("supports NodeList as childNode", () => {
            const span = document.createElement("span")
            const img = document.createElement("img")
            span.append(img)

            const node = <div>{span.childNodes}</div>
            expect(node.children).to.have.lengthOf(1)
            expect(node.firstElementChild).toBe(img)
        })

        it("supports HTMLCollection as childNode", () => {
            const span = document.createElement("span")
            const img = document.createElement("img")
            span.append(img)

            const node = <div>{span.children}</div>
            expect(node.children).to.have.lengthOf(1)
            expect(node.firstElementChild).toBe(img)
        })

        it("children must be copied before iteration", () => {
            const span = document.createElement("span")
            const img = document.createElement("img")
            const code = document.createElement("code")
            span.append(img, code)

            const node = <div>{span.children}</div>
            expect(node.children).to.have.lengthOf(2)
            expect(node.firstElementChild).toBe(img)
            expect(node.lastElementChild).toBe(code)
        })

        it("supports passing `children` explicitly", () => {
            expect((<div children="internal" />).textContent).toBe("internal")
            expect((<div children={["internal", 20]} />).textContent).toBe("internal20")
        })

        it("will not override JSX childNodes with `children` attribute", () => {
            expect((<div {...{ children: "i" }}>override</div>).textContent).toBe("override")
        })
    })

    describe("className", () => {
        it("accepts both `class`, `className` as valid input for classes.", () => {
            expect((<div className="me irl" />).className).toBe("me irl")
            expect((<div class="me too thanks" />).className).toBe("me too thanks")
        })

        it("accepts an array as valid input for class", () => {
            expect((<div class={["first", "second"]} />).className).toBe("first second")
        })

        it("expects a recursive array as valid input for classes", () => {
            expect((<div class={["first", ["second", false, "third"]]} />).className).toBe(
                "first second third"
            )
        })

        it("accepts an object literal as a valid input", () => {
            const node = (
                <div
                    class={{
                        excluded: false,
                        included: true,
                    }}
                />
            )

            expect(node.className).toBe("included")
        })

        it("filters out falsy values, but not 0, from the class array", () => {
            const node = <div class={[Math.PI < 3 && "PI < 3?", [].length && "should be 0", "rest"]} />

            expect(node.className).toBe("0 rest")
        })

        it("accepts a DOMTokenList as classname", () => {
            const base = (<div class="base" />) as HTMLDivElement
            expect(base.classList).toBeInstanceOf(DOMTokenList)

            const node = <div class={base.classList} />
            expect(node.className).toBe("base")
        })
    })

    describe("attributes", () => {
        describe.skip("supports boolean properties", () => {
            class MyCustomElement extends HTMLElement {
                isBoolean: boolean
                constructor() {
                    super()
                }
            }

            const BooleanTest = "boolean-test" as any;
            customElements.define(BooleanTest, MyCustomElement)
            it("shoud attach truly attribute as property", () => {
                expect((<BooleanTest isBoolean={true} /> as any).isBoolean).toBe(true)
            })

            it("shoud attach falsy attribute as property", () => {
                expect((<BooleanTest isBoolean={false} /> as any).isBoolean).toBe(false)
            })
        })

        it("supports complex objects attributes as properties", () => {
            class MyCustomElement extends HTMLElement {
                constructor() {
                    super()
                }
            }
            const RichDataTest = "rich-data-test" as any;
            customElements.define(RichDataTest, MyCustomElement)
            const richData = { foo: "bar" }
            expect((<RichDataTest richData={richData} /> as any).richData).toBe(richData)
        })

        it("supports boolean attributes", () => {
            expect((<input disabled={true} />).getAttribute("disabled")).toBe("")
            expect((<input disabled={false} />).getAttribute("disabled")).toBe(null)
        })

        it("supports dataset", () => {
            expect((<div data-key="value" />).dataset.key).toBe("value")
            expect((<div dataset={{ key: "0" }} />).getAttribute("data-key")).toBe("0")
        })

        it("suppresses null / undefined dataset", () => {
            expect({
                ...(<div dataset={{ key: "", value: null, data: undefined }} />).dataset,
            }).to.deep.equal({
                key: "",
            })
        })

        it("supports innerHTML, innerText and textContent", () => {
            //don't support
            //expect((<div innerHTML="<div></div><div></div>" />).querySelectorAll("div").length).toBe(2)
            //expect((<div innerText="<img>" />).querySelectorAll("img")).to.be.empty
            //expect((<div innerText="<img>" />).textContent).toBe("<img>")
            expect((<div textContent="<img>" />).querySelectorAll("img")).to.be.empty
        })

        it("supports dangerouslySetInnerHTML", () => {
            expect(
                (<div dangerouslySetInnerHTML={{ __html: "<div></div><div></div>" }} />).querySelectorAll(
                    "div"
                ).length
            ).toBe(2)

            expect(
                (
                    <svg dangerouslySetInnerHTML={{ __html: "<path></path><path></path>" }} />
                ).querySelectorAll("path").length
            ).toBe(2)
        })

        it("supports ref store function", () => {
            let button = null
            const div = (
                <div>
                    <button ref={e => (button = e)} />
                </div>
            )
            expect(button).not.toBeNull()
            expect(div.children[0]).toBe(button)
        })

        it("supports React style createRef.", () => {
            const ref = lib.createRef()
            expect(ref).toHaveProperty("current", null)

            const div = (
                <div>
                    <button ref={ref} />
                </div>
            );
            expect(ref).not.toBeNull();
            expect(div.children[0]).toBe(ref.current);

            (<input ref={ref} /> as HTMLInputElement).focus();;

            expect(ref).not.toBeNull()
            expect(ref.current).toHaveProperty("tagName", "INPUT")
        })

        it("supports ref in functional components", () => {
            let button = null
            const Button = ({ ref }) => (
                <span>
                    <button ref={ref} />
                </span>
            )
            const div = (
                <div>
                    <Button ref={e => (button = e)} />
                </div>
            )
            expect(button).not.toBeNull()
            expect(div.children[0].children[0]).toBe(button)
            expect(button).toBeInstanceOf(HTMLButtonElement)
        })

        describe("supports forwardRef", () => {
            it("element", () => {
                const Container = lib.forwardRef<HTMLButtonElement, any>((props, ref) => (
                    <div {...props}>
                        <button ref={ref}>Button</button>
                    </div>
                ))

                const ref = lib.createRef()
                const node = (
                    <Container className="container" ref={ref}>
                        Click me!
                    </Container>
                )
                expect(node.className).toBe("container")
                expect(ref.current).toBeInstanceOf(HTMLButtonElement)
            })

            it("component", () => {
                const Button = props => <button {...props} />
                const Container = lib.forwardRef<HTMLButtonElement, any>((props, ref) => (
                    <div {...props}>
                        <Button ref={ref}>Button</Button>
                    </div>
                ))

                const ref = lib.createRef()
                const node = (
                    <Container className="container" ref={ref}>
                        Click me!
                    </Container>
                )
                expect(node.className).toBe("container")
                expect(ref.current).toBeInstanceOf(HTMLButtonElement)
            })

            // #104
            it("class component with ref", () => {
                class Button extends lib.Component<{ className: string }> {
                    render() {
                        return <button className={this.props.className} />
                    }
                }

                const ref = lib.createRef<Button>()
                const node = (
                    <Button className="container" ref={ref}>
                        Click me!
                    </Button>
                )
                expect(node.className).toBe("container")
                expect(ref.current).toBeInstanceOf(Button)
            })
        })

        it("supports useImperativeHandle", () => {
            const Button = lib.forwardRef<{ focus: () => string }, any>((props, ref) => {
                lib.useImperativeHandle(ref, () => ({
                    focus: () => "ping",
                }))
                return <button {...props} />
            })

            const ref = lib.createRef<HTMLButtonElement>()
            const node = (
                <Button className="container" ref={ref}>
                    Click me!
                </Button>
            )
            expect(node.className).toBe("container")
            expect(ref.current).toHaveProperty("focus")
            expect(ref.current.focus()).toBe("ping")
        })

        it("supports defaultProps in functional components", () => {
            const Button = (props: any) => <div className={props.className} />
            Button.defaultProps = { className: "defaultClass" }
            const button = <Button />
            expect(button.className).toBe("defaultClass")
        })

        it("supports defaultProps in class components", () => {
            class Button extends lib.Component<{ className: string }> {
                static defaultProps = { className: "defaultClass" }
                render() {
                    return <div className={this.props.className} />
                }
            }
            // @ts-ignore
            const button = <Button />
            expect(button.className).toBe("defaultClass")
        })

        it("supports spellCheck attribute", () => {
            <label for=""></label>
            expect((<input spellCheck={true} /> as HTMLInputElement).spellcheck).toBe(true)
            expect((<input spellCheck={false} /> as HTMLInputElement).spellcheck).toBe(false)
            expect((<textarea spellCheck={true} /> as HTMLTextAreaElement).spellcheck).toBe(true)
            expect((<textarea spellCheck={false} /> as HTMLTextAreaElement).spellcheck).toBe(false)
        })

        it("supports value attribute on textarea", () => {
            expect((<textarea value="Test" /> as HTMLTextAreaElement).value).toBe("Test")
            expect((<textarea value={undefined} /> as HTMLTextAreaElement).value).toBe("")
        })

        it("supports value attribute on select", () => {
            const select = (
                <select value="B">
                    <option value="A"></option>
                    <option value="B"></option>
                    <option value="C"></option>
                </select>
            ) as HTMLSelectElement

            expect(select.querySelector<HTMLOptionElement>("[value=B]").selected).toBe(true)

            const selectDeep = (
                <select value={["C"]}>
                    <optgroup label="Group 1">
                        <option value="A"></option>
                    </optgroup>
                    <optgroup label="Group 2">
                        <option value="B" selected></option>
                        <option value="C"></option>
                    </optgroup>
                </select>
            ) as HTMLSelectElement

            expect(selectDeep.querySelector<HTMLOptionElement>("[value=C]").selected).toBe(true)
        })

        it("supports value attribute on select with multiple", () => {
            const select = (
                <select multiple value={["B", "C"]}>
                    <option value="A" selected></option>
                    <option value="B" selected></option>
                    <option value="C"></option>
                </select>
            ) as HTMLSelectElement

            expect(select.querySelector<HTMLOptionElement>("[value=A]").selected).toBe(false)
            expect(select.querySelector<HTMLOptionElement>("[value=B]").selected).toBe(true)
            expect(select.querySelector<HTMLOptionElement>("[value=C]").selected).toBe(true)

            const selectSingle = (
                <select multiple value={"C"}>
                    <option value="A"></option>
                    <option value="B" selected></option>
                    <option value="C" selected></option>
                </select>
            ) as HTMLSelectElement

            expect(selectSingle.querySelector<HTMLOptionElement>("[value=A]").selected).toBe(false)
            expect(selectSingle.querySelector<HTMLOptionElement>("[value=B]").selected).toBe(false)
            expect(selectSingle.querySelector<HTMLOptionElement>("[value=C]").selected).toBe(true)

            const selectDeep = (
                <select multiple value={["A", "B"]}>
                    <optgroup label="Group 1">
                        <option value="A"></option>
                    </optgroup>
                    <optgroup label="Group 2">
                        <option value="B" selected></option>
                        <option value="C"></option>
                    </optgroup>
                </select>
            ) as HTMLSelectElement

            expect(selectDeep.querySelector<HTMLOptionElement>("[value=A]").selected).toBe(true)
            expect(selectDeep.querySelector<HTMLOptionElement>("[value=B]").selected).toBe(true)
            expect(selectDeep.querySelector<HTMLOptionElement>("[value=C]").selected).toBe(false)
        })
    })

    describe("styles", () => {
        it("supports style object", () => {
            expect((<div style={{ display: "none" }} />).style.display).toBe("none")
        })

        it("supports style string", () => {
            const el = <div style="display: none; margin: 1px;" />
            expect(el.style.display).toBe("none")
            expect(el.style.margin).toBe("1px")
        })
    })

    const _it =
        (name: string, fn: (resolve: () => void, reject: (reason?: any) => void) => void) => () =>
            it(name, () => new Promise<void>(fn))

    describe("events", () => {
        _it("supports event listeners", done => {
            const button = (<button onClick={() => done()} />) as HTMLButtonElement
            button.click()
        })

        _it("supports capture event listeners", (done, reject) => {
            const button = (
                <button
                    onClickCapture={event => {
                        event.stopImmediatePropagation()
                        done()
                    }}
                    onClick={() => reject("`onClickCapture` was not called")}
                />
            ) as HTMLButtonElement
            button.click()
        })

        it("uses `element.on...` properties", () => {
            const button = (<button onClick={() => void 0} />) as HTMLButtonElement
            expect(button.onclick).to.be.a("function")
        })

        it("uses addEventListener", () => {
            const button = (<button {...{ onCustomEvent: () => void 0 } as any} />) as HTMLButtonElement
            // @ts-expect-error checking not existing property
            expect(button.oncustomevent).to.not.be.a("function")
            // @ts-expect-error checking not existing property
            expect(button.customEvent).to.not.be.a("function")
        })

        _it("supports custom events", done => {
            const button = (<button {...{ onCustomEvent: () => done() } as any} />) as HTMLButtonElement
            button.dispatchEvent(new window.Event("customEvent"))
        })

        _it("supports event listeners using `on` attribute", done => {
            const button = (<button on={{ click: () => done() }} />) as HTMLButtonElement
            button.click()
        })

        _it("supports capture event listeners using `onCapture` attribute", (done, reject) => {
            const button = (
                <button
                    onCapture={{
                        click: event => {
                            event.stopImmediatePropagation()
                            done()
                        },
                    }}
                    onClick={() => reject("`onCapture` was not called")}
                />
            ) as HTMLButtonElement
            button.click()
        })

        _it("supports custom events using `on` attribute", done => {
            const button = (<button on={{ CustomEvent: () => done() }} />) as HTMLButtonElement
            button.dispatchEvent(new window.Event("CustomEvent"))
        })
        _it("maps onDoubleClick to dblclick event", done => {
            const button = (<button onDoubleClick={() => done()} />) as HTMLButtonElement
            button.dispatchEvent(new window.Event("dblclick"))
        })
        _it("maps onDblClick to dblclick event", done => {
            const button = (<button onDblClick={() => done()} />) as HTMLButtonElement
            button.dispatchEvent(new window.Event("dblclick"))
        })
        _it("maps onDoubleClickCapture to dblclick event", done => {
            const button = (<button onDoubleClickCapture={ () => done()} />) as HTMLButtonElement
            button.dispatchEvent(new window.Event("dblclick"))
        })
        _it("maps onDblClickCapture to dblclick event", done => {
            const button = (<button onDblClickCapture={() => done()} />) as HTMLButtonElement
            button.dispatchEvent(new window.Event("dblclick"))
        })
    })

    describe("forwardRef", () => {
        it("supports forwardRef", () => {
            // const FancyButton = lib.forwardRef((props, ref) => (
            const FancyButton = props => (
                <button ref={props.ref} className="FancyButton">
                    {props.children}
                </button>
            )

            // You can now get a ref directly to the DOM button:
            const ref = lib.createRef();
            (<FancyButton ref={ref}>Click me!</FancyButton>).toString();
            expect(ref.current).toBeInstanceOf(HTMLButtonElement)
        })
    })

    describe("fragment", () => {
        it("supports fragments", () => {
            const frag = (
                <>
                    {[2]}
                    <span>Bonjour</span>
                </>
            )
            const nodes = frag.childNodes
            expect(nodes[0].nodeType).toBe(Node.TEXT_NODE)
            expect(nodes[0].textContent).toBe("2")
            expect(nodes[1].nodeName).toBe("SPAN")
            expect(nodes[1].textContent).toBe("Bonjour")
        })

        it("supports fragments with explicit tag", () => {
            const frag = (
                <lib.Fragment>
                    {[2]    }
                    <span>Bonjour</span>
                </lib.Fragment>
            )
            const nodes = frag.childNodes
            expect(nodes[0].nodeType).toBe(Node.TEXT_NODE)
            expect(nodes[0].textContent).toBe("2")
            expect(nodes[1].nodeName).toBe("SPAN")
            expect(nodes[1].textContent).toBe("Bonjour")
        })

        it("supports fragments with one child", () => {
            const frag = (
                <>
                    <span>Text</span>
                </>
            )
            const nodes = frag.childNodes
            expect(nodes).to.have.lengthOf(1)
            expect(nodes[0].nodeName).toBe("SPAN")
            expect(nodes[0].textContent).toBe("Text")
        })

        it("supports fragments as child", () => {
            const frag = (
                <div>
                    <>
                        <span>Text</span>
                    </>
                </div>
            )
            const nodes = frag.childNodes
            expect(nodes).to.have.lengthOf(1)
            expect(nodes[0].nodeName).toBe("SPAN")
            expect(nodes[0].textContent).toBe("Text")
        })
    })

    describe("web component events", () => {
        it("can be defined as a web component", () => {
            customElements.define("web-component", class WebComponent extends HTMLElement { })
        })
    })

    describe("templates", () => {
        it("supports templates", () => {
            const template = (
                <template>
                    {[2]}
                    <span>Bonjour</span>
                </template>
            ) as HTMLTemplateElement

            const nodes = template.content.childNodes
            expect(nodes[0].nodeType).toBe(Node.TEXT_NODE)
            expect(nodes[0].textContent).toBe("2")
            expect(nodes[1].nodeName).toBe("SPAN")
            expect(nodes[1].textContent).toBe("Bonjour")
        })
    })
})
