import { ServiceResponse } from "@serenity-is/corelib";

export interface SendResetPasswordResponse extends ServiceResponse {
    DemoLink?: string;
}