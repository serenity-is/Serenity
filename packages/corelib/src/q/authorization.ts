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
            return await Authorization.isLoggedInAsync;

        var ud = await Authorization.userDefinitionAsync;
        if (!ud)
            return false;

        if (ud.IsAdmin)
            return true;

        return isPermissionInSet(ud.Permissions, permission);
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
    export let isLoggedInAsync: Promise<boolean>;
    export let username: string;
    export let usernameAsync: Promise<string>;
    export let userDefinition: UserDefinition;
    export let userDefinitionAsync: Promise<UserDefinition>;
}

Object.defineProperty(Authorization, "isLoggedIn", {
    get: function () {
        return !!(Authorization.userDefinition?.Username?.length);
    },
    configurable: true
});

Object.defineProperty(Authorization, "isLoggedInAsync", {
    get: async function () {
        return !!((await Authorization.userDefinitionAsync)?.Username?.length);
    },
    configurable: true
});

Object.defineProperty(Authorization, "userDefinition", {
    get: function () {
        return getRemoteData<UserDefinition>("UserData");
    },
    configurable: true
});

Object.defineProperty(Authorization, "userDefinitionAsync", {
    get: async function () {
        return await getRemoteDataAsync<UserDefinition>("UserData");
    },
    configurable: true
});

Object.defineProperty(Authorization, "username", {
    get: function () {
        return Authorization.userDefinition?.Username;
    },
    configurable: true
});

Object.defineProperty(Authorization, "usernameAsync", {
    get: async function () {
        return (await Authorization.userDefinitionAsync)?.Username;
    },
    configurable: true
});