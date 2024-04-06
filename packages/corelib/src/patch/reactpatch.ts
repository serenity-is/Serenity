import { getGlobalObject } from "../base";

export function reactPatch() {
    let global = getGlobalObject();
    if (!global.React) {
        if (global.preact) {
            global.React = global.ReactDOM = global.preact;
            global.React.Fragment = global.Fragment ?? "x-fragment";
        }
        else {
            global.React = {
                Component: function () { },
                Fragment: "x-fragment",
                createElement: function () { return { _reactNotLoaded: true }; }
            }
            global.ReactDOM = {
                render: function () { throw Error("To use React, it should be included before Serenity.CoreLib.js"); }
            }
        }
    }
}
