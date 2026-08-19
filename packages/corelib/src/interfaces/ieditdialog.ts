import { interfaceTypeInfo, nsSerenity, registerType, RetrieveResponse } from "../base";

/**
 * Type token for dialogs that can load an entity by id or instance.
 */
export abstract class IEditDialog {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IEditDialog {
    /**
     * Loads an entity into the dialog.
     * @param entityOrId - Entity instance or primary key value.
     * @param done - Callback invoked after successful load.
     * @param fail - Optional callback invoked on failure. @param p1 - Error details.
     * @returns Promise-like for the retrieve response.
     */
    load(entityOrId: any, done: () => void, fail?: (p1: any) => void): PromiseLike<RetrieveResponse<any>>;
}
