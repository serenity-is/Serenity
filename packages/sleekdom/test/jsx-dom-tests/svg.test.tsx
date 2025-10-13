/** @jsxImportSource ../../src */
import { SVGNamespace } from "../../src/jsx-impl"

describe("SVG", () => {
  const namespace = SVGNamespace

  it("exports the correct SVG namespace URI", () => {
    expect(namespace).to.equal("http://www.w3.org/2000/svg")
  })

  it("supports SVG elements", () => {
    const supportedElements = [
      <svg />,
      <animate />,
      <circle />,
      <clipPath />,
      <defs />,
      <desc />,
      <ellipse />,
      <feBlend />,
      <feColorMatrix />,
      <feComponentTransfer />,
      <feComposite />,
      <feConvolveMatrix />,
      <feDiffuseLighting />,
      <feDisplacementMap />,
      <feDistantLight />,
      <feFlood />,
      <feFuncA />,
      <feFuncB />,
      <feFuncG />,
      <feFuncR />,
      <feGaussianBlur />,
      <feImage />,
      <feMerge />,
      <feMergeNode />,
      <feMorphology />,
      <feOffset />,
      <fePointLight />,
      <feSpecularLighting />,
      <feSpotLight />,
      <feTile />,
      <feTurbulence />,
      <filter />,
      <foreignObject />,
      <g />,
      <image />,
      <line />,
      <linearGradient />,
      <marker />,
      <mask />,
      <metadata />,
      <path />,
      <pattern />,
      <polygon />,
      <polyline />,
      <radialGradient />,
      <rect />,
      <stop />,
      <switch />,
      <symbol />,
      <text />,
      <textPath />,
      <tspan />,
      <use />,
      <view />,
    ]

    supportedElements.forEach(one =>
      expect(one.namespaceURI, `Tag: ${one.tagName}`).to.equal(namespace)
    )
  })

  it("supports SVG namespace", () => {
    expect((<a namespaceURI={namespace} />).namespaceURI).to.equal(namespace)
  })

  //it("supports xlink and XML attributes", () => {
  //  const checkXLink = (node: JSX.Element, attr: string) =>
  //    expect(node.getAttributeNS("http://www.w3.org/1999/xlink", attr) === "value")
  //
  //  const checkXML = (node: JSX.Element, attr: string) =>
  //    expect(node.getAttributeNS("http://www.w3.org/XML/1998/namespace", attr) === "value")
  //
  //  checkXLink(<use xlinkHref="value" />, "xlink:href")
  //  checkXLink(<use xlinkArcrole="value" />, "xlink:arcrole")
  //  checkXLink(<use xlinkHref="value" />, "xlink:href")
  //  checkXLink(<use xlinkRole="value" />, "xlink:role")
  //  checkXLink(<use xlinkShow="value" />, "xlink:show")
  //  checkXLink(<use xlinkTitle="value" />, "xlink:title")
  //  checkXLink(<use xlinkType="value" />, "xlink:type")
  //  checkXML(<use xmlBase="value" />, "xml:base")
  //  checkXML(<use xmlLang="value" />, "xml:lang")
  //  checkXML(<use xmlSpace="value" />, "xml:space")
  //})

  it("supports unitless CSS properties", () => {
    const test = (key: string) =>
      expect((<div style={{ [key]: 5 }} />).style[key]).to.be.oneOf([5, "5"], key)
    ;[
      "fillOpacity",
      "floodOpacity",
      "stopOpacity",
      "strokeDasharray",
      "strokeDashoffset",
      "strokeMiterlimit",
      "strokeOpacity",
      "strokeWidth",
    ].forEach(key => test(key))
  })

  it("supports alphanumeric SVG attributes", () => {
    const el = <line x1={0} y1={0} x2={20} y2={20} /> as SVGLineElement

    expect(el.attributes.length).to.equal(4)

    for (const attr of ["x1", "y1", "x2", "y2"]) {
      expect(el.hasAttribute(attr)).to.be.true
    }
  })

  it("supports presentation SVG attributes", () => {
    const Path = "path" as any;
    const element = (
      <svg>
        <Path
          accentHeight="auto"
          alignmentBaseline="auto"
          arabicForm="initial"
          baselineShift="auto"
          capHeight="auto"
          clipPath="auto"
          clipRule="auto"
          colorInterpolation="auto"
          colorInterpolationFilters="auto"
          colorProfile="auto"
          colorRendering="auto"
          dominantBaseline="auto"
          enableBackground="auto"
          fillOpacity="auto"
          fillRule="inherit"
          floodColor="auto"
          floodOpacity="auto"
          fontFamily="auto"
          fontSize="auto"
          fontSizeAdjust="auto"
          fontStretch="auto"
          fontStyle="auto"
          fontVariant="auto"
          fontWeight="auto"
          glyphName="auto"
          glyphOrientationHorizontal="auto"
          glyphOrientationVertical="auto"
          horizAdvX="auto"
          horizOriginX="auto"
          imageRendering="auto"
          letterSpacing="auto"
          lightingColor="auto"
          markerEnd="auto"
          markerMid="auto"
          markerStart="auto"
          overlinePosition="auto"
          overlineThickness="auto"
          panose1="auto"
          paintOrder="auto"
          pointerEvents="auto"
          renderingIntent="auto"
          shapeRendering="auto"
          stopColor="auto"
          stopOpacity="auto"
          strikethroughPosition="auto"
          strikethroughThickness="auto"
          strokeDasharray="auto"
          strokeDashoffset="auto"
          strokeLinecap="inherit"
          strokeLinejoin="inherit"
          strokeMiterlimit="auto"
          strokeOpacity="auto"
          strokeWidth="auto"
          textAnchor="auto"
          textDecoration="auto"
          textRendering="auto"
          underlinePosition="auto"
          underlineThickness="auto"
          unicodeBidi="auto"
          unicodeRange="auto"
          unitsPerEm="auto"
          vAlphabetic="auto"
          vHanging="auto"
          vIdeographic="auto"
          vMathematical="auto"
          vectorEffect="auto"
          vertAdvY="auto"
          vertOriginX="auto"
          vertOriginY="auto"
          wordSpacing="auto"
          writingMode="auto"
          xHeight="auto"
        />
      </svg>
    )

    ;[...(element.firstElementChild.attributes as any as Array<Attr>)]
      .map(x => x.name)
      .forEach(x => expect(/^[^A-Z]*$/.test(x)).to.be.true)
  })

  it("supports non-presentation svg attributes", () => {
    const Path = "path" as any;
    const element = (
      <svg>
        <Path
          allowReorder="yes"
          attributeName="auto"
          attributeType="auto"
          autoReverse={true}
          baseFrequency="auto"
          baseProfile="auto"
          calcMode="auto"
          clipPathUnits="auto"
          contentScriptType="auto"
          contentStyleType="auto"
          diffuseConstant="auto"
          edgeMode="auto"
          externalResourcesRequired={true}
          filterRes="auto"
          filterUnits="auto"
          glyphRef="auto"
          gradientTransform="auto"
          gradientUnits="auto"
          kernelMatrix="auto"
          kernelUnitLength="auto"
          keyPoints="auto"
          keySplines="auto"
          keyTimes="auto"
          lengthAdjust="auto"
          limitingConeAngle="auto"
          markerHeight="auto"
          markerUnits="auto"
          markerWidth="auto"
          maskContentUnits="auto"
          maskUnits="auto"
          numOctaves="auto"
          pathLength="auto"
          patternContentUnits="auto"
          patternTransform="auto"
          patternUnits="auto"
          pointsAtX="auto"
          pointsAtY="auto"
          pointsAtZ="auto"
          preserveAlpha={true}
          preserveAspectRatio="auto"
          primitiveUnits="auto"
          refX="auto"
          refY="auto"
          repeatCount="auto"
          repeatDur="auto"
          requiredExtensions="auto"
          requiredFeatures="auto"
          specularConstant="auto"
          specularExponent="auto"
          spreadMethod="auto"
          startOffset="auto"
          stdDeviation="auto"
          stitchTiles="auto"
          surfaceScale="auto"
          systemLanguage="auto"
          tableValues="auto"
          targetX="auto"
          targetY="auto"
          textLength="auto"
          viewBox="auto"
          viewTarget="auto"
          xChannelSelector="auto"
          yChannelSelector="auto"
          zoomAndPan="auto"
        />
      </svg>
    )

    ;[...(element.firstElementChild.attributes as any as Array<Attr>)]
      .map(x => x.name)
      .forEach(x => expect(x).to.not.include("-"))
  })
})
