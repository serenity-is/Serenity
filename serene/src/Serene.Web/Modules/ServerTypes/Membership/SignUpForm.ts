import { EmailAddressEditor, initFormType, PasswordEditor, PrefixedContext, StringEditor } from "@serenity-is/corelib";

export interface SignUpForm {
    DisplayName: StringEditor;
    Email: EmailAddressEditor;
    ConfirmEmail: EmailAddressEditor;
    Password: PasswordEditor;
    ConfirmPassword: PasswordEditor;
}

export class SignUpForm extends PrefixedContext {
    static readonly formKey = 'Membership.SignUp';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!SignUpForm.init) {
            SignUpForm.init = true;

            initFormType(SignUpForm, [
                'DisplayName', StringEditor,
                'Email', EmailAddressEditor,
                'ConfirmEmail', EmailAddressEditor,
                'Password', PasswordEditor,
                'ConfirmPassword', PasswordEditor
            ]);
        }
    }
}