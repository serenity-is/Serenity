import { executeOnceWhenVisible, executeEverytimeWhenVisible } from "@serenity-is/corelib/q";

export namespace LazyLoadHelper {
    export const executeOnceWhenShown = executeOnceWhenVisible 
    export const executeEverytimeWhenShown = executeEverytimeWhenVisible;
}