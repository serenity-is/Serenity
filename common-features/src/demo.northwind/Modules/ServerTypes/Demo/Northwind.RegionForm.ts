import { IntegerEditor, StringEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface RegionForm {
    RegionID: IntegerEditor;
    RegionDescription: StringEditor;
}

export class RegionForm extends PrefixedContext {
    static readonly formKey = 'Northwind.Region';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!RegionForm.init)  {
            RegionForm.init = true;

            var w0 = IntegerEditor;
            var w1 = StringEditor;

            initFormType(RegionForm, [
                'RegionID', w0,
                'RegionDescription', w1
            ]);
        }
    }
}