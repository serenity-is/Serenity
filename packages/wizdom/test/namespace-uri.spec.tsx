import { inHTMLNamespace, inMathMLNamespace, inNamespaceURI, inSVGNamespace, MathMLNamespace, SVGNamespace, type JSXElement } from "../src";

describe("supports namespaceURI attribute", () => {
    it("allows setting namespaceURI on elements", () => {
        expect((<a namespaceURI={SVGNamespace} />).namespaceURI).to.equal(SVGNamespace);
    });

    it("allows setting namespaceURI on SVG elements", () => {
        expect((<svg namespaceURI={SVGNamespace} />).namespaceURI).to.equal(SVGNamespace);
    });

    it("defaults SVG elements to SVG namespace", () => {
        expect((<svg />).namespaceURI).to.equal(SVGNamespace);
    });

    it("defaults MathML elements to MathML namespace", () => {
        expect((<math />).namespaceURI).to.equal(MathMLNamespace);
    });

    it("defaults HTML elements to no namespace", () => {
        expect((<div />).namespaceURI).to.equal("http://www.w3.org/1999/xhtml");
    });
});

describe("namespace URI context helpers", () => {
    it("inSVGNamespace sets namespace to SVG for children", () => {
        const circle = inSVGNamespace(() => <circle />);
        expect((circle as Element).namespaceURI).to.equal(SVGNamespace);

        const a = inSVGNamespace(() => <a />);
        expect((a as Element).namespaceURI).to.equal(SVGNamespace);
    });

    it("inMathMLNamespace sets namespace to MathML for children", () => {
        const mrow = inMathMLNamespace(() => <mrow />);
        expect((mrow as Element).namespaceURI).to.equal(MathMLNamespace);
    });

    it("inHTMLNamespace sets namespace to HTML for children", () => {
        const div = inHTMLNamespace(() => <div />);
        expect((div as Element).namespaceURI).to.equal("http://www.w3.org/1999/xhtml");
    });

    it("nested namespace contexts work correctly", () => {
        let svg: JSXElement, math: JSXElement, div: JSXElement;
        inSVGNamespace(() =>
            inMathMLNamespace(() =>
                inHTMLNamespace(() => <>
                    <svg ref={el => svg = el} />
                    <math ref={el => math = el} />
                    <a ref={el => div = el} />
                </>)
            )
        );

        expect(svg.namespaceURI).to.equal(SVGNamespace);
        expect(math.namespaceURI).to.equal(MathMLNamespace);
        expect(div.namespaceURI).to.equal("http://www.w3.org/1999/xhtml");
    });

    it("svg only tags take precedence in other namespaces", () => {
        let circle: JSXElement;
        inHTMLNamespace(() => <circle ref={el => circle = el} />);
        expect(circle.namespaceURI).to.equal(SVGNamespace);
    });

    it("return value can be used as JSX children", () => {
        const svg = (
            <svg>
                {inMathMLNamespace(() => <math />)}
            </svg>
        ); 
        expect(svg.namespaceURI).to.equal(SVGNamespace);
        expect(svg.children[0].namespaceURI).to.equal(MathMLNamespace);
    });

    it("inNamespaceURI allows custom namespace", () => {
        const customNS = "http://example.com/custom";
        const div = inNamespaceURI(customNS, () => <div />);
        expect((div as Element).namespaceURI).to.equal(customNS);
    });

    it("explicit namespaceURI attribute overrides context", () => {
        const div = inSVGNamespace(() => <div namespaceURI="http://example.com/custom" />);
        expect((div as Element).namespaceURI).to.equal("http://example.com/custom");
    });

    it("context is properly restored after function calls", () => {
        // First call sets SVG context
        inSVGNamespace(() => <circle />);
        
        // Second call should not be affected by first
        const div = inHTMLNamespace(() => <div />);
        expect((div as Element).namespaceURI).to.equal("http://www.w3.org/1999/xhtml");
    });

    it("handles null namespaceURI in inNamespaceURI", () => {
        const div = inNamespaceURI(null, () => <div />);
        expect((div as Element).namespaceURI).to.equal("http://www.w3.org/1999/xhtml");
    });

    it("multiple elements in single context call", () => {
        const result = inSVGNamespace(() => <>
            <circle />
            <rect />
            <a />
        </>);
        
        const elements = Array.from((result as any).childNodes) as Element[];
        expect(elements[0].namespaceURI).to.equal(SVGNamespace); // circle
        expect(elements[1].namespaceURI).to.equal(SVGNamespace); // rect  
        expect(elements[2].namespaceURI).to.equal(SVGNamespace); // a (ambiguous, gets context)
    });
});
