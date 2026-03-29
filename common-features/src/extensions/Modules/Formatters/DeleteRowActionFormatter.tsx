import { DataGridTexts, EntityDialogTexts, faIcon, Formatter, formatterTypeInfo, registerType, type RemoteView } from "@serenity-is/corelib";
import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { nsExtensions } from "../ServerTypes/Namespaces";

export class DeleteRowActionFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsExtensions); static { registerType(this); }

    format(ctx: FormatterContext): FormatterResult {
        if (!ctx.item || 
            ctx.item.__nonDataRow ||
            ctx.item[(ctx.grid?.getData?.() as RemoteView)?.getIdPropertyName()] == null) 
            return "";

        return <a class="inline-action" data-action="delete-row" title={EntityDialogTexts.DeleteButton}><i class={faIcon("trash", "danger")}></i></a>;
    }
}