import { getRemoteData } from "@serenity-is/corelib";
import { ScriptUserDefinition } from "../..";

export function userDefinition() {
    return getRemoteData('UserData') as ScriptUserDefinition;
}

export function hasPermission(permissionKey: string): boolean {
    let ud = userDefinition();
    return ud.Username === 'admin' || !!ud.Permissions[permissionKey];
}