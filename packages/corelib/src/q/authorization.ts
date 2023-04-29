import { localText } from "./localtext";
import { notifyError } from "./notify";
import { getRemoteData, getRemoteDataAsync } from "./scriptdata";
import { UserDefinition } from "./userdefinition";

const andOrRegex = /[|&]/;

/**
 * Contains permission related functions.
 * 
 * ## Note
 * We use a namespace here both for compatibility and for allowing users to override
 * these functions easily in ES modules environment, which is normally hard to do.
 */
export namespace Authorization {

    /** 
     * Checks if the current user has the permission specified.
     * This should only be used for UI purposes and it is strongly recommended to check permissions server side.
     * 
     * > Please prefer the `hasPermissionAsync` variant as this may block the UI thread if the `UserData` script is not already loaded.
     * @param permission Permission key. It may contain logical operators like A&B|C.
     * @returns `false` for "null or undefined", true for "*", `IsLoggedIn` for "?". For other permissions, 
     * if the user has the permission or if the user has the `IsAdmin` flag (super admin) `true`, otherwise `false`.
     */
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

    /** 
     * Checks if the current user has the permission specified.
     * This should only be used for UI purposes and it is strongly recommended to check permissions server side.
     * 
     * @param permission Permission key. It may contain logical operators like A&B|C.
     * @returns `false` for "null or undefined", true for "*", `IsLoggedIn` for "?". For other permissions, 
     * if the user has the permission or if the user has the `IsAdmin` flag (super admin) `true`, otherwise `false`.
     */
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

    /**
     * Throws an error if the current user does not have the specified permission. 
     * Prefer `await validatePermissionAsync()` as this one might block the UI if the `UserData`
     * is not already loaded.
     * @param permission Permission key. It may contain logical operators like A&B|C.
     */
    export function validatePermission(permission: string) {
        if (!hasPermission(permission)) {
            notifyError(localText("Authorization.AccessDenied"));
            throw new Error(localText("Authorization.AccessDenied"));
        }
    }

    /**
    * Throws an error if the current user does not have the specified permission.
    * @param permission Permission key. It may contain logical operators like A&B|C.
    * @example 
    * await Authorization.validatePermissionAsync("A&B|C");
    */
    export async function validatePermissionAsync(permission: string): Promise<void> {
        if (!(await hasPermissionAsync(permission))) {
            notifyError(localText("Authorization.AccessDenied"));
            throw new Error(localText("Authorization.AccessDenied"));
        }
    }
}

export declare namespace Authorization {
    /** 
     * Checks if the current user is logged in. Prefer `isLoggedInAsync` as this one might block the UI if the `UserData`
     * is not already loaded.
     * @returns `true` if the user is logged in, `false` otherwise. 
     * @example 
     * if (Authorization.isLoggedIn) {
     *     // do something
     * }
     */
    export let isLoggedIn: boolean;

    /** 
     * Checks if the current user is logged in.
     * @returns `true` if the user is logged in, `false` otherwise. 
     * @example 
     * if (await Authorization.isLoggedInAsync) {
     *     // do something
     * }
     */
    export let isLoggedInAsync: Promise<boolean>;

    /** Returns the username for currently logged user. Prefer `usernameAsync` as this one might block the UI if the `UserData`
     * is not already loaded.
     * @returns Username for currently logged user.
     * @example 
     * if (Authorization.username) {
     *     // do something
     * } 
     */
    export let username: string;

    /** Returns the username for currently logged user.
     * @returns Username for currently logged user.
     * @example 
     * if (await Authorization.usernameAsync) {
     *     // do something
     * }
     */
    export let usernameAsync: Promise<string>;

    /** Returns the user data for currently logged user. Prefer `userDefinitionAsync` as this one might block the UI if the `UserData`
     * is not already loaded.
     * @returns User data for currently logged user.
     * @example 
     * if (Authorization.userDefinition.IsAdmin) {
     *     // do something
     * }
     */
    export let userDefinition: UserDefinition;

    /** Returns the user data for currently logged user.
     * @returns User data for currently logged user.
     * @example 
     * if ((await Authorization.userDefinitionAsync).IsAdmin) {
     *     // do something
     * }
     */
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