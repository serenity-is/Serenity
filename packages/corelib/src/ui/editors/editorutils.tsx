﻿import { Fluent, PropertyItem, isArrayLike, isInstanceOfType, localText, parseDecimal, setElementReadOnly, tryGetText } from "../../base";
import { IBooleanValue, IDoubleValue, IGetEditValue, ISetEditValue, IStringValue, IValidateRequired } from "../../interfaces";
import { cast, isTrimmedEmpty, safeCast } from "../../q";
import { type Widget } from "../widgets/widget";
import { tryGetWidget } from "../widgets/widgetutils";
import { Combobox } from "./combobox";

export namespace EditorUtils {

    export function getDisplayText(editor: Widget<any>): string {

        var combobox = Combobox.getInstance(editor.domNode);

        if (combobox) {
            var data = combobox.getSelectedItems();
            if (!data)
                return '';

            return data.map(x => x.text).join(", ");
        }

        var value = getValue(editor);
        if (value == null) {
            return '';
        }

        if (typeof value === "string")
            return value;

        if (value instanceof Boolean)
            return (!!value ? (tryGetText('Controls.FilterPanel.OperatorNames.true') ?? 'True') :
                (tryGetText('Controls.FilterPanel.OperatorNames.true') ?? 'False'));

        return value.toString();
    }

    var dummy: PropertyItem = { name: '_' };

    export function getValue(editor: Widget<any>): any {
        var target: Record<string, any> = {};
        saveValue(editor, dummy, target);
        return target['_'];
    }

    export function saveValue(editor: Widget<any>, item: PropertyItem, target: any): void {

        var getEditValue = safeCast(editor, IGetEditValue);

        if (getEditValue != null) {
            getEditValue.getEditValue(item, target);
            return;
        }

        var stringValue = safeCast(editor, IStringValue);
        if (stringValue != null) {
            target[item.name] = stringValue.get_value();
            return;
        }

        var booleanValue = safeCast(editor, IBooleanValue);
        if (booleanValue != null) {
            target[item.name] = booleanValue.get_value();
            return;
        }

        var doubleValue = safeCast(editor, IDoubleValue);
        if (doubleValue != null) {
            var value = doubleValue.get_value();
            target[item.name] = (isNaN(value) ? null : value);
            return;
        }

        if ((editor as any).getEditValue != null) {
            (editor as any).getEditValue(item, target);
            return;
        }

        if (Fluent.isInputLike(editor.domNode)) {
            target[item.name] = editor.domNode.value;
            return;
        }
    }

    export function setValue(editor: Widget<any>, value: any): void {
        var source = { _: value };
        loadValue(editor, dummy, source);
    }

    export function loadValue(editor: Widget<any>, item: PropertyItem, source: any): void {

        var setEditValue = safeCast(editor, ISetEditValue);
        if (setEditValue != null) {
            setEditValue.setEditValue(source, item);
            return;
        }

        var stringValue = safeCast(editor, IStringValue);
        if (stringValue != null) {
            var value = source[item.name];
            if (value != null) {
                value = value.toString();
            }
            stringValue.set_value(cast(value, String));
            return;
        }

        var booleanValue = safeCast(editor, IBooleanValue);
        if (booleanValue != null) {
            var value1 = source[item.name];
            if (typeof (value1) === 'number') {
                booleanValue.set_value(value1 > 0);
            }
            else {
                booleanValue.set_value(!!value1);
            }
            return;
        }

        var doubleValue = safeCast(editor, IDoubleValue);
        if (doubleValue != null) {
            var d = source[item.name];
            if (!!(d == null || isInstanceOfType(d, String) && isTrimmedEmpty(cast(d, String)))) {
                doubleValue.set_value(null);
            }
            else if (isInstanceOfType(d, String)) {
                doubleValue.set_value(cast(parseDecimal(cast(d, String)), Number));
            }
            else if (isInstanceOfType(d, Boolean)) {
                doubleValue.set_value((!!d ? 1 : 0));
            }
            else {
                doubleValue.set_value(cast(d, Number));
            }
            return;
        }

        if ((editor as any).setEditValue != null) {
            (editor as any).setEditValue(source, item);
            return;
        }

        if (Fluent.isInputLike(editor.domNode)) {
            var v = source[item.name];
            editor.domNode.value = v ?? '';
            return;
        }
    }

    /**
     * This functions sets readonly class and disabled (for select, radio, checkbox) or readonly attribute (for other inputs) on given elements
     * or widgets. If a widget is passed and it has set_readOnly method it is called instead of setting readonly class or attributes.
     * Note that if an element, instead of the widget attached to it is passed directly, this searchs for a widget attached to it.
     * If you don't want this behavior, use setElementReadOnly method.
     * @param elements 
     * @param value 
     */
    export function setReadonly(elements: Element | Widget<any> | ArrayLike<Element | Widget>, value: boolean) {
        elements = isArrayLike(elements) ? elements : [elements];
        for (var i = 0; i < elements.length; i++) {
            let el = elements[i];
            if (el == null)
                continue;

            if (!(el instanceof Node)) {
                if (typeof (el as any).set_readOnly === "function") {
                    (el as any).set_readOnly(!!value);
                    continue;
                }
                el = el.domNode;
                if (!el)
                    continue;
            }
            else {
                const widget = tryGetWidget(el);
                if (widget != null && typeof (widget as any).set_readOnly === "function") {
                    (widget as any).set_readOnly(!!value);
                    continue;
                }
            }

            setElementReadOnly(el, value);
        }
    }

    /**
     * Legacy alias for setReadonly
     */
    export const setReadOnly = setReadonly;

    export function setRequired(widget: Widget<any>, isRequired: boolean): void {
        var req = safeCast(widget, IValidateRequired);
        if (req != null) {
            req.set_required(isRequired);
        }
        else if (Fluent.isInputLike(widget.domNode)) {
            widget.domNode.classList.toggle('required', !!isRequired);
        }
        var gridField = widget.domNode.closest('.field');
        var hasSupItem = gridField?.querySelector('sup');
        if (isRequired && !hasSupItem && gridField) {
            Fluent(<sup title={localText('Controls.PropertyGrid.RequiredHint')}>*</sup>)
                .prependTo(gridField.querySelector('.caption'));
        }
        else if (!isRequired && hasSupItem) {
            Fluent(hasSupItem).remove();
        }
    }

    export function setContainerReadOnly(container: ArrayLike<HTMLElement> | HTMLElement, readOnly: boolean) {

        container = isArrayLike(container) ? container[0] : container;
        if (!readOnly) {

            if (!container.classList.contains('readonly-container'))
                return;

            container.classList.remove('readonly-container');
            container.querySelectorAll(".editor.container-readonly").forEach(el => {
                el.classList.remove('container-readonly');
                EditorUtils.setReadOnly(el, false);
            });

            return;
        }

        container.classList.add('readonly-container');
        container.querySelectorAll(".editor:not(.container-readonly)").forEach((el: HTMLElement) => {
            var w = tryGetWidget(el) as any;
            if (w != null) {
                if (w['get_readOnly']) {
                    if (w['get_readOnly']())
                        return;
                }
                else if (el.matches('[readonly]') || el.matches('[disabled]') || el.matches('.readonly') || el.matches('.disabled'))
                    return;

                el.classList.add('container-readonly');
                EditorUtils.setReadOnly(w, true);
            }
            else {
                if (el.matches('[readonly]') || el.matches('[disabled]') || el.matches('.readonly') || el.matches('.disabled'))
                    return;

                el.classList.add('container-readonly');
                setElementReadOnly(el, true);
            }
        });
    }
}