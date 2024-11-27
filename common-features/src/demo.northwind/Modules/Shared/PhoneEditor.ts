import { Decorators, StringEditor, WidgetProps, localText, replaceAll } from "@serenity-is/corelib";

export interface PhoneEditorOptions {
    multiple?: boolean;
}

@Decorators.registerEditor('Serenity.Demo.Northwind.PhoneEditor')
export class PhoneEditor<P extends PhoneEditorOptions = PhoneEditorOptions> extends StringEditor<P> {

    constructor(props: WidgetProps<P>) {
        super(props);

        this.addValidationRule(this.uniqueName, e => {
            var value = this.get_value()?.trim();
            if (!value)
                return null;
            return PhoneEditor.validate(value, this.props?.multiple);
        });

        let input = this.domNode as HTMLInputElement;
        input.addEventListener('change', e => {
            this.formatValue();
        });

        input.addEventListener('blur', e => {
            if (this.domNode.classList.contains('valid')) {
                this.formatValue();
            }
        });
    }

    protected formatValue(): void {
        (this.domNode as HTMLInputElement).value = this.getFormattedValue();
    }

    protected getFormattedValue(): string {
        var value = (this.domNode as HTMLInputElement).value;
        if (this.props?.multiple) {
            return PhoneEditor.formatMulti(value, PhoneEditor.formatPhone);
        }
        return PhoneEditor.formatPhone(value);
    }

    get_value() {
        return this.getFormattedValue();
    }

    set_value(value: string) {
        (this.domNode as HTMLInputElement).value = value;
    }

    static validate(phone: string, isMultiple: boolean) {
        var valid = (isMultiple ? PhoneEditor.isValidMulti(phone, PhoneEditor.isValidPhone) : PhoneEditor.isValidPhone(phone));
        if (valid) {
            return null;
        }
        return localText((isMultiple ? 'Validation.NorthwindPhoneMultiple' : 'Validation.NorthwindPhone'));
    }

    static isValidPhone(phone: string) {
        if (!phone) {
            return false;
        }
        phone = replaceAll(replaceAll(phone, ' ', ''), '-', '');
        if (phone.length < 10) {
            return false;
        }

        if (phone.startsWith('0')) {
            phone = phone.substring(1);
        }

        if (phone.startsWith('(') && phone.charAt(4) === ')') {
            phone = phone.substring(1, 4) + phone.substring(5);
        }

        if (phone.length !== 10) {
            return false;
        }

        if (phone.startsWith('0')) {
            return false;
        }

        for (var i = 0; i < phone.length; i++) {
            var c = phone.charAt(i);
            if (c < '0' || c > '9') {
                return false;
            }
        }

        return true;
    }

    static formatPhone(phone) {
        if (!PhoneEditor.isValidPhone(phone)) {
            return phone;
        }
        phone = replaceAll(replaceAll(replaceAll(replaceAll(phone, ' ', ''), '-', ''), '(', ''), ')', '');
        if (phone.startsWith('0')) {
            phone = phone.substring(1);
        }
        phone = '(' + phone.substr(0, 3) + ') ' + phone.substr(3, 3) + '-' + phone.substr(6, 2) + phone.substr(8, 2);
        return phone;
    }

    static formatMulti(phone: string, format: (s: string) => string) {
        var phones = replaceAll(phone, String.fromCharCode(59), String.fromCharCode(44)).split(String.fromCharCode(44));
        var result = '';
        for (var x of phones) {
            var s = x?.trim();
            if (!s)
                continue;
            if (result.length > 0)
                result += ', ';
            result += format(s);
        }
        return result;
    }

    static isValidMulti(phone: string, check: (s: string) => boolean) {
        if (!phone)
            return false;
        var phones = replaceAll(phone, String.fromCharCode(59), String.fromCharCode(44)).split(String.fromCharCode(44));
        var anyValid = false;
        for (var $t1 = 0; $t1 < phones.length; $t1++) {
            var x = phones[$t1];
            var s = x?.trim();
            if (!s)
                continue;
            if (!check(s))
                return false;
            anyValid = true;
        }
        if (!anyValid)
            return false;
        return true;
    }
}
