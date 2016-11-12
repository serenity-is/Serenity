namespace Q {
    export namespace Authorization {
        export function hasPermission(permission: string) {
            if (permission == null)
                return false;

            if (permission == "*")
                return true;

            if (permission == "" || permission == "?")
                return isLoggedIn;

            var ud = Authorization.userDefinition;
            if (ud && ud.Permissions) {
                var p = ud.Permissions;
                if (p[permission])
                    return true;

                var orParts = permission.split('|');
                for (var r of orParts) {
                    if (!r)
                        continue;

                    var andParts = r.split('&');
                    if (!andParts.length)
                        continue;

                    let fail = false;
                    for (var n of andParts) {
                        if (!p[n]) {
                            fail = true;
                            break;
                        }
                    }

                    if (!fail)
                        return true;
                }
            }

            return false;
        }

        export function validatePermission(permission: string) {
            if (!hasPermission(permission)) {
                Q.notifyError(Q.text("Authorization.AccessDenied"));
                throw new Error(Q.text("Authorization.AccessDenied"));
            }
        }
    }

    export declare namespace Authorization {
        export let isLoggedIn: boolean;
        export let username: string;
        export let userDefinition: Serenity.UserDefinition;
    }

    Object.defineProperty(Q.Authorization, "userDefinition", {
        get: function () {
            return Q.getRemoteData("UserData");
        }
    });

    Object.defineProperty(Q.Authorization, "isLoggedIn", {
        get: function () {
            var ud = Authorization.userDefinition;
            return ud && !!ud.Username;
        }
    });

    Object.defineProperty(Q.Authorization, "username", {
        get: function () {
            var ud = Authorization.userDefinition;
            if (ud)
                return ud.Username;

            return null;
        }
    });
}

namespace Serenity {
    export interface UserDefinition {
        Username?: string;
        DisplayName?: string;
        Permissions?: { [key: string]: boolean };
    }
}