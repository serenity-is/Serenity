import { BooleanEditor, EmailAddressEditor, ImageUploadEditor, initFormType, LookupEditor, PasswordEditor, PrefixedContext, StringEditor } from "@serenity-is/corelib";

export interface UserForm {
    Username: StringEditor;
    DisplayName: StringEditor;
    Email: EmailAddressEditor;
    Roles: LookupEditor;
    UserImage: ImageUploadEditor;
    Password: PasswordEditor;
    PasswordConfirm: PasswordEditor;
    Source: StringEditor;
    IsActive: BooleanEditor;
}

export class UserForm extends PrefixedContext {
    static readonly formKey = 'Administration.User';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!UserForm.init) {
            UserForm.init = true;

            initFormType(UserForm, [
                'Username', StringEditor,
                'DisplayName', StringEditor,
                'Email', EmailAddressEditor,
                'Roles', LookupEditor,
                'UserImage', ImageUploadEditor,
                'Password', PasswordEditor,
                'PasswordConfirm', PasswordEditor,
                'Source', StringEditor,
                'IsActive', BooleanEditor
            ]);
        }
    }
}