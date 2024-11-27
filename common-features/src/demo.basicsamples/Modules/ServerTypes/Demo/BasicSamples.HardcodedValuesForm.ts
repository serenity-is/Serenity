import { PrefixedContext, initFormType } from "@serenity-is/corelib";
import { HardcodedValuesEditor } from "../../Editors/SelectWithHardcodedValues/SelectWithHardcodedValuesPage";

export interface HardcodedValuesForm {
    SomeValue: HardcodedValuesEditor;
}

export class HardcodedValuesForm extends PrefixedContext {
    static readonly formKey = 'BasicSamples.HarcodedValues';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!HardcodedValuesForm.init)  {
            HardcodedValuesForm.init = true;

            var w0 = HardcodedValuesEditor;

            initFormType(HardcodedValuesForm, [
                'SomeValue', w0
            ]);
        }
    }
}