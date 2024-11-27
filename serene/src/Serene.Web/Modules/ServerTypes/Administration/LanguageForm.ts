import { StringEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface LanguageForm {
    LanguageId: StringEditor;
    LanguageName: StringEditor;
}

export class LanguageForm extends PrefixedContext {
    static readonly formKey = 'Administration.Language';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!LanguageForm.init)  {
            LanguageForm.init = true;

            var w0 = StringEditor;

            initFormType(LanguageForm, [
                'LanguageId', w0,
                'LanguageName', w0
            ]);
        }
    }
}