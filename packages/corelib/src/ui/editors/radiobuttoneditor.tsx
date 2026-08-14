import { Enum, EnumKeyAttribute, getCustomAttribute, getType, getTypeFullName, isPromiseLike, localText, nsSerenity } from "../../base";
import { getLookup } from "../../compat";
import { IReadOnly, IStringValue } from "../../interfaces";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";
import { EditorProps, EditorWidget } from "./editorwidget";

export interface RadioButtonEditorOptions {
    enumKey?: string;
    enumType?: any;
    lookupKey?: string;
}

export class RadioButtonEditor<P extends RadioButtonEditorOptions = RadioButtonEditorOptions> extends EditorWidget<P> implements IReadOnly {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue, IReadOnly]);

    declare private _pendingValue: string;

    constructor(props: EditorProps<P>) {
        super(props);

        if (!this.options.enumKey &&
            this.options.enumType == null &&
            !this.options.lookupKey) {
            return;
        }

        if (this.options.lookupKey) {
            var lookup = getLookup(this.options.lookupKey);
            for (var item of lookup.items) {
                var textValue = (item as any)[lookup.textField];
                var text = (textValue == null ? '' : textValue.toString());
                var idValue = (item as any)[lookup.idField];
                var id = (idValue == null ? '' : idValue.toString());
                this.addRadio(id, text);
            }
        }
        else {
            var enumType = this.options.enumType || EnumTypeRegistry.getOrLoad(this.options.enumKey);
            const then = (enumType: any) => {
                var enumKey = this.options.enumKey;
                if (enumKey == null && enumType != null) {
                    enumKey = getCustomAttribute(enumType, EnumKeyAttribute, false)?.value ??
                        getTypeFullName(enumType);
                }

                var values = this.getEnumValues(enumType);
                for (var x of values) {
                    var name = Enum.toString(enumType, x);
                    this.addRadio(x.toString(), localText("Enums." + enumKey + "." + name, name));
                }

                // radios are ready now; apply any value that was set while the enum was still loading
                if (this._pendingValue != null)
                    this.set_value(this._pendingValue);

                this._pendingValue = void 0;
            }
            if (isPromiseLike(enumType))
                enumType.then(then);
            else
                then(enumType);
        }
    }

    protected getEnumValues(enumType: any): any[] {
        var values = Enum.getValues(enumType);
        if (values.length || enumType == null)
            return values;

        // Enum.getValues only returns numeric values, so for string enums collect the string members directly.
        return Object.keys(enumType)
            .filter(k => typeof enumType[k] === "string")
            .map(k => enumType[k]);
    }

    protected addRadio(value: string, text: string): void {
        this.domNode.appendChild(<label><input type="radio" name={this.uniqueName} id={this.uniqueName + '_' + value} value={value} />{text}</label>);
    }

    get_value(): string {
        if (this.domNode.querySelector('input'))
            return this.element.findFirst('input:checked').val();

        // radios are not ready yet (async enum load); return the preserved value
        return this._pendingValue;
    }

    get value(): string {
        return this.get_value();
    }

    set_value(value: string): void {
        this._pendingValue = value;

        if (!this.domNode.querySelector('input'))
            return; // not loaded yet; the preserved value is applied once radios are ready

        if (value === this.get_value())
            return;

        var inputs = this.element.findAll<HTMLInputElement>('input');
        var checks = inputs.filter(x => x.checked);
        if (checks.length > 0) {
            (checks[0] as HTMLInputElement).checked = false;
        }
        if (value) {
            checks = inputs.filter(x => (x as HTMLInputElement).value === value);
            if (checks.length > 0) {
                (checks[0] as HTMLInputElement).checked = true;
            }
        }
    }

    set value(v: string) {
        this.set_value(v);
    }

    get_readOnly(): boolean {
        return this.domNode.getAttribute("disabled") != null;
    }

    set_readOnly(value: boolean): void {
        if (this.get_readOnly() !== !!value) {
            this.element.attr("disabled", value ? 'disabled' : null)
                .findEach('input[type=radio]', x => x.attr('disabled', value ? 'disabled' : null));
        }
    }

}