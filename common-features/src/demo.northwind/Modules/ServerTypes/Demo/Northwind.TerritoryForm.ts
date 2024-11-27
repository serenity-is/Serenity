import { StringEditor, LookupEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface TerritoryForm {
    TerritoryID: StringEditor;
    TerritoryDescription: StringEditor;
    RegionID: LookupEditor;
}

export class TerritoryForm extends PrefixedContext {
    static readonly formKey = 'Northwind.Territory';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!TerritoryForm.init)  {
            TerritoryForm.init = true;

            var w0 = StringEditor;
            var w1 = LookupEditor;

            initFormType(TerritoryForm, [
                'TerritoryID', w0,
                'TerritoryDescription', w0,
                'RegionID', w1
            ]);
        }
    }
}