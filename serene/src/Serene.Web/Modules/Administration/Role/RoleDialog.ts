import { EntityDialog } from "@serenity-is/corelib";
import { RoleForm, RoleRow, RoleService } from "../../ServerTypes/Administration";
import { nsAdministration } from "../../ServerTypes/Namespaces";
import { RolePermissionDialogTexts } from "../../ServerTypes/Texts";
import { RolePermissionDialog } from "../RolePermission/RolePermissionDialog";

const editPermissions = "edit-permissions";

export class RoleDialog extends EntityDialog<RoleRow, any> {
    static override[Symbol.typeInfo] = this.registerClass(nsAdministration);

    protected override getFormKey() { return RoleForm.formKey; }
    protected override getIdProperty() { return RoleRow.idProperty; }
    protected override getLocalTextPrefix() { return RoleRow.localTextPrefix; }
    protected override getNameProperty() { return RoleRow.nameProperty; }
    protected override getService() { return RoleService.baseUrl; }

    protected form = new RoleForm(this.idPrefix);

    protected override getToolbarButtons() {
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

    protected override updateInterface() {
        super.updateInterface();

        this.toolbar.findButton(editPermissions).toggleClass("disabled", this.isNewOrDeleted());
    }
}