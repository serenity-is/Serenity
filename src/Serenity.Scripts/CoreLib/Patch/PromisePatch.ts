export function promisePatch() {
    if (typeof Promise === "undefined") {
        // @ts-ignore check for global
        if (typeof (RSVP) !== "undefined") Promise = RSVP as any;
        else if (typeof ($) !== "undefined") {
            Promise = $.Deferred as any;
            (Promise as any).resolve = function (value?: any) {
                return $.Deferred().resolveWith(value);
            }
        }
    }
}

export {}