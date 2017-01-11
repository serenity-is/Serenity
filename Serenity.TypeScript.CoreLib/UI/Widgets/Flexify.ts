declare namespace Serenity {
    class Flexify extends Widget<FlexifyOptions> {
        constructor(container: JQuery, options: FlexifyOptions);
    }

    interface FlexifyOptions {
        getXFactor?: (p1: JQuery) => any;
        getYFactor?: (p1: JQuery) => any;
        designWidth?: any;
        designHeight?: any;
    }

    namespace FLX {
        function flexHeightOnly(element: JQuery, flexY?: number): JQuery;
        function flexWidthOnly(element: JQuery, flexX?: number): JQuery;
        function flexWidthHeight(element: JQuery, flexX?: number, flexY?: number): JQuery;
        function flexXFactor(element: JQuery, flexX: number): JQuery;
        function flexYFactor(element: JQuery, flexY: number): JQuery;
    }
}