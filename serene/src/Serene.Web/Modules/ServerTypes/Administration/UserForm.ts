import { StringEditor, EmailAddressEditor, LookupEditor, ImageUploadEditor, PasswordEditor, BooleanEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

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
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!UserForm.init)  {
            UserForm.init = true;

            var w0 = StringEditor;
            var w1 = EmailAddressEditor;
            var w2 = LookupEditor;
            var w3 = ImageUploadEditor;
            var w4 = PasswordEditor;
            var w5 = BooleanEditor;

            initFormType(UserForm, [
                'Username', w0,
                'DisplayName', w0,
                'Email', w1,
                'Roles', w2,
                'UserImage', w3,
                'Password', w4,
                'PasswordConfirm', w4,
                'Source', w0,
                'IsActive', w5
            ]);
        }
    }
}