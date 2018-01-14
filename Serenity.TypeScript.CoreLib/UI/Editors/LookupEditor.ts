namespace Serenity {

    @Decorators.registerEditor('Serenity.LookupEditor')
    export class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(hidden: JQuery, opt?: LookupEditorOptions) {
            super(hidden, opt);
        }
    }
}