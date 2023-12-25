import { EditorProps, Widget } from "./widget";

function _widgetFactory(this: any, props: any) {
    return new this(props).render();
}

type JsxDomWidgetProps<P> = EditorProps<P> & {
    children?: any | undefined;
    class?: string;
}

export interface JsxDomWidget<P = {}, TElement extends Element = HTMLElement> {
    (props: JsxDomWidgetProps<P>, context?: any): TElement | null
}

export function jsxDomWidget<TWidget extends Widget<TOptions>, TOptions>(
    type: (new (element: ArrayLike<HTMLElement>, options?: TOptions) => Widget<TOptions>) | 
          (new (options?: TOptions) => TWidget)): JsxDomWidget<TOptions & { ref?: (r: TWidget) => void }> {
    return _widgetFactory.bind(type);
}

