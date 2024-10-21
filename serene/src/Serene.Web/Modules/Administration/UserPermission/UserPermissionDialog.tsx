import { getRemoteData, localText, notifySuccess, stringFormat, BaseDialog } from "@serenity-is/corelib";
import { UserPermissionService } from "../../ServerTypes/Administration/UserPermissionService";
import { PermissionCheckEditor } from "./PermissionCheckEditor";
import { RemoteDataKeys } from "../../ServerTypes/RemoteDataKeys";

export class UserPermissionDialog extends BaseDialog<UserPermissionDialogOptions> {

    private permissions: PermissionCheckEditor;

    constructor(opt: UserPermissionDialogOptions) {
        super(opt);

        this.permissions = new PermissionCheckEditor({ element: this.byId('Permissions'), ... {
            showRevoke: true
        }});

        UserPermissionService.List({
            UserID: this.options.userID
        }, response => {
            this.permissions.value = response.Entities;
        });

        UserPermissionService.ListRolePermissions({
            UserID: this.options.userID
        }, response => {
            this.permissions.rolePermissions = response.Entities;
        });

        this.permissions.implicitPermissions = getRemoteData(RemoteDataKeys.Administration.ImplicitPermissions);
        this.dialogTitle = stringFormat(localText('Site.UserPermissionDialog.DialogTitle'),
            this.options.username);
    }

    protected getDialogButtons() {
        return [
            {
                text: localText('Dialogs.OkButton'),
                cssClass: 'btn btn-primary',
                click: e => {
                    UserPermissionService.Update({
                        UserID: this.options.userID,
                        Permissions: this.permissions.value
                    }, () => {
                        this.dialogClose();
                        window.setTimeout(() => notifySuccess(localText('Site.UserPermissionDialog.SaveSuccess')), 0);
                    });
                }
            }, {
                text: localText('Dialogs.CancelButton'),
                click: () => this.dialogClose()
            }
        ];
    }

    protected renderContents(): any {
        const id = this.useIdPrefix();
        return <div id={id.Permissions} />;
    }
}

export interface UserPermissionDialogOptions {
    userID?: number;
    username?: string;
}
