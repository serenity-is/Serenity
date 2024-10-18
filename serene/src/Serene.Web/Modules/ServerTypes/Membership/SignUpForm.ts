import { StringEditor, EmailAddressEditor, PasswordEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface SignUpForm {
    DisplayName: StringEditor;
    Email: EmailAddressEditor;
    ConfirmEmail: EmailAddressEditor;
    Password: PasswordEditor;
    ConfirmPassword: PasswordEditor;
}

export class SignUpForm extends PrefixedContext {
    static readonly formKey = 'Membership.SignUp';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!SignUpForm.init)  {
            SignUpForm.init = true;

            var w0 = StringEditor;
            var w1 = EmailAddressEditor;
            var w2 = PasswordEditor;

            initFormType(SignUpForm, [
                'DisplayName', w0,
                'Email', w1,
                'ConfirmEmail', w1,
                'Password', w2,
                'ConfirmPassword', w2
            ]);
        }
    }
}