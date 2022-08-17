import { text } from "./localtext";
import { notifyError } from "./notify";
import { getRemoteData } from "./scriptdata";
import { UserDefinition } from "./userdefinition";

export namespace Authorization {
    export function hasPermission(permission: string) {
        if (permission == null)
            return false;

        if (permission == "*")
            return true;

        if (permission == "" || permission == "?")
            return isLoggedIn;

        var ud = Authorization.userDefinition;
        if (ud && ud.IsAdmin)
            return true;

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
            notifyError(text("Authorization.AccessDenied"));
            throw new Error(text("Authorization.AccessDenied"));
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
