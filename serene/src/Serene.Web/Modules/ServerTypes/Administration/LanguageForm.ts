import { initFormType, PrefixedContext, StringEditor } from "@serenity-is/corelib";

export interface LanguageForm {
    LanguageId: StringEditor;
    LanguageName: StringEditor;
}

export class LanguageForm extends PrefixedContext {
    static readonly formKey = 'Administration.Language';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!LanguageForm.init) {
            LanguageForm.init = true;

            initFormType(LanguageForm, [
                'LanguageId', StringEditor,
                'LanguageName', StringEditor
            ]);
        }
    }
}