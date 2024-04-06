import { Enum, Fluent, getCustomAttribute, tryGetText } from "../../base";
import { IReadOnly, IStringValue } from "../../interfaces";
import { getLookup } from "../../q";
import { EnumKeyAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";
import { EditorProps, EditorWidget } from "../widgets/widget";

export interface RadioButtonEditorOptions {
    enumKey?: string;
    enumType?: any;
    lookupKey?: string;
}

@Decorators.registerEditor('Serenity.RadioButtonEditor', [IStringValue, IReadOnly])
export class RadioButtonEditor<P extends RadioButtonEditorOptions = RadioButtonEditorOptions> extends EditorWidget<P>
    implements IReadOnly {

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
            var enumType = this.options.enumType || EnumTypeRegistry.get(this.options.enumKey);
            var enumKey = this.options.enumKey;
            if (enumKey == null && enumType != null) {
                var enumKeyAttr = getCustomAttribute(enumType, EnumKeyAttribute, false);
                if (enumKeyAttr) {
                    enumKey = enumKeyAttr.value;
                }
            }

            var values = Enum.getValues(enumType);
            for (var x of values) {
                var name = Enum.toString(enumType, x);
                this.addRadio(x.toString(), tryGetText(
                    'Enums.' + enumKey + '.' + name) ?? name);
            }
        }
    }

    protected addRadio(value: string, text: string) {
        var label = Fluent("label").text(text);
        Fluent("input").attr("type", "radio").attr('name', this.uniqueName)
            .attr('id', this.uniqueName + '_' + value)
            .attr('value', value).prependTo(label);
        label.appendTo(this.domNode);
    }

    get_value(): string {
        return this.element.findFirst('input:checked').val();
    }

    get value(): string {
        return this.get_value();
    }

    set_value(value: string): void {
        if (value !== this.get_value()) {
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
    }

    set value(v: string) {
        this.set_value(v);
    }

    get_readOnly(): boolean {
        return this.domNode.getAttribute("disabled") != null;
    }

    set_readOnly(value: boolean): void {
        if (this.get_readOnly() !== value) {
            if (value) {
                this.element.attr('disabled', 'disabled')
                    .findFirst('input').attr('disabled', 'disabled');
            }
            else {
                this.element.removeAttr('disabled')
                    .findFirst('input').removeAttr('disabled');
            }
        }
    }

}