import { initFormType, PasswordEditor, PrefixedContext } from "@serenity-is/corelib";

export interface ResetPasswordForm {
    NewPassword: PasswordEditor;
    ConfirmPassword: PasswordEditor;
}

export class ResetPasswordForm extends PrefixedContext {
    static readonly formKey = 'Serenity.Extensions.ResetPasswordRequest';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!ResetPasswordForm.init) {
            ResetPasswordForm.init = true;

            initFormType(ResetPasswordForm, [
                'NewPassword', PasswordEditor,
                'ConfirmPassword', PasswordEditor
            ]);
        }
    }
}