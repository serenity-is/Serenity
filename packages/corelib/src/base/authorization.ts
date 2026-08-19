import { localText } from "./localtext";
import { notifyError } from "./notify";
import { getRemoteData, getRemoteDataAsync } from "./scriptdata";
import { UserDefinition } from "./userdefinition";

const andOrRegex = /[|&]/;

/**
 * Provides permission checks and user-state accessors for the current session.
 *
 * Aggregates synchronous and asynchronous helpers that are intended for UI gating only;
 * server-side authorization must still be enforced. Permission expressions may combine
 * keys with `&` (AND) and `|` (OR), e.g. `"Admin&Sales|Manager"`.
 *
 * @remarks
 * Defined as a namespace (rather than plain functions) for backward compatibility and
 * to allow consumers to override/monkey-patch members in ES-module environments.
 * `*` always grants access; `""` and `"?"` only check that a user is logged in.
 * Users with `IsAdmin` are granted every permission.
 * @example
 * if (Authorization.hasPermission("Administration:General")) {
 *     // show admin UI
 * }
 * @example
 * if (await Authorization.hasPermissionAsync("Orders:View&Orders:Approve")) {
 *     // show approve button
 * }
 */
export namespace Authorization {

    /**
     * Synchronously checks whether the current user has the specified permission.
     *
     * @remarks
     * Prefer {@link Authorization.hasPermissionAsync} in new code — this synchronous
     * variant may block the UI thread if the `UserData` script has not been loaded yet
     * (it falls back to {@link getRemoteData} which can issue a synchronous request).
     * Use only for UI gating; always enforce permissions server-side as well.
     * @param permission - Permission key or expression. May contain `&` (AND) and `|` (OR)
     * operators, e.g. `"A&B|C"`. `null`/`undefined` returns `false`; `"*"` returns `true`;
     * `""` or `"?"` returns whether the user is logged in.
     * @returns `true` if the user has the permission (or is an admin), otherwise `false`.
     * @example
     * Authorization.hasPermission("Administration:General"); // true if admin or granted
     * @example
     * Authorization.hasPermission("A&B|C"); // true if (A and B) or C
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

        const ud = Authorization.userDefinition;
        if (!ud)
            return false;

        if (ud.IsAdmin)
            return true;

        return isPermissionInSet(ud.Permissions, permission);
    }

    /**
     * Asynchronously checks whether the current user has the specified permission.
     *
     * @remarks
     * Preferred over {@link Authorization.hasPermission} because it awaits
     * `UserData` via {@link getRemoteDataAsync} instead of potentially blocking
     * the UI thread. Still UI-only — enforce permissions server-side as well.
     * @param permission - Permission key or expression with `&`/`|` operators,
     * e.g. `"A&B|C"`. `null`/`undefined` returns `false`; `"*"` returns `true`;
     * `""` or `"?"` returns whether the user is logged in.
     * @returns Promise that resolves to `true` if the user has the permission
     * (or is an admin), otherwise `false`.
     * @example
     * if (await Authorization.hasPermissionAsync("Administration:General")) { ... }
     * @example
     * await Authorization.hasPermissionAsync("A&B|C"); // true if (A and B) or C
     */
    export async function hasPermissionAsync(permission: string): Promise<boolean> {
        if (permission == null)
            return false;

        if (permission == "*")
            return true;

        if (permission == "" || permission == "?")
            return await Authorization.isLoggedInAsync;

        const ud = await Authorization.userDefinitionAsync;
        if (!ud)
            return false;

        if (ud.IsAdmin)
            return true;

        return isPermissionInSet(ud.Permissions, permission);
    }

