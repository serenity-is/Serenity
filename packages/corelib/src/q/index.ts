/** 
 * 
 * This is the main entry point for `@serenity-is/corelib/q` module. This module mainly contains 
 * helper functions.
 * 
 * The types from this module are available by importing from "@serenity-is/corelib/q":
 * 
 * ```ts
 * import { htmlEncode, notifyInfo } from "serenity-is/corelib/q"
 * notifyInfo(htmlEncode("&'<>"));
 * ```
 *   
 * > When using classic namespaces instead of the ESM modules, the types and functions in this module are directly available from the global `Q` namespace.
 * > e.g. `Q.htmlEncode("&'<>")`
 * @packageDocumentation
 * @module corelib/q
 */

export * from "./arrays";
export * from "./authorization";
export * from "./blockui";
export * from "./config";
export * from "./debounce";
export * from "./dialogs";
export * from "./errorhandling";
export * from "./formatting";
export * from "./html";
export * from "./layout";
export * from "./layouttimer";
export * from "./localtext";
export * from "./lookup";
export * from "./notify";
export * from "./propertyitem";
export * from "./router";
export * from "./scriptdata";
export * from "./servicetypes";
export * from "./services";
export * from "./strings";
export * from "./system";
export * from "./toastr2";
export * from "./userdefinition";
export * from "./validateoptions";
export * from "./validation";
export * from "./criteria";