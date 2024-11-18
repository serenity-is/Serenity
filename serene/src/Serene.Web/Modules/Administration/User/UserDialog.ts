import { Decorators, EditorUtils, EntityDialog, stringFormat } from "@serenity-is/corelib";
import { UserForm, UserRow, UserService } from "../../ServerTypes/Administration";
import { MembershipValidationTexts, UserDialogTexts } from "../../ServerTypes/Texts";
import { UserPermissionDialog } from "../UserPermission/UserPermissionDialog";

@Decorators.registerClass()
export class UserDialog extends EntityDialog<UserRow, any> {
    protected getFormKey() { return UserForm.formKey; }
    protected getIdProperty() { return UserRow.idProperty; }
    protected getIsActiveProperty() { return UserRow.isActiveProperty; }
    protected getLocalTextPrefix() { return UserRow.localTextPrefix; }
    protected getNameProperty() { return UserRow.nameProperty; }
    protected getService() { return UserService.baseUrl; }

    protected form = new UserForm(this.idPrefix);

    constructor(props?: any) {
        super(props);

        this.form.Password.change(() => {
            EditorUtils.setRequired(this.form.PasswordConfirm, this.form.Password.value.length > 0);
        });

        this.form.Password.addValidationRule(this.uniqueName, e => {
            if (this.form.Password.value.length < 6)
                return stringFormat(MembershipValidationTexts.MinRequiredPasswordLength, 6);
        });

        this.form.PasswordConfirm.addValidationRule(this.uniqueName, e => {
            if (this.form.Password.value != this.form.PasswordConfirm.value)
                return MembershipValidationTexts.PasswordConfirmMismatch;
        });
    }

    protected getToolbarButtons()
    {
        let buttons = super.getToolbarButtons();

        buttons.push({
            title: UserDialogTexts.EditPermissionsButton,
            cssClass: 'edit-permissions-button',
            icon: 'fa-lock text-green',
            onClick: () =>
            {
                UserPermissionDialog({
                    userID: this.entity.UserId,
                    username: this.entity.Username
                });
            }
        });

        return buttons;
    }

    protected updateInterface() {
        super.updateInterface();

        this.toolbar.findButton("edit-permissions-button").toggleClass("disabled", this.isNewOrDeleted());
    }

    protected afterLoadEntity() {
        super.afterLoadEntity();

        // these fields are only required in new record mode
        this.form.Password.element.toggleClass('required', this.isNew())
            .closest('.field').findFirst('sup').toggle(this.isNew());
        this.form.PasswordConfirm.element.toggleClass('required', this.isNew())
            .closest('.field').findFirst('sup').toggle(this.isNew());
    }
}
