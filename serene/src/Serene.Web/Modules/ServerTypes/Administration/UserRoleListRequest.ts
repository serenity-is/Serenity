import { ServiceRequest } from "@serenity-is/corelib";

export interface UserRoleListRequest extends ServiceRequest {
    UserID?: number;
}