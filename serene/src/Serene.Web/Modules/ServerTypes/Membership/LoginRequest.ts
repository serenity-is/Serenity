import { ServiceRequest } from "@serenity-is/corelib";

export interface LoginRequest extends ServiceRequest {
    Username?: string;
    Password?: string;
}