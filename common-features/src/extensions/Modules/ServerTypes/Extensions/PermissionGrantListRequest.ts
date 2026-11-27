import { ServiceRequest } from "@serenity-is/corelib";

export interface PermissionGrantListRequest extends ServiceRequest {
    PermissionKeys?: string[];
}