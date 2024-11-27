import { ServiceRequest } from "@serenity-is/corelib";

export interface ForgotPasswordRequest extends ServiceRequest {
    Email?: string;
}