import { EmailAddressEditor, initFormType, PrefixedContext } from "@serenity-is/corelib";

export interface ForgotPasswordForm {
    Email: EmailAddressEditor;
}

export class ForgotPasswordForm extends PrefixedContext {
    static readonly formKey = 'Serenity.Extensions.ForgotPasswordRequest';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!ForgotPasswordForm.init) {
            ForgotPasswordForm.init = true;

            initFormType(ForgotPasswordForm, [
                'Email', EmailAddressEditor
            ]);
        }
    }
}