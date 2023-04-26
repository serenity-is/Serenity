import { localText } from "./localtext";
import { notifyError } from "./notify";
import { getRemoteData, getRemoteDataAsync } from "./scriptdata";
import { UserDefinition } from "./userdefinition";

const andOrRegex = /[|&]/;

/**
 * Contains permission related functions.
 * Note: We use a namespace here both for compatibility and allow users to override
 * these functions easily in ES modules environment, which is normally hard to do.
 */
export namespace Authorization {

    export function hasPermission(permission: string) {
        if (permission == null)
            return false;

        if (permission == "*")
            return true;

        // normally in server side empty permission would return false
        // here we are more tolerant for compatibility reasons and
        // as it is less risky
        if (permission == "" || permission == "?")
            return !!Authorization.isLoggedIn;

        var ud = Authorization.userDefinition;
        if (!ud)
            return false;

        if (ud.IsAdmin)
            return true;

        return isPermissionInSet(ud.Permissions, permission);
    }

    export async function hasPermissionAsync(permission: string): Promise<boolean> {
        if (permission == null)
            return false;

        if (permission == "*")
            return true;

        if (permission == "" || permission == "?")
            return await Authorization.isLoggedInAsync();

        var ud = await Authorization.userDefinitionAsync();
        if (!ud)
            return false;

        if (ud.IsAdmin)
            return true;

        return isPermissionInSet(ud.Permissions, permission);
    }

    export async function isLoggedInAsync(): Promise<boolean> {
        var ud = await Authorization.userDefinitionAsync();
        return ud?.Username?.length > 0;
    }

    export async function userDefinitionAsync(): Promise<UserDefinition> {
        return await getRemoteDataAsync("UserData") as UserDefinition;
    }

    export async function usernameAsync(): Promise<string> {
        var ud = await Authorization.userDefinitionAsync();
        return ud?.Username;
    }

    /**
     * Checks if the hashset contains the specified permission, also handling logical "|" and "&" operators
     * @param permissionSet Set of permissions
     * @param permission Permission key or a permission expression containing & | operators
     * @returns true if set contains permission
     */
    export function isPermissionInSet(permissionSet: { [key: string]: boolean }, permission: string) {
        if (!permissionSet || permission == null)
            return false;

        if (permissionSet[permission])
            return true;

        if (!andOrRegex.test(permission))
            return false;

        var orParts = permission.split('|');
        for (var r of orParts) {
            if (!r.length)
                continue;

            var andParts = r.split('&');

            let anyFalse = false;
            for (var n of andParts) {
                if (!n || !permissionSet[n]) {
                    anyFalse = true;
                    break;
                }
            }

            if (!anyFalse)
                return true;
        }

        return false;
    }

    export function validatePermission(permission: string) {
        if (!hasPermission(permission)) {
            notifyError(localText("Authorization.AccessDenied"));
            throw new Error(localText("Authorization.AccessDenied"));
        }
    }

    export async function validatePermissionAsync(permission: string): Promise<void> {
        if (!(await hasPermissionAsync(permission))) {
            notifyError(localText("Authorization.AccessDenied"));
            throw new Error(localText("Authorization.AccessDenied"));
        }
    }
}

export declare namespace Authorization {
    export let isLoggedIn: boolean;
    export let username: string;
    export let userDefinition: UserDefinition;
}

Object.defineProperty(Authorization, "userDefinition", {
    get: function () {
        return getRemoteData("UserData");
    },
    configurable: true
});

Object.defineProperty(Authorization, "isLoggedIn", {
    get: function () {
        var ud = Authorization.userDefinition;
        return ud && !!ud.Username;
    },
    configurable: true
});

Object.defineProperty(Authorization, "username", {
    get: function () {
        var ud = Authorization.userDefinition;
        if (ud)
            return ud.Username;

        return null;
    },
    configurable: true
});