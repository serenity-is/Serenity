import { EntityDialog } from "@serenity-is/corelib";
import { RoleForm, RoleRow, RoleService } from "../../ServerTypes/Administration";
import { nsAdministration } from "../../ServerTypes/Namespaces";
import { RolePermissionDialogTexts } from "../../ServerTypes/Texts";
import { RolePermissionDialog } from "../RolePermission/RolePermissionDialog";

const editPermissions = "edit-permissions";

export class RoleDialog extends EntityDialog<RoleRow, any> {
    static [Symbol.typeInfo] = this.registerClass(nsAdministration);

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
                RolePermissionDialog({
                    roleID: this.entity.RoleId,
                    roleName: this.entity.RoleName
                });
            }
        });

        return buttons;
    }

    protected updateInterface() {
        super.updateInterface();

        this.toolbar.findButton(editPermissions).toggleClass("disabled", this.isNewOrDeleted());
    }
}