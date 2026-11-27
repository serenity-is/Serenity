import { ServiceResponse } from "@serenity-is/corelib";

export interface PermissionGrantListResponse extends ServiceResponse {
    IsSuperAdmin?: boolean;
    Denied?: { [key: string]: string };
}