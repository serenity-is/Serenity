import { ServiceRequest } from "@serenity-is/corelib";

export interface UserPermissionListRequest extends ServiceRequest {
    UserID?: number;
}