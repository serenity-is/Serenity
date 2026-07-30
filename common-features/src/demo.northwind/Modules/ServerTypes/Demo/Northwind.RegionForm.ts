import { initFormType, IntegerEditor, PrefixedContext, StringEditor } from "@serenity-is/corelib";

export interface RegionForm {
    RegionID: IntegerEditor;
    RegionDescription: StringEditor;
}

export class RegionForm extends PrefixedContext {
    static readonly formKey = 'Northwind.Region';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!RegionForm.init) {
            RegionForm.init = true;

            initFormType(RegionForm, [
                'RegionID', IntegerEditor,
                'RegionDescription', StringEditor
            ]);
        }
    }
}