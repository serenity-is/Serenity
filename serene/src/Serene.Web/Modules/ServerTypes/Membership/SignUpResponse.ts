import { ServiceResponse } from "@serenity-is/corelib";

export interface SignUpResponse extends ServiceResponse {
    DemoActivationLink?: string;
}