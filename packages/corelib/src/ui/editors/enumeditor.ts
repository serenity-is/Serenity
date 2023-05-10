import { Decorators, EnumKeyAttribute } from "../../decorators";
import { Enum, getAttributes, tryGetText } from "@serenity-is/corelib/q";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";
import { Select2CommonOptions, Select2Editor } from "./select2editor";

export interface EnumEditorOptions extends Select2CommonOptions {
    enumKey?: string;
    enumType?: any;
}

@Decorators.registerEditor('Serenity.EnumEditor')
export class EnumEditor extends Select2Editor<EnumEditorOptions, Select2Item> {
    constructor(hidden: JQuery, opt: EnumEditorOptions) {
        super(hidden, opt);
        this.updateItems();
    }

    protected updateItems(): void {
        this.clearItems();

        var enumType = this.options.enumType || EnumTypeRegistry.get(this.options.enumKey);
        var enumKey = this.options.enumKey;

        if (enumKey == null && enumType != null) {
            var enumKeyAttr = getAttributes(enumType, EnumKeyAttribute, false);
            if (enumKeyAttr.length > 0) {
                enumKey = enumKeyAttr[0].value;
            }
        }

        var values = Enum.getValues(enumType);
        for (var x of values) {
            var name = Enum.toString(enumType, x);
            this.addOption(parseInt(x, 10).toString(),
                (tryGetText('Enums.' + enumKey + '.' + name) ?? name), null, false);
        }
    }

    protected allowClear() {
        return (this.options.allowClear ?? true);
    }
}