import { Decorators, EntityDialog } from "@serenity-is/corelib";
import { RolePermissionDialog } from "../RolePermission/RolePermissionDialog";
import { RoleRow, RoleForm, RoleService } from "../../ServerTypes/Administration";
import { RolePermissionDialogTexts } from "../../ServerTypes/Texts";

const editPermissions = "edit-permissions";

@Decorators.registerClass('Serene.Administration.RoleDialog')
export class RoleDialog extends EntityDialog<RoleRow, any> {
    protected getFormKey() { return RoleForm.formKey; }
    protected getIdProperty() { return RoleRow.idProperty; }
    protected getLocalTextPrefix() { return RoleRow.localTextPrefix; }
    protected getNameProperty() { return RoleRow.nameProperty; }
    protected getService() { return RoleService.baseUrl; }

    protected form = new RoleForm(this.idPrefix);

    protected getToolbarButtons() {
        let buttons = super.getToolbarButtons();

        buttons.push({
            title: RolePermissionDialogTexts.EditButton,
            cssClass: editPermissions,
            icon: 'fa-lock text-green',
            onClick: () => {
                new RolePermissionDialog({
                    roleID: this.entity.RoleId,
                    title: this.entity.RoleName
                }).dialogOpen();
            }
        });

        return buttons;
    }

    protected updateInterface() {
        super.updateInterface();

        this.toolbar.findButton(editPermissions).toggleClass("disabled", this.isNewOrDeleted());
    }
}