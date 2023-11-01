import { executeOnceWhenVisible, executeEverytimeWhenVisible } from "../../q";

export namespace LazyLoadHelper {
    export const executeOnceWhenShown = executeOnceWhenVisible 
    export const executeEverytimeWhenShown = executeEverytimeWhenVisible;
}