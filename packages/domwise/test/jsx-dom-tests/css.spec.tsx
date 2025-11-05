import type { StyleProperties } from "../../types"

describe("CSS", () => {
    it("supports numeric CSS properties", () => {
        const test = (key: keyof StyleProperties) =>
            expect((<div style={{ [key]: 2 } as any} />).style[key]).toBe("2px")

        test("paddingTop")
        test("fontSize")
        test("marginBottom")
    })

    const testUnitless = (key: string) =>
        expect((<div style={{ [key]: 5 }} />).style[key]).to.be.oneOf([5, "5", key], key)

    it("supports unitless CSS properties", () => {
        [
            "animationIterationCount",
            "borderImageOutset",
            "borderImageSlice",
            "borderImageWidth",
            "boxFlex",
            "boxFlexGroup",
            "boxOrdinalGroup",
            "columnCount",
            "columns",
            //"flex", => returns 5 1 0%
            "flexGrow",
            "flexPositive",
            "flexShrink",
            "flexNegative",
            "flexOrder",
            "gridArea",
            "gridRow",
            "gridRowEnd",
            "gridRowSpan",
            "gridRowStart",
            "gridColumn",
            "gridColumnEnd",
            "gridColumnSpan",
            "gridColumnStart",
            "fontWeight",
            "lineClamp",
            "lineHeight",
            //"opacity", can't be 5
            "order",
            "orphans",
            "tabSize",
            "widows",
            "zIndex",
            "zoom",
        ].forEach(key => testUnitless(key))
    })

    it("supports unitless CSS properties with vendor prefixes", () => {
        // Vendor prefix
        testUnitless("WebkitFlex")
        testUnitless("MozFlex")
    })

    it("supports CSS custom properties (variables)", () => {
        const test = (key: string, value: string) =>
            expect((<div style={{ [key]: value }} />).style.getPropertyValue(key)).toBe(value)

        test("--my-variable", "1")
        test("--my-variable", "1px")
        test("--my-variable", "anystring")
        test("--myVariable", "1")
        test("--myVariable", "1px")
        test("--myVariable", "anystring")
    })
})
