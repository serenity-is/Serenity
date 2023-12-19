import { EditorProps, Widget, WidgetNode, WidgetProps } from "./widget";

function _widgetFactory(this: any, props: any) {

    var widget = Widget.create({
        type: this,
        options: props
    });

    if (props.ref)
        props.ref(widget);

    return widget.render();
}

type JsxDomWidgetProps<P> = EditorProps<P> & {
    children?: any | undefined;
    class?: string;
}

export interface JsxDomWidget<P = {}, TElement extends Element = HTMLElement> {
    (props: JsxDomWidgetProps<P>, context?: any): TElement | null
}

export function jsxDomWidget<TWidget extends Widget<TOptions>, TOptions>(
    type: (new (element: WidgetNode, options?: TOptions) => Widget<TOptions>) | 
          (new (options?: TOptions) => TWidget)): JsxDomWidget<TOptions & { ref?: (r: TWidget) => void }> {
    return _widgetFactory.bind(type);
}

