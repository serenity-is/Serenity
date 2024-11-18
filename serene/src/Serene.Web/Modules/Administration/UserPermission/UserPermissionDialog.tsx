import { Dialog, cancelDialogButton, getRemoteDataAsync, notifySuccess, okDialogButton, stringFormat } from "@serenity-is/corelib";
import { UserPermissionService } from "../../ServerTypes/Administration";
import { RemoteDataKeys } from "../../ServerTypes/RemoteDataKeys";
import { UserPermissionDialogTexts } from "../../ServerTypes/Texts";
import { PermissionCheckEditor } from "../UserPermission/PermissionCheckEditor";

export async function UserPermissionDialog(props: { userID: number, username: string }) {

    const permissions = (await UserPermissionService.List({ UserID: props.userID })).Entities;
    const rolePermissions = (await UserPermissionService.ListRolePermissions({ UserID: props.userID })).Entities;

    let implicitPermissions = await getRemoteDataAsync<Record<string, string[]>>(RemoteDataKeys.Administration.ImplicitPermissions);

    var checkEditor: PermissionCheckEditor;
    new Dialog({
        dialogClass: "s-UserPermissionDialog",
        title: stringFormat(UserPermissionDialogTexts.DialogTitle, props.username),
        buttons: [
            okDialogButton({
                click: async () => {
                    await UserPermissionService.Update({
                        UserID: props.userID,
                        Permissions: checkEditor.value
                    });
                    notifySuccess(UserPermissionDialogTexts.SaveSuccess);
                }
            }),
            cancelDialogButton()
        ],
        element: el => el.appendChild(<PermissionCheckEditor ref={w => checkEditor = w}
            showRevoke={true} value={permissions} implicitPermissions={implicitPermissions} rolePermissions={rolePermissions} />)
    });
}

