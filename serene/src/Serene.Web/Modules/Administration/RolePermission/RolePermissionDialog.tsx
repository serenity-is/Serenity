import { Dialog, cancelDialogButton, getRemoteDataAsync, notifySuccess, okDialogButton, stringFormat } from "@serenity-is/corelib";
import { RolePermissionService } from "../../ServerTypes/Administration";
import { RemoteDataKeys } from "../../ServerTypes/RemoteDataKeys";
import { RolePermissionDialogTexts } from "../../ServerTypes/Texts";
import { PermissionCheckEditor } from "../UserPermission/PermissionCheckEditor";

export async function RolePermissionDialog(props: {
    roleID?: number,
    roleName?: string,
}) {
    let checkEditor: PermissionCheckEditor;
    let permissions = (await RolePermissionService.List({ RoleID: props.roleID, })).Entities;
    let implicitPermissions = await getRemoteDataAsync<Record<string, string[]>>(RemoteDataKeys.Administration.ImplicitPermissions);

    new Dialog({
        dialogClass: "s-RolePermissionDialog",
        title: stringFormat(RolePermissionDialogTexts.DialogTitle, props.roleName),
        buttons: [
            okDialogButton({
                click: async () => {
                    await RolePermissionService.Update({
                        RoleID: props.roleID,
                        Permissions: checkEditor.valueAsStrings
                    });
                    notifySuccess(RolePermissionDialogTexts.SaveSuccess);
                }
            }),
            cancelDialogButton()
        ],
        element: el => el.appendChild(<PermissionCheckEditor ref={w => checkEditor = w}
            showRevoke={false} value={permissions} implicitPermissions={implicitPermissions} />)
    });
}