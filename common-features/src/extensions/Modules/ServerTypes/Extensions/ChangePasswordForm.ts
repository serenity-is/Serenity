import { initFormType, PasswordEditor, PrefixedContext } from "@serenity-is/corelib";

export interface ChangePasswordForm {
    OldPassword: PasswordEditor;
    NewPassword: PasswordEditor;
    ConfirmPassword: PasswordEditor;
}

export class ChangePasswordForm extends PrefixedContext {
    static readonly formKey = 'Serenity.Extensions.ChangePasswordRequest';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!ChangePasswordForm.init) {
            ChangePasswordForm.init = true;

            initFormType(ChangePasswordForm, [
                'OldPassword', PasswordEditor,
                'NewPassword', PasswordEditor,
                'ConfirmPassword', PasswordEditor
            ]);
        }
    }
}