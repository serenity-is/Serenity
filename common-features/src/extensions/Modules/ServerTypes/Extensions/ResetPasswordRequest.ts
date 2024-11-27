import { ServiceRequest } from "@serenity-is/corelib";

export interface ResetPasswordRequest extends ServiceRequest {
    Token?: string;
    NewPassword?: string;
    ConfirmPassword?: string;
}