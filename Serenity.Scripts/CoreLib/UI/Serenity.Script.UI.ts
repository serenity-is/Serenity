namespace Serenity {

    @Serenity.Decorators.registerEditor('Serenity.AsyncLookupEditor',
        [Serenity.ISetEditValue, Serenity.IGetEditValue, Serenity.IStringValue, Serenity.IReadOnly, Serenity.IAsyncInit])
    export class AsyncLookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(hidden: JQuery, opt: LookupEditorOptions) {
            super(hidden, opt);
        }

        getLookupKey() {
            return Q.coalesce(this.options.lookupKey, super.getLookupKey());
        }
    }
}

