import { ServiceRequest } from "@serenity-is/corelib";

export interface UserPreferenceRetrieveRequest extends ServiceRequest {
    PreferenceType?: string;
    Name?: string;
}