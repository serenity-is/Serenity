namespace Serenity {

    export interface RadioButtonEditorOptions {
        enumKey?: string;
        enumType?: any;
        lookupKey?: string;
    }

    @Decorators.registerEditor('Serenity.RadioButtonEditor', [IStringValue, IReadOnly])
    @Decorators.element('<div/>')
    export class RadioButtonEditor extends Widget<RadioButtonEditorOptions>
        implements IReadOnly {

        constructor(input: JQuery, opt: RadioButtonEditorOptions) {
            super(input, opt);

            if (Q.isEmptyOrNull(this.options.enumKey) &&
                this.options.enumType == null &&
                Q.isEmptyOrNull(this.options.lookupKey)) {
                return;
            }

            if (!Q.isEmptyOrNull(this.options.lookupKey)) {
                var lookup = Q.getLookup(this.options.lookupKey);
                for (var item of lookup.items) {
                    var textValue = item[lookup.textField];
                    var text = (textValue == null ? '' : textValue.toString());
                    var idValue = item[lookup.idField];
                    var id = (idValue == null ? '' : idValue.toString());
                    this.addRadio(id, text);
                }
            }
            else {
                var enumType = this.options.enumType || Serenity.EnumTypeRegistry.get(this.options.enumKey);
                var enumKey = this.options.enumKey;
                if (enumKey == null && enumType != null) {
                    var enumKeyAttr = Q.getAttributes(enumType, Serenity.EnumKeyAttribute, false);
                    if (enumKeyAttr.length > 0) {
                        enumKey = enumKeyAttr[0].value;
                    }
                }

                var values = Q.Enum.getValues(enumType);
                for (var x of values) {
                    var name = Q.Enum.toString(enumType, x);
                    this.addRadio(x.toString(), Q.coalesce(Q.tryGetText(
                        'Enums.' + enumKey + '.' + name), name));
                }
            }
        }

        protected addRadio(value: string, text: string) {
            var label = $('<label/>').text(text);
            $('<input type="radio"/>').attr('name', this.uniqueName)
                .attr('id', this.uniqueName + '_' + value)
                .attr('value', value).prependTo(label);
            label.appendTo(this.element);
        }

        get_value(): string {
            return this.element.find('input:checked').first().val();
        }

        get value(): string {
            return this.get_value();
        }

        set_value(value: string): void {
            if (value !== this.get_value()) {
                var inputs = this.element.find('input');
                var checks = inputs.filter(':checked');
                if (checks.length > 0) {
                    (checks[0] as HTMLInputElement).checked = false;
                }
                if (!Q.isEmptyOrNull(value)) {
                    checks = inputs.filter('[value=' + value + ']');
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
            return this.element.attr('disabled') != null;
        }

        set_readOnly(value: boolean): void {
            if (this.get_readOnly() !== value) {
                if (value) {
                    this.element.attr('disabled', 'disabled')
                        .find('input').attr('disabled', 'disabled');
                }
                else {
                    this.element.removeAttr('disabled')
                        .find('input').removeAttr('disabled');
                }
            }
        }

    }
}