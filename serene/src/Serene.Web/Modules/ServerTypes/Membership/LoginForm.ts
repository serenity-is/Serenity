import { initFormType, PasswordEditor, PrefixedContext, StringEditor } from "@serenity-is/corelib";

export interface LoginForm {
    Username: StringEditor;
    Password: PasswordEditor;
}

export class LoginForm extends PrefixedContext {
    static readonly formKey = 'Membership.Login';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!LoginForm.init) {
            LoginForm.init = true;

            initFormType(LoginForm, [
                'Username', StringEditor,
                'Password', PasswordEditor
            ]);
        }
    }
}