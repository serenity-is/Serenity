import { executeOnceWhenShown as once, executeEverytimeWhenShown as every } from "../../q";

export namespace LazyLoadHelper {
    export const executeOnceWhenShown = once;
    export const executeEverytimeWhenShown = every;
}