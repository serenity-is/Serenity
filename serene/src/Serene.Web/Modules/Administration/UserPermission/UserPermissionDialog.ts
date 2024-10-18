import { Decorators, TemplatedDialog } from "@serenity-is/corelib";
import { format, getRemoteData, notifySuccess, localText } from "@serenity-is/corelib";
import { UserPermissionService } from "../";
import { PermissionCheckEditor } from "./PermissionCheckEditor";

export class UserPermissionDialog extends TemplatedDialog<UserPermissionDialogOptions> {

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

        this.permissions.implicitPermissions = getRemoteData('Administration.ImplicitPermissions');
        this.dialogTitle = format(localText('Site.UserPermissionDialog.DialogTitle'),
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
                    }, response => {
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

    protected getTemplate(): string {
        return '<div id="~_Permissions"></div>';
    }
}

export interface UserPermissionDialogOptions {
    userID?: number;
    username?: string;
}
