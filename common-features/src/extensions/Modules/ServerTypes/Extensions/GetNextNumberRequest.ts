import { ServiceRequest } from "@serenity-is/corelib";

export interface GetNextNumberRequest extends ServiceRequest {
    Prefix?: string;
    Length?: number;
}