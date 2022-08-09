export * from "./Q/Arrays";
export * from "./Q/Authorization";
export * from "./Q/BlockUI";
export * from "./Q/Config";
export * from "./Q/Debounce";
export * from "./Q/Dialogs";
export * from "./Q/ErrorHandling";
export * from "./Q/Formatting";
export * from "./Q/Html";
export * from "./Q/Layout";
export * from "./Q/LayoutTimer";
export * from "./Q/LocalText";
export * from "./Q/Lookup";
export * from "./Q/Notify";
export * from "./Q/Router";
export * from "./Q/ScriptData";
export * from "./Q/Services";
export * from "./Q/Strings";
export * from "./Q/System";
export * from "./Q/UserDefinition";
export * from "./Q/ValidateOptions";
export * from "./Q/Validation";
export * from "./Services/Criteria";

export interface ServiceOptions<TResponse extends Serenity.ServiceResponse> extends Serenity.ServiceOptions<TResponse> {
}