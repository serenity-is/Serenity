import { registerEnum } from "@serenity-is/corelib/q";

export {}

export enum CaptureOperationType {
    Before = 0,
    Delete = 1,
    Insert = 2,
    Update = 3
}

registerEnum(CaptureOperationType, 'Serenity.CaptureOperationType');