import { EditorUtils } from "../editors/editorutils";
import { ReflectionOptionsSetter } from "./reflectionoptionssetter";
import { Widget, WidgetComponentProps } from "./widget";

function _widgetFactory(this: any, props: any) {

    var widget = Widget.create({
        type: this,
        element: (el$) => {
            var el = el$[0];
            if (props.id != null)
                el.id = props.id;

            if (props.name != null)
                el.setAttribute('name', props.name);

            if (props.placeholder != null)
                el.setAttribute("placeholder", props.placeholder);

            if (props.className != null)
                el.className = props.className;
            else if (props.class != null)
                el.className = props.class;

            if (props.oneWay)
                el.dataset.oneWay = "1";

            if (props.maxLength != null)
                el.setAttribute("maxLength", props.maxLength || 0);
            else if (props.maxlength != null)
                el.setAttribute("maxLength", props.maxlength || 0);
        }
    });

    if (props.required != null)
        EditorUtils.setRequired(widget, props.required);

    if (props.readOnly !== null)
        EditorUtils.setReadOnly(widget, props.readOnly);
    else if (props.readonly !== null)
        EditorUtils.setReadOnly(widget, props.readonly);

    if (props.setOptions != props.setOptions)
        ReflectionOptionsSetter.set(widget, props.setOptions);

    if (props.value !== undefined)
        EditorUtils.setValue(widget, props.value);
    else if (props.initialValue != undefined)
        EditorUtils.setValue(widget, props.initialValue);

    if (props.onChange != null)
        widget.element.on('change', props.onChange);
    else if (props.onchange != null)
        widget.element.on('change', props.onchange);

    if (props.onChangeSelect2 != null)
        widget.changeSelect2(props.onChangeSelect2);

    if (props.ref)
        props.ref(widget);

    return widget.element[0];
}

type JsxDomWidgetProps<P> = P & WidgetComponentProps<any> & {
    children?: any | undefined;
    class?: string;
}

export interface JsxDomWidget<P = {}, TElement extends Element = HTMLElement> {
    (props: JsxDomWidgetProps<P>, context?: any): TElement | null
}

export function jsxDomWidget<TWidget extends Widget<TOptions>, TOptions>(
    type: new (element: JQuery, options?: TOptions) => TWidget): JsxDomWidget<TOptions & { ref?: (r: TWidget) => void }> {
    return _widgetFactory.bind(type);
}

