import { StringEditor, PasswordEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface LoginForm {
    Username: StringEditor;
    Password: PasswordEditor;
}

export class LoginForm extends PrefixedContext {
    static readonly formKey = 'Membership.Login';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!LoginForm.init)  {
            LoginForm.init = true;

            var w0 = StringEditor;
            var w1 = PasswordEditor;

            initFormType(LoginForm, [
                'Username', w0,
                'Password', w1
            ]);
        }
    }
}