import { initFormType, LookupEditor, PrefixedContext, StringEditor } from "@serenity-is/corelib";

export interface TerritoryForm {
    TerritoryID: StringEditor;
    TerritoryDescription: StringEditor;
    RegionID: LookupEditor;
}

export class TerritoryForm extends PrefixedContext {
    static readonly formKey = 'Northwind.Territory';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!TerritoryForm.init) {
            TerritoryForm.init = true;

            initFormType(TerritoryForm, [
                'TerritoryID', StringEditor,
                'TerritoryDescription', StringEditor,
                'RegionID', LookupEditor
            ]);
        }
    }
}