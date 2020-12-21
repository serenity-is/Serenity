import { Decorators } from "../../Decorators";
import { IGetEditValue, IReadOnly, ISetEditValue, IStringValue } from "../../Interfaces";
import { LookupEditorBase, LookupEditorOptions } from "./LookupEditor";

// legacy, don't use!
@Decorators.registerEditor('Serenity.AsyncLookupEditor',
    [ISetEditValue, IGetEditValue, IStringValue, IReadOnly])
export class AsyncLookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
    constructor(hidden: JQuery, opt: LookupEditorOptions) {
        super(hidden, opt);
    }

    getLookupKey() {
        return (this.options.lookupKey ?? super.getLookupKey());
    }
}

