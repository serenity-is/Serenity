import { ServiceRequest } from "@serenity-is/corelib";

export interface UserRoleUpdateRequest extends ServiceRequest {
    UserID?: number;
    Roles?: number[];
}