/// <reference path="../../../typings/jquery/jquery.d.ts" />
/// <reference path="../Imports/SS.ts" />

namespace Q {
    export function coalesce(a: any, b: any): any {
        return (ss as any).coalesce(a, b);
    }

    export function isValue(a: any): boolean {
        return (ss as any).isValue(a);
    }

    // derived from https://github.com/mistic100/jQuery.extendext/blob/master/jQuery.extendext.js
    export function deepClone<TItem>(arg1: TItem, ...args: TItem[]): TItem {
        let options: any,
            name: string,
            src: any,
            copy: any,
            copyIsArray: boolean,
            clone: any,
            target = arguments[0] || {},
            i = 1,
            length = arguments.length;

        // Handle case when target is a string or something (possible in deep copy)
        if (typeof target !== "object" && !$.isFunction(target)) {
            target = {};
        }
        if (i === length) {
            target = {};
            i = 0;
        }
        for (; i < length; i++) {
            // Only deal with non-null/undefined values
            if ((options = arguments[i]) !== null) {
                // Special operations for arrays
                if ($.isArray(options)) {
                    target = $.extend(true, [], options);
                }
                else {
                    // Extend the base object
                    for (name in options) {
                        src = target[name];
                        copy = options[name];
                        // Prevent never-ending loop
                        if (target === copy) {
                            continue;
                        }
                        // Recurse if we're merging plain objects or arrays
                        if (copy && ($.isPlainObject(copy) ||
                            (copyIsArray = $.isArray(copy)))) {
                            if (copyIsArray) {
                                copyIsArray = false;
                                clone = src && $.isArray(src) ? src : [];
                            }
                            else {
                                clone = src && $.isPlainObject(src) ? src : {};
                            }
                            // Never move original objects, clone them
                            target[name] = deepClone(clone, copy);
                        }
                        else if (copy !== undefined) {
                            target[name] = copy;
                        }
                    }
                }
            }
        }
        // Return the modified object
        return target;
    }
}