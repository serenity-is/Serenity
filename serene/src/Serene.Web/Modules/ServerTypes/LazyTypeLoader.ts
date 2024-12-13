import { Config } from "@serenity-is/corelib";

const loaderByKey = {
    "Serene.Administration.LanguageDialog": async () => (await import("../Administration/Language/LanguageDialog")).LanguageDialog,
    "Serene.Administration.LanguageGrid": async () => (await import("../Administration/Language/LanguageGrid")).LanguageGrid,
    "Serene.Administration.PermissionCheckEditor": async () => (await import("../Administration/UserPermission/PermissionCheckEditor")).PermissionCheckEditor,
    "Serene.Administration.RoleCheckEditor": async () => (await import("../Administration/UserRole/RoleCheckEditor")).RoleCheckEditor,
    "Serene.Administration.RoleDialog": async () => (await import("../Administration/Role/RoleDialog")).RoleDialog,
    "Serene.Administration.RoleGrid": async () => (await import("../Administration/Role/RoleGrid")).RoleGrid
}

Config.lazyTypeLoader = (function(org: any) {
    return (key: string, type: any) => loaderByKey[key]?.() || org?.(key, type);
})(Config.lazyTypeLoader);