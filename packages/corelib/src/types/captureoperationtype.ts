import { registerEnum } from "../base";

export { };

/**
 * Operation type for data change capture (used by history / audit features).
 */
export enum CaptureOperationType {
    /** Fired before the operation; allows cancellation or modification. */
    Before = 0,
    /** Entity deletion. */
    Delete = 1,
    /** Entity insertion. */
    Insert = 2,
    /** Entity update. */
    Update = 3
}

registerEnum(CaptureOperationType, 'Serenity.CaptureOperationType');