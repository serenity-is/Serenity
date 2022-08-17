import { Decorators } from "../../decorators";
import { IGetEditValue, IReadOnly, ISetEditValue, IStringValue } from "../../interfaces";
import { LookupEditorBase, LookupEditorOptions } from "./lookupeditor";

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

