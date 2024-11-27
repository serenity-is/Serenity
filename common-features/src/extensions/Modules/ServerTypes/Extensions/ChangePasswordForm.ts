import { PasswordEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface ChangePasswordForm {
    OldPassword: PasswordEditor;
    NewPassword: PasswordEditor;
    ConfirmPassword: PasswordEditor;
}

export class ChangePasswordForm extends PrefixedContext {
    static readonly formKey = 'Serenity.Extensions.ChangePasswordRequest';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!ChangePasswordForm.init)  {
            ChangePasswordForm.init = true;

            var w0 = PasswordEditor;

            initFormType(ChangePasswordForm, [
                'OldPassword', w0,
                'NewPassword', w0,
                'ConfirmPassword', w0
            ]);
        }
    }
}