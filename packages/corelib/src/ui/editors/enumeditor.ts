import { Enum, getCustomAttribute, tryGetText } from "../../base";
import { EnumKeyAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";
import { EditorProps } from "../widgets/widget";
import { ComboboxItem } from "./combobox";
import { ComboboxCommonOptions, ComboboxEditor } from "./comboboxeditor";

export interface EnumEditorOptions extends ComboboxCommonOptions {
    enumKey?: string;
    enumType?: any;
}

@Decorators.registerEditor('Serenity.EnumEditor')
export class EnumEditor<P extends EnumEditorOptions = EnumEditorOptions> extends ComboboxEditor<P, ComboboxItem> {
    constructor(props: EditorProps<P>) {
        super(props);

        this.updateItems();
    }

    protected updateItems(): void {
        this.clearItems();

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
            this.addOption(parseInt(x, 10).toString(),
                (tryGetText('Enums.' + enumKey + '.' + name) ?? name), null, false);
        }
    }

    protected allowClear() {
        return (this.options.allowClear ?? true);
    }
}