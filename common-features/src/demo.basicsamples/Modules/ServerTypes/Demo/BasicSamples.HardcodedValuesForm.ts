import { initFormType, PrefixedContext } from "@serenity-is/corelib";
import { HardcodedValuesEditor } from "../../Editors/SelectWithHardcodedValues/SelectWithHardcodedValuesPage";

export interface HardcodedValuesForm {
    SomeValue: HardcodedValuesEditor;
}

export class HardcodedValuesForm extends PrefixedContext {
    static readonly formKey = 'BasicSamples.HarcodedValues';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!HardcodedValuesForm.init) {
            HardcodedValuesForm.init = true;

            initFormType(HardcodedValuesForm, [
                'SomeValue', HardcodedValuesEditor
            ]);
        }
    }
}