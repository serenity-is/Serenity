import { ServiceRequest } from "@serenity-is/corelib";

export interface ChangePasswordRequest extends ServiceRequest {
    OldPassword?: string;
    NewPassword?: string;
    ConfirmPassword?: string;
}