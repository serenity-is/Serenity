import { ServiceRequest } from "@serenity-is/corelib";

export interface RolePermissionListRequest extends ServiceRequest {
    RoleID?: number;
}