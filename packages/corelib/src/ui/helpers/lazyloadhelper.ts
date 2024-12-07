import { executeOnceWhenVisible, executeEverytimeWhenVisible } from "../../compat";

export namespace LazyLoadHelper {
    export const executeOnceWhenShown = executeOnceWhenVisible 
    export const executeEverytimeWhenShown = executeEverytimeWhenVisible;
}