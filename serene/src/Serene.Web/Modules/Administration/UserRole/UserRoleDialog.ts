import { Decorators, TemplatedDialog } from "@serenity-is/corelib";
import { DialogButton, format, notifySuccess, serviceRequest, localText } from "@serenity-is/corelib";
import { UserRoleService } from "../";
import { RoleCheckEditor } from "./RoleCheckEditor";

@Decorators.registerClass()
export class UserRoleDialog extends TemplatedDialog<UserRoleDialogOptions> {

    private permissions: RoleCheckEditor;

    constructor(opt: UserRoleDialogOptions) {
        super(opt);

        this.permissions = new RoleCheckEditor(this.byId('Roles'));

        UserRoleService.List({
            UserID: this.options.userID
        }, response => {
            this.permissions.value = response.Entities.map(x => x.toString());
        });

        this.dialogTitle = format(localText('Site.UserRoleDialog.DialogTitle'), this.options.username);
    }

    protected getDialogButtons(): DialogButton[] {
        return [{
            text: localText('Dialogs.OkButton'),
            cssClass: 'btn btn-primary',
            click: () => {
                serviceRequest('Administration/UserRole/Update', {
                    UserID: this.options.userID,
                    Roles: this.permissions.value.map(x => parseInt(x, 10))
                }, _ => {
                    this.dialogClose();
                    notifySuccess(localText('Site.UserRoleDialog.SaveSuccess'));
                });
            }
        }, {
            text: localText('Dialogs.CancelButton'),
            click: () => this.dialogClose()
        }];
    }

    protected getTemplate() {
        return "<div id='~_Roles'></div>";
    }
}

export interface UserRoleDialogOptions {
    userID: number;
    username: string;
}
