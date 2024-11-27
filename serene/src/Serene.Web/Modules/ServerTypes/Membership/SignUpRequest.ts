import { ServiceRequest } from "@serenity-is/corelib";

export interface SignUpRequest extends ServiceRequest {
    DisplayName?: string;
    Email?: string;
    Password?: string;
}