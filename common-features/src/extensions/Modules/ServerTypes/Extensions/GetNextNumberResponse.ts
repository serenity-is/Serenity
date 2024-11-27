import { ServiceResponse } from "@serenity-is/corelib";

export interface GetNextNumberResponse extends ServiceResponse {
    Number?: number;
    Serial?: string;
}