    /**
     * Tests whether a permission hash-set contains the specified permission or expression.
     *
     * @remarks
     * Handles logical operators: `"A&B"` requires all listed permissions, `"A|B"`
     * requires at least one. `&` binds tighter than `|`, so `"A&B|C"` is
     * `(A AND B) OR C`. An exact key match is checked first before parsing operators.
     * @param permissionSet - Dictionary of granted permissions (key → `true`). `null`/`undefined` yields `false`.
     * @param permission - Permission key or expression with `&`/`|` operators. `null` returns `false`.
     * @returns `true` if the permission is implied by the set, otherwise `false`.
     * @example
     * Authorization.isPermissionInSet({ "A": true, "B": true }, "A&B"); // true
     * @example
     * Authorization.isPermissionInSet({ "A": true }, "A|C"); // true
     * @example
     * Authorization.isPermissionInSet({ "A": true }, "A&B"); // false
     */
    export function isPermissionInSet(permissionSet: { [key: string]: boolean }, permission: string) {
        if (!permissionSet || permission == null)
            return false;

        if (permissionSet[permission])
            return true;

        if (!andOrRegex.test(permission))
            return false;

        const orParts = permission.split('|');
        for (const r of orParts) {
            if (!r.length)
                continue;

            const andParts = r.split('&');

            let anyFalse = false;
            for (const n of andParts) {
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
     * Synchronously validates that the current user has the specified permission.
     *
     * @remarks
     * Shows a localized "Access Denied" notification and throws if the check fails.
     * Prefer {@link Authorization.validatePermissionAsync} to avoid potentially
     * blocking on `UserData` loading. Use only for UI gating.
     * @param permission - Permission key or expression with `&`/`|` operators.
     * @throws Error with localized "Authorization.AccessDenied" message if the user lacks the permission.
     * @example
     * Authorization.validatePermission("Administration:General");
     */
    export function validatePermission(permission: string) {
        if (!hasPermission(permission)) {
            notifyError(localText("Authorization.AccessDenied"));
            throw new Error(localText("Authorization.AccessDenied"));
        }
    }

    /**
     * Asynchronously validates that the current user has the specified permission.
     *
     * @remarks
     * Awaits {@link Authorization.hasPermissionAsync} and, on failure, shows a
     * localized "Access Denied" notification before throwing.
     * @param permission - Permission key or expression with `&`/`|` operators.
     * @returns Promise that resolves if authorized, or rejects/throws if not.
     * @throws Error with localized "Authorization.AccessDenied" message if the user lacks the permission.
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
     * Whether the current user is logged in (synchronous).
     *
     * @remarks
     * Implemented as a getter over {@link Authorization.userDefinition}. Prefer
     * {@link Authorization.isLoggedInAsync} to avoid blocking on `UserData` load.
     * Returns `true` when `UserDefinition.Username` is truthy.
     * @example
     * if (Authorization.isLoggedIn) {
     *     // user is authenticated
     * }
     */
    export let isLoggedIn: boolean;

    /**
     * Whether the current user is logged in (asynchronous).
     *
     * @remarks
     * Awaits {@link Authorization.userDefinitionAsync} so it never blocks the UI thread.
     * @example
     * if (await Authorization.isLoggedInAsync) {
     *     // user is authenticated
     * }
     */
    export let isLoggedInAsync: Promise<boolean>;

    /**
     * Username of the currently logged-in user (synchronous).
     *
     * @remarks
     * Getter over {@link Authorization.userDefinition}`.Username`. Prefer
     * {@link Authorization.usernameAsync} if `UserData` may not be loaded yet.
     * Returns `undefined` when no user is logged in.
     * @example
     * const name = Authorization.username;
     */
    export let username: string;

    /**
     * Username of the currently logged-in user (asynchronous).
     *
     * @remarks
     * Awaits {@link Authorization.userDefinitionAsync}.
     * @example
     * const name = await Authorization.usernameAsync;
     */
    export let usernameAsync: Promise<string>;

    /**
     * User definition for the currently logged-in user (synchronous).
     *
     * @remarks
     * Retrieved via {@link getRemoteData}`("UserData")`. This may trigger a
     * synchronous load if not cached — prefer {@link Authorization.userDefinitionAsync}.
     * Returns `undefined`/`null` when not logged in.
     * @example
     * if (Authorization.userDefinition?.IsAdmin) {
     *     // super-admin branch
     * }
     */
    export let userDefinition: UserDefinition;

    /**
     * User definition for the currently logged-in user (asynchronous).
     *
     * @remarks
     * Retrieved via {@link getRemoteDataAsync}`("UserData")`.
     * @example
     * if ((await Authorization.userDefinitionAsync)?.IsAdmin) {
     *     // super-admin branch
     * }
     */
    export let userDefinitionAsync: Promise<UserDefinition>;
}

Object.defineProperty(Authorization, "isLoggedIn", {
    get: function () {
        return !!(Authorization.userDefinition?.Username);
    },
    configurable: true
});

Object.defineProperty(Authorization, "isLoggedInAsync", {
    get: async function () {
        return !!((await Authorization.userDefinitionAsync)?.Username);
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