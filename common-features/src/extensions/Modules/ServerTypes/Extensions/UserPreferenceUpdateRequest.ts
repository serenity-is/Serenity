import { ServiceRequest } from "@serenity-is/corelib";

export interface UserPreferenceUpdateRequest extends ServiceRequest {
    PreferenceType?: string;
    Name?: string;
    Value?: string;
}