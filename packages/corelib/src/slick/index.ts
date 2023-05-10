/** 
 * 
 * This is the main entry point for `@serenity-is/corelib/slick` module. This module mainly contains 
 * some Serenity specific extensions for sleekgrid like `RemoteView`.
 * 
 * The types from this module are available by importing from "@serenity-is/corelib/slick" module:
 * 
 * ```ts
 * import { RemoteView } from "@serenity-is/corelib/slick";
 * export class MyCustomView extends RemoveView {
 * }
 * ```
 *   
 * > When using classic namespaces instead of the ESM modules, the types and functions in this module are directly available from the global `Slick` namespace.
 * > e.g. `Slick.RemoveView`
 * @packageDocumentation
 * @module corelib/slick
 */

export * from "./aggregators";
export * from "./remoteview";
export * from "./slicktypes";