/** 
 * 
 * This is the main entry point for `@serenity-is/corelib` quick access functions module. This module mainly contains 
 * helper functions.
 * 
 * The types from this module are available by importing from "@serenity-is/corelib":
 * 
 * ```ts
 * import { htmlEncode, notifyInfo } from "serenity-is/corelib"
 * notifyInfo(htmlEncode("&'<>"));
 * ```
 *   
 * > When using classic namespaces instead of the ESM modules, the types and functions in this module are directly available from the global `Q` namespace
 * > in addition to `Serenity` namespace for compatibility, e.g. `Q.htmlEncode("&'<>")`
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