import { registerEditor } from "../../Decorators";
import { IGetEditValue } from "../../Interfaces/IGetEditValue";
import { IReadOnly } from "../../Interfaces/IReadOnly";
import { ISetEditValue } from "../../Interfaces/ISetEditValue";
import { IStringValue } from "../../Interfaces/IStringValue";
import { LookupEditorBase, LookupEditorOptions } from "./LookupEditor";

// legacy, don't use!
@registerEditor('Serenity.AsyncLookupEditor',
    [ISetEditValue, IGetEditValue, IStringValue, IReadOnly])
export class AsyncLookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
    constructor(hidden: JQuery, opt: LookupEditorOptions) {
        super(hidden, opt);
    }

    getLookupKey() {
        return (this.options.lookupKey ?? super.getLookupKey());
    }
}

