import { initFormType, PrefixedContext, StringEditor } from "@serenity-is/corelib";

export interface RoleForm {
    RoleName: StringEditor;
}

export class RoleForm extends PrefixedContext {
    static readonly formKey = 'Administration.Role';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!RoleForm.init) {
            RoleForm.init = true;

            initFormType(RoleForm, [
                'RoleName', StringEditor
            ]);
        }
    }
}