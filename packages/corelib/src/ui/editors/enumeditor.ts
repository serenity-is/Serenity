import { Enum, EnumKeyAttribute, getCustomAttribute, getTypeFullName, isPromiseLike, localText, nsSerenity } from "../../base";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";
import { ComboboxItem } from "./combobox";
import { ComboboxCommonOptions, ComboboxEditor } from "./comboboxeditor";
import { EditorProps } from "./editorwidget";

export interface EnumEditorOptions extends ComboboxCommonOptions {
    enumKey?: string;
    enumType?: any;
}

export class EnumEditor<P extends EnumEditorOptions = EnumEditorOptions> extends ComboboxEditor<P, ComboboxItem> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    constructor(props: EditorProps<P>) {
        super(props);

        this.updateItems();
    }

    protected override updateItems(): void | PromiseLike<void> {
        this.clearItems();

        var enumType = this.options.enumType || EnumTypeRegistry.getOrLoad(this.options.enumKey);

        const then = (enumType: any) => {
            var enumKey = this.options.enumKey;

            if (enumKey == null && enumType != null) {
                enumKey = getCustomAttribute(enumType, EnumKeyAttribute, false)?.value ?? 
                    getTypeFullName(enumType);
            }

            var values = Enum.getValues(enumType);
            for (var x of values) {
                var name = Enum.toString(enumType, x);
                this.addOption(parseInt(x as any, 10).toString(),
                    localText("Enums." + enumKey + "." + name, name), null, false);
            }
        }

        if (isPromiseLike(enumType))
            return enumType.then(then);
        else
            then(enumType);
    }

    protected override allowClear() {
        return (this.options.allowClear ?? true);
    }
}