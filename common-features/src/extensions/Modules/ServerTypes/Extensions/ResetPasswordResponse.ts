import { ServiceResponse } from "@serenity-is/corelib";

export interface ResetPasswordResponse extends ServiceResponse {
    RedirectHome?: boolean;
}