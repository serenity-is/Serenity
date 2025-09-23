import { Enum, getCustomAttribute, isPromiseLike, localText, nsSerenity } from "../../base";
import { EnumKeyAttribute } from "../../types/attributes";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";
import { ComboboxItem } from "./combobox";
import { ComboboxCommonOptions, ComboboxEditor } from "./comboboxeditor";
import { EditorProps } from "./editorwidget";

export interface EnumEditorOptions extends ComboboxCommonOptions {
    enumKey?: string;
    enumType?: any;
}

export class EnumEditor<P extends EnumEditorOptions = EnumEditorOptions> extends ComboboxEditor<P, ComboboxItem> {
    static [Symbol.typeInfo] = this.registerEditor(nsSerenity);

    constructor(props: EditorProps<P>) {
        super(props);

        this.updateItems();
    }

    protected updateItems(): void | PromiseLike<void> {
        this.clearItems();

        var enumType = this.options.enumType || EnumTypeRegistry.getOrLoad(this.options.enumKey);

        const then = (enumType: any) => {
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
                    localText("Enums." + enumKey + "." + name, name), null, false);
            }
        }

        if (isPromiseLike(enumType))
            return enumType.then(then);
        else
            then(enumType);
    }

    protected allowClear() {
        return (this.options.allowClear ?? true);
    }
